using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Utils;

[ExecuteInEditMode]
public class Waypoint : MatrixNode
{
    // Mouse position for drawing a line between the waypoint and the mouse when shift is pressed
    public Vector3 mousePos = new(0, 0, 0);
    public bool Selected { get; set; }
    public TrafficController controller;
    public Matrix<Waypoint> Matrix => controller.matrix;

    public Waypoint[] Successors => Matrix.GetSuccessors(this);
    public Waypoint[] Predecessors => Matrix.GetPredecessors(this);

    // Each waypoint has a list of path builders, one for each successor
    // This ables to manage a different curved path for each successor
    public List<PathBuilder> pathBuilders = new();

    // Path type to determine the way the mobile object will move from this waypoint to the next
    public PathType pathType = PathType.Straight;

    // Show paths is used to toggle the visibility of the paths
    public bool showPaths = true;

    // Speed is used to determine the speed of the mobile object when it moves from this waypoint to the next
    public float speedLimit = 10f;

    // Spawn type is used to determine if a mobile object will spawn from this waypoint
    // and which type of mobile object will spawn
    public SpawnType spawnType = SpawnType.None;

    // If this waypoint is a destination, the mobile object will be destroyed when it reaches this waypoint
    public bool isDestination;

    // Spawn rate is used to determine the frequency of the mobile object spawning
    public float spawnRate = 50;

    // Spawn amount is used to determine how many mobile objects will spawn
    public int spawnAmount = 1;

    // Spawn behavior is used to determine how to spawn the mobile objects
    public SpawnBehavior spawnBehavior = SpawnBehavior.Amount;

    // Spawn interval is used to determine the time between each spawn
    private float spawnInterval;
    private int currentSpawnAmount;

    private IEnumerator Start()
    {
        currentSpawnAmount = spawnAmount;
        yield return new WaitUntil(() => controller.IsInitialized);
        if (spawnType is not SpawnType.None && !isDestination &&
            controller.destinationsWaypoints.Length > 0)
            SpawnMobileObject();
    }

    private void SpawnMobileObject()
    {
        var destinations = new List<Waypoint>(controller.destinationsWaypoints);
        if (spawnType is SpawnType.Vehicle)
        {
            int randomIndex = -1;
            while (destinations.Count > 0 && !Matrix.IsReachable(this,
                       destinations[randomIndex = controller.random.Next(0, destinations.Count)]))
            {
                destinations.RemoveAt(randomIndex);
                randomIndex = -1;
            }

            if (randomIndex > -1)
            {
                Vehicle vehicle = controller.AddVehicle(this).GetComponent<Vehicle>();
                vehicle.from = this;
                vehicle.to = destinations[randomIndex];

                if (spawnBehavior is SpawnBehavior.Amount && currentSpawnAmount > 1)
                {
                    currentSpawnAmount -= 1;
                    Invoke(nameof(SpawnMobileObject), 1);
                }
                else if (spawnBehavior is SpawnBehavior.Interval)
                {
                    spawnInterval = controller.random.Next((int) (100 - spawnRate), (int) (105 - spawnRate));
                    Invoke(nameof(SpawnMobileObject), spawnInterval);
                }
            }
        }
    }


    private void OnDestroy()
    {
        // Remove all the curves that are connected to this waypoint
        foreach (var predecessor in Predecessors)
        {
            if (predecessor.pathType is PathType.Curved)
            {
                PathBuilder predecessorPathBuilder = predecessor.pathBuilders.Find(_p => _p.destination.id == id);
                DestroyImmediate(predecessorPathBuilder.gameObject);
                predecessor.pathBuilders.Remove(predecessorPathBuilder);
            }
        }

        Matrix.Remove(this);
    }

    /**
     * Toggle the visibility of the paths
     */
    public void TogglePaths()
        => pathBuilders.ForEach(_p => _p.gameObject.SetActive(showPaths));

    /**
     * Get the position on the curve between this waypoint and the given successor
     * @param _successor The successor of this waypoint
     * @param _t The t parameter of the curve that a mobile object will use to move
     */
    public Vector3 GetPositionOnCurve(Waypoint _successor, float _t)
    {
        if (pathType != PathType.Curved) return new Vector3();

        // We need to find the path builder that is connected to the given successor
        PathBuilder pathBuilder = pathBuilders.Find(_p => _p.destination.id == _successor.id);
        return pathBuilder.GetPositionOnCurve(_t);
    }


    /// <summary>
    /// Detroy or create the path builders depending on the path type
    /// </summary>
    public void UpdatePathType()
    {
        if (pathType is PathType.Curved)
        {
            foreach (var successor in Successors)
            {
                // If the successor is not already connected to this waypoint by a curve, create a new path builder
                if (!pathBuilders.Exists(_p => _p.destination.id == successor.id))
                {
                    GameObject pathBuilderObj = new GameObject("Path builder");
                    PathBuilder pathBuilder = pathBuilderObj.AddComponent<PathBuilder>();
                    pathBuilderObj.transform.position = transform.position;
                    pathBuilderObj.transform.parent = transform;
                    pathBuilder.Load(this, successor);
                    pathBuilders.Add(pathBuilder);
                }
            }
        }
        else
        {
            foreach (var pathBuilder in pathBuilders)
                DestroyImmediate(pathBuilder.gameObject);
            pathBuilders.Clear();
        }
    }

    /**
     * Merge this waypoint with another one if they are close enough
     */
    public void Merge()
    {
        int i = 0;
        while (i < Matrix.Count && i > -1)
        {
            Waypoint to = Matrix.Get(i);
            if (Vector3.Distance(transform.position, to.transform.position) < 1 &&
                id != to.id)
            {
                i = -1;
                foreach (var successor in Successors
                             .Where(_successor => !to.Successors.Contains(_successor) && to.id != _successor.id))
                    Matrix.Link(to, successor);

                foreach (var predecessor in Predecessors
                             .Where(_predecessor =>
                                 !to.Predecessors.Contains(_predecessor) && _predecessor.id != to.id))
                    Matrix.Link(predecessor, to);

                DestroyImmediate(transform.gameObject);
            }
            else
                i++;
        }
    }

    /**
     * Draw an arrow between two points
     */
    private void DrawGizmoArrow(Vector3 _pos, Vector3 _direction, Color _color, float _arrowHeadLength = 0.25f,
        float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = _color;
        Gizmos.DrawRay(_pos, _direction);

        Vector3 right = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                        new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                       new Vector3(0, 0, 1);
        Gizmos.DrawRay(_pos + _direction, right * _arrowHeadLength);
        Gizmos.DrawRay(_pos + _direction, left * _arrowHeadLength);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawCube(position, new Vector3(1, 1, 1));

        foreach (Waypoint successor in Successors)
            DrawGizmoArrow(position, successor.transform.position - position, Color.cyan, 1, 40);

        if (Selected)
            Gizmos.DrawLine(position, mousePos);
    }
}