using Enums;
using UnityEngine;

public class Vehicle : MobileObject
{
    public new void Start()
    {
        //Setting up vehicle type first
        if (this.vehicleType == VehicleType.Bus) //If vehicle is Bus
        {
            collider = GetComponent<BoxCollider>();
            sportiness = controller.random.Next(10, 50) / 100f;
            ((BoxCollider) collider).size = new Vector3(1.5f, 1.5f, 1.5f);
        }
        else //We assume it is a car
        {
            collider = GetComponent<SphereCollider>();
            sportiness = controller.random.Next(50, 100) / 100f;
            ((SphereCollider) collider).radius = 1.5f;
        }
        
        base.Start();
    }

    public static GameObject AddVehicle(TrafficController controller, Waypoint waypoint = null)
    {
        int id = controller.vehicles.Count + 1;
        GameObject obj;
        Vehicle vehicle;

        int randomNum = controller.random.Next() % 10;
        if (randomNum < 7) //If less then 7 we want to make a car
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            vehicle = obj.AddComponent<Vehicle>();
            vehicle.vehicleType = VehicleType.Car;
        }
        else //We want to make a bus
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vehicle = obj.AddComponent<Vehicle>();
            vehicle.vehicleType = VehicleType.Bus;
        }

        obj.layer = LayerMask.NameToLayer("MobileObject");
        obj.name = "Vehicle " + id;
        if (waypoint != null)
        {
            obj.transform.position = waypoint.transform.position;
            obj.transform.parent = waypoint.transform;
        }
        else
        {
            obj.transform.position = controller.transform.position;
            obj.transform.parent = controller.transform;
        }

        vehicle.id = id;
        vehicle.controller = controller;
        controller.vehicles.Add(vehicle);

        return obj;
    }
}