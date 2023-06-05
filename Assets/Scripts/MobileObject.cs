using System;
using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using Utils;

/**
 * Class that represents a mobile object in the scene.
 * That is, a car, a pedestrian, etc.
 * All the logic for moving the object is here.
 * Then, implements different behaviours in subclasses for each type of object.
 */
public class MobileObject : MonoBehaviour
{
    // Id of the object
    public int id;

    // Starting waypoint
    public Waypoint from;

    // Destination waypointf
    public Waypoint to;

    public TrafficController controller;

    // Path to follow after generating it with dijkstra's algorithm
    private Waypoint[] path;

    // Current index in the path
    private int currentIndex = 0;

    // Current t parameter for the curve calculation
    private float tParam = 0;

    [SerializeField] public float speed;
    public float baseSpeed;

    protected float Sportiness = 0;

    protected Collider Collider;

    //Type of vehicle
    public VehicleType vehicleType;

    [SerializeField] private bool isStopped = false;

    public int currentLane = 0;

    protected void Start()
    {
        // Set the initial position to the starting waypoint
        // and generate the path
        transform.position = from.transform.position;

        path = FindShortestPath();
        if (path.Length > 0)
        {
            speed = path[0].currentSpeedLimit * Sportiness;
            baseSpeed = speed;
        }

        Collider.isTrigger = true;
        Rigidbody rigidBody = this.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }


    void FixedUpdate()
    {
        if (isStopped)
        {
            speed = 0;
            return;
        }

        // Checks if the object has reached the next waypoint
        if (currentIndex < path.Length - 1)
        {
            Waypoint nextWp = path[currentIndex + 1];
            Waypoint currentWp = path[currentIndex];

            float distanceBtWps = Vector3.Distance(path[currentIndex].transform.position,
                nextWp.transform.position);
            float distanceWithWp = Vector3.Distance(transform.position, nextWp.transform.position);
            float distanceWaypointDetector = distanceBtWps * Sportiness > 20 ? 20 : distanceBtWps * Sportiness;

            float dist = IsObjectInFront(out MobileObject otherMo);
            if (otherMo != null)
            {
                bool checkDistance = true;
                if (nextWp.Lanes != null && currentWp.Lanes != null && otherMo.currentLane == currentLane &&
                    otherMo.speed < speed * 0.8)
                {
                    Waypoint newWp = nextWp.Lanes.GetSiblingWaypoint(this, true);
                    if (newWp != null)
                    {
                        currentWp.Lanes.RemoveVehicle(this);
                        currentLane += 1;
                        currentWp.Lanes.AddVehicle(this);
                        path[currentIndex + 1] = newWp;
                        checkDistance = false;
                    }
                }

                if (checkDistance)
                {
                    if (dist < 2)
                    {
                        speed = 0;
                    }
                    else if (Math.Abs(dist - 2) < 0.1)
                    {
                        speed = otherMo.speed;
                    }
                    else
                    {
                        speed = baseSpeed * Tools.Remap(dist, 10, 0,
                            1, otherMo.speed / baseSpeed);
                    }
                }
            }
            else if (distanceWithWp < distanceWaypointDetector)
            {
                speed = baseSpeed * Tools.Remap(distanceWithWp, distanceWaypointDetector, 0,
                    1, (nextWp.currentSpeedLimit * Sportiness) / baseSpeed);
            }

            if (distanceWithWp < 1)
            {
                if (currentWp.Lanes != null)
                    currentWp.Lanes.RemoveVehicle(this);

                currentIndex++;
                speed = nextWp.currentSpeedLimit * Sportiness;
                baseSpeed = speed;
                tParam = 0;

                if (nextWp.Lanes != null)
                {
                    if (currentWp.Lanes == null)
                    {
                        currentLane = nextWp.Lanes.GetCurrentLane(nextWp);
                    }

                    if (currentIndex + 1 < path.Length - 1 && path[currentIndex + 1].Lanes != null)
                    {
                        if (nextWp.Lanes.CanDownLane(this))
                        {
                            Waypoint newWp = path[currentIndex + 1].Lanes.GetSiblingWaypoint(this, false);
                            if (newWp != null)
                            {
                                currentLane -= 1;
                                path[currentIndex + 1] = newWp;
                            }
                        }
                        else
                        {
                            path[currentIndex + 1] = path[currentIndex + 1].Lanes.GetWaypoint(this);
                        }
                    }

                    nextWp.Lanes.AddVehicle(this);
                }
                else
                    currentLane = 0;
            }
        }

        if (float.IsNaN(speed))
            speed = 0;

        if (currentIndex < path.Length - 1)
        {
            Waypoint current = path[currentIndex];

            if (current.IsInTrafficLights && current.currentSpeedLimit > 0 && speed == 0)
            {
                speed = current.currentSpeedLimit * Sportiness;
                baseSpeed = speed;
            }

            Vector3 destination = current.pathType is PathType.Straight
                ? path[currentIndex + 1].transform.position
                : current.GetPositionOnCurve(path[currentIndex + 1], tParam);
            transform.LookAt(destination);
            transform.Translate(0, 0, speed * Time.deltaTime);
            tParam += Time.deltaTime * speed / Vector3.Distance(current.transform.position,
                path[currentIndex + 1].transform.position);
        }
        else
        {
            if (this is Vehicle)
            {
                controller.vehicles.Remove((Vehicle) this);
                DestroyImmediate(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("MobileObject"))
        {
            MobileObject mobileObject = other.GetComponent<MobileObject>();
            if (mobileObject != null && !mobileObject.isStopped) isStopped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("MobileObject"))
        {
            isStopped = false;
            other.GetComponent<MobileObject>().isStopped = false;
        }
    }

    private float IsObjectInFront(out MobileObject mobileObject)
    {
        bool detector = Physics.Raycast(transform.position, transform.forward, out var hit, 10f,
            1 << LayerMask.NameToLayer("MobileObject"));
        mobileObject = detector ? hit.transform.GetComponent<MobileObject>() : null;
        return hit.distance;
    }


    public Waypoint[] FindShortestPath()
    {
        var matrix = controller.matrix;
        int graphSize = matrix.Count; //Size of graph

        int[] vertexPredecessors = new int[graphSize]; //node predecessors
        bool[] checkedSet = new bool[graphSize]; //node checked flag
        int[] distanceValues = new int[graphSize]; //nodes distance from source node

        //Setting all distance values to infinity
        for (int i = 0; i < graphSize; i++)
        {
            distanceValues[i] = Int32.MaxValue;
            checkedSet[i] = false;
        }

        distanceValues[from.id] = 0; //Source is 0 from its self

        //You run algorithm one less then the size of the graph
        //If you run again and it returns true then you have a negative closed loop in the graph
        //Which is impossible to have in our situation so no need to check for 
        for (int i = 0; i < graphSize - 1; i++)
        {
            //Get the lowest value in the "distanceValues" that "checkSet" flag is set to false
            int minIndex = minDistance(distanceValues, checkedSet);

            checkedSet[minIndex] = true; //Setting checked on vertex to "true"

            for (int j = 0; j < graphSize; j++)
            {
                //This If statement is checking for 3 things
                //1 - If "j" has not been checked yet
                //2 - If there is an edge between verticies at "minIndex" and "j"
                //3 - If The distance from minIndex + the edge from minIndex vertex to j is less than the current calculated distance to j
                //If all of these pass. Then we put the distance value of "j" as === distance to "minIndex" + edge of "minIndex" and "j"
                //We also keep track of the node we took to get to the shortest path to vertext "j" so we store "minIndex" in the "vertexPredecessors" array at index "j"
                if (!checkedSet[j] && matrix.GetWeight(minIndex, j) != 0 &&
                    distanceValues[minIndex] != Int32.MaxValue &&
                    distanceValues[minIndex] + matrix.GetWeight(minIndex, j) < distanceValues[j])
                {
                    distanceValues[j] = distanceValues[minIndex] + matrix.GetWeight(minIndex, j);
                    vertexPredecessors[j] = minIndex;
                }
            }
        }

        /*Now that the algorithm is complete we must build the path starting from the 
            Sink and working our way to the source Using the "vertexPredessesors array*/
        List<Waypoint> pathList = new List<Waypoint>();
        int indexPtr = to.id; //Points to the current vertex 
        while (indexPtr != from.id)
        {
            pathList.Insert(0, matrix.Get(indexPtr));
            indexPtr = vertexPredecessors[indexPtr];
        }

        pathList.Insert(0, from); //Adding the source to the path
        return pathList.ToArray();
    }

//Method gets the smallest distance in the distances array that hasn't been checked yet
    private int minDistance(int[] _distances, bool[] _checkedSet)
    {
        int minValue = Int32.MaxValue;
        int minIndex = 1;

        //Simple search for smallest number in distances array
        for (int i = 0; i < _distances.Length; i++)
        {
            if (!_checkedSet[i] && _distances[i] <= minValue)
            {
                //Checks if lower && if it hasn't been checked yet
                minValue = _distances[i];
                minIndex = i;
            }
        }

        return minIndex;
    }
}