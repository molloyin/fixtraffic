using System;
using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
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

    // Destination waypoint
    public Waypoint to;

    public TrafficController controller;

    // Path to follow after generating it with dijkstra's algorithm
    private Waypoint[] path;

    // Current index in the path
    private int currentIndex = 0;

    // Current t parameter for the curve calculation
    private float tParam = 0;

    [SerializeField] private float speed = 0;

    [SerializeField] private float baseSpeed = 0;

    private float sportiness = 0;

    private new SphereCollider collider;

    [SerializeField] private bool isStopped = false;

    void Start()
    {
        // Set the initial position to the starting waypoint
        // and generate the path
        transform.position = from.transform.position;


        path = FindShortestPath();
        if (path.Length > 0)
        {
            speed = path[0].speedLimit;
            baseSpeed = speed;
        }

        collider = GetComponent<SphereCollider>();
        sportiness = controller.random.Next(10, 100) / 100f;
        collider.isTrigger = true;
        collider.radius = 1.5f;
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
            float distanceBtWps = Vector3.Distance(path[currentIndex].transform.position,
                nextWp.transform.position);
            float distanceWithWp = Vector3.Distance(transform.position, nextWp.transform.position);

            float dist = IsObjectInFront(out MobileObject mobileObject);

            if (mobileObject != null)
            {
                if (dist < 1.5)
                {
                    speed = mobileObject.speed;
                    if (speed == 0) Debug.Log(transform.name);
                }
                else
                {
                    speed = baseSpeed * Tools.Remap(dist, 10, 0,
                        1, mobileObject.speed / baseSpeed);
                }
            }
            else if (distanceWithWp < distanceBtWps * sportiness)
            {
                speed = baseSpeed * Tools.Remap(distanceWithWp, distanceBtWps * sportiness, 0,
                    1, nextWp.speedLimit / baseSpeed);
            }


            if (distanceWithWp < 1)
            {
                if (nextWp.carRemover)
                {
                    Debug.Log("IN HERE");
                    //Then we want to destroy this object
                    bool removedCar = controller.removeVehicle(this.id);

                    if (removedCar)
                    {
                        Debug.Log("Removing Game Object");
                        Destroy(gameObject); //Destroying game object
                    }
                }

                currentIndex++;
                tParam = 0;
                speed = nextWp.speedLimit;
                baseSpeed = speed;
            }
        }

        if (currentIndex < path.Length - 1)
        {
            Waypoint current = path[currentIndex];
            Vector3 destination = current.pathType is PathType.Straight
                ? path[currentIndex + 1].transform.position
                : current.GetPositionOnCurve(path[currentIndex + 1], tParam);
            transform.LookAt(destination);
            transform.Translate(0, 0, speed * Time.deltaTime);
            tParam += Time.deltaTime * speed / Vector3.Distance(current.transform.position,
                path[currentIndex + 1].transform.position);
        } else
        {
            //Checking to see if next waypoint is a remover
            Waypoint currentWp = path[currentIndex];
            Debug.Log(currentWp.carRemover);
            



            //Path is finished and we need to create a new path
            from = path[currentIndex];
            setNewDestination();
            path = FindShortestPath();
            currentIndex = 0;
            
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
            isStopped = false;
    }

    private float IsObjectInFront(out MobileObject mobileObject)
    {
        bool detector = Physics.Raycast(transform.position, transform.forward, out var hit, 10f,
            1 << LayerMask.NameToLayer("MobileObject"));
        mobileObject = detector ? hit.transform.GetComponent<MobileObject>() : null;
        return hit.distance;
    }

    private void OnDrawGizmos()
    {
        if (isStopped)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.7f);
        }
    }


    /*
    This is am Implementation of dijkstra's algorithm in C# 
    This takes int a 2D array "int[,] matrix" which should be
    an adjacnecy matrix for the graph.
    The "int source", and "int sink" are the start and finish nodes
    for the algorihtm to find the shortest path between
*/
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



    //Method is used to get random destination and set it to the sink
    private void setNewDestination()
    {
        do
        {
            to = controller.destinationsWaypoints[controller.random.Next(0, controller.destinationsWaypoints.Length)];
        } while (to == from);
    }
}