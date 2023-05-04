using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Random = System.Random;

/**
 * Heart of all the traffic system.
 */
public class TrafficController : MonoBehaviour
{
    public Matrix<Waypoint> matrix = new("Waypoint");
    [SerializeField] public List<Vehicle> vehicles = new();
    public int simulationSeed = Guid.NewGuid().GetHashCode();
    public Random random;
    [NonSerialized] public Waypoint[] destinationsWaypoints;
    public bool IsInitialized { get; private set; }

    [SerializeField] Camera mainCamera; //Field for the camera
    private int carPtr;
    private int yOffset = 20;

    private void Start()
    {
        random = new Random(simulationSeed);
        destinationsWaypoints = matrix.Vertices.Where(_v => _v.isDestination).ToArray();
        Debug.Log(destinationsWaypoints.Length);
        IsInitialized = true;

        mainCamera = Camera.main;
        mainCamera.enabled = true; //Enabling Camera
        carPtr = 0; //By default is 0 looking at car 0
    }

    /**
     * Add a new waypoint to the system. Register it in the matrix and update the path type of the previous waypoint.
     * @param _from The previous waypoint. If null, the waypoint will be added at the center of the system.
     * @return The game object of the new waypoint.
     */
    public GameObject AddWaypoint(Waypoint _from = null)
    {
        if (_from == null && matrix.Count > 0) return null;

        int id = matrix.GetNewIndex();

        GameObject obj = new GameObject("Waypoint " + id);
        Waypoint waypoint = obj.AddComponent<Waypoint>();
        waypoint.id = id;
        waypoint.controller = this;

        obj.transform.parent = transform;
        obj.transform.position = _from == null ? transform.position : _from.mousePos;

        matrix.Add(waypoint, _from);
        if (_from != null) _from.UpdatePathType();

        return obj;
    }

    /**
     * Add a new vehicle to the system.
     * @return The game object of the new vehicle.
     */
    public GameObject AddVehicle(Waypoint _waypoint = null)
    {
        int id = vehicles.Count + 1;
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Vehicle vehicle = obj.AddComponent<Vehicle>();
        obj.layer = LayerMask.NameToLayer("MobileObject");
        obj.name = "Vehicle " + id;
        if (_waypoint != null)
        {
            obj.transform.position = _waypoint.transform.position;
            obj.transform.parent = _waypoint.transform;
        }
        else
        {
            obj.transform.position = transform.position;
            obj.transform.parent = transform;
        }

        vehicle.id = id;
        vehicle.controller = this;
        vehicles.Add(vehicle);

        return obj;
    }


    public Boolean removeVehicle(int vehicleId)
    {
        int index = -1;

        for(int i = 0; i < vehicles.Count; i++)
        {
            if(vehicles[i].id == vehicleId)
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            vehicles.RemoveAt(index);
            return true;
        }
        return false;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(this.carPtr < this.vehicles.Count-1)
            {
                Debug.Log("Incrementing " + this.carPtr);
                this.carPtr++; //Increment
            } else
            {
                Debug.Log("Jump " + this.carPtr);
                this.carPtr = 0; //Jump round to start
            }
        }
    }

    void FixedUpdate()
    {
        if(vehicles.Count > 0)
        {
            Vehicle vehiclePtr = this.vehicles[carPtr];


            //Now we want to set the cemeras position to the same x and z
            //And increase the y
            Vector3 newPosition = vehiclePtr.transform.position;
            newPosition.y += this.yOffset;
            this.mainCamera.transform.position = newPosition;

        } else
        {
            //Place cemera in the sky ?
        }
    }
}