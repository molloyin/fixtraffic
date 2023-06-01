using Enums;
using UnityEngine;

public class Vehicle : MobileObject
{
    public new void Start()
    {
        collider = GetComponent<SphereCollider>();
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
            obj = Instantiate(controller.vehicleModel);
            vehicle = obj.AddComponent<Vehicle>();
            vehicle.vehicleType = VehicleType.Car;
            vehicle.sportiness = controller.random.Next(50, 100) / 100f;
        }
        else //We want to make a bus
        {
            obj = Instantiate(controller.busModel);
            vehicle = obj.AddComponent<Vehicle>();
            vehicle.vehicleType = VehicleType.Bus;
            vehicle.sportiness = controller.random.Next(10, 50) / 100f;
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
        SphereCollider collider = obj.AddComponent<SphereCollider>();
        collider.radius = 1.5f;

        return obj;
    }
}