using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = System.Random;

/**
 * Heart of all the traffic system.
 */
public class TrafficController : MonoBehaviour
{
    public Matrix<Waypoint> matrix = new("Waypoint");

    [SerializeField] public List<TrafficLights> trafficLights = new();
    [SerializeField] public List<Vehicle> vehicles = new();
    [NonSerialized] public Waypoint[] destinationsWaypoints;
    
    public GameObject vehicleModel;
    
    public int simulationSeed = Guid.NewGuid().GetHashCode();
    public Random random;
    public bool IsInitialized { get; private set; }

    [SerializeField] Camera mainCamera; //Field for the camera
    private int carPtr;
    private int yOffset = 20;

    //Fields for spawning
    private float spawnDelay = 5f; //In seconds
    public bool defaultSpawnBehavior = false;

    private void Start()
    {
        random = new Random(simulationSeed);
        destinationsWaypoints = matrix.Vertices.Where(v => v.isDestination).ToArray();
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

    public GameObject AddTrafficLights()
        => TrafficLights.Add(this);

    /**
     * Add a new vehicle to the system.
     * @return The game object of the new vehicle.
     */
    public GameObject AddVehicle(Waypoint waypoint = null)
        => Vehicle.AddVehicle(this, waypoint);

    void SpawnAllVehicles()
    {
        foreach (var matrixVertex in matrix.Vertices)
        {
            matrixVertex.SpawnMobileObject();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.carPtr < this.vehicles.Count - 1)
            {
                Debug.Log("Incrementing " + this.carPtr);
                this.carPtr++; //Increment
            }
            else
            {
                Debug.Log("Jump " + this.carPtr);
                this.carPtr = 0; //Jump round to start
            }
        }

        //When we press Q we want to stop the spawning
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Stopping spawn");
            CancelInvoke();
        }

        //When we press W we want to start the spawning
        if (Input.GetKeyDown(KeyCode.W) && !defaultSpawnBehavior)
        {
            Debug.Log("Starting spawn");
            InvokeRepeating(nameof(SpawnAllVehicles), 1f, spawnDelay);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CancelInvoke();
            defaultSpawnBehavior = !defaultSpawnBehavior;
            Debug.Log(
                defaultSpawnBehavior ? "Waypoint default spawning behaviors enabled" : "Interval spawning enabled");
            if (defaultSpawnBehavior) SpawnAllVehicles();
        }
    }

    // void FixedUpdate()
    // {
    //     if (vehicles.Count > 0)
    //     {
    //         Vehicle vehiclePtr = this.vehicles[carPtr];
    //
    //
    //         //Now we want to set the cemeras position to the same x and z
    //         //And increase the y
    //         Vector3 newPosition = vehiclePtr.transform.position;
    //         newPosition.y += this.yOffset;
    //         this.mainCamera.transform.position = newPosition;
    //     }
    //     else
    //     {
    //         //Place cemera in the sky ?
    //     }
    // }
}