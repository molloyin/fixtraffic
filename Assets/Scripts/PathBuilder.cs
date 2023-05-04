using System;
using UnityEngine;

/**
 * PathBuilder class to build complex paths between waypoints
 * For now it only supports straight and curved paths
 */
public class PathBuilder : MonoBehaviour
{
    // Starting waypoint
    [SerializeField] public Waypoint source;
    // Ending waypoint, successor of the starting waypoint
    [SerializeField] public Waypoint destination;

    // Control points for managing the curve
    [SerializeField] private GameObject[] controlPoints;
    // Current gizmos position for drawing the path helper lines
    private Vector3 gizmosPosition;

    /**
     * Load method to load the path between two waypoints
     * @param _source: Starting waypoint
     * @param _destination: Ending waypoint, successor of the starting waypoint
     */
    public void Load(Waypoint _source, Waypoint _destination)
    {
        // This function creates the path between two waypoints
        // It summons the control points
        
        controlPoints = new GameObject[2];
        source = _source;
        destination = _destination;
        GameObject objA = new GameObject("Control Point 1");
        GameObject objB = new GameObject("Control Point 2");

        objA.AddComponent<ControlPoint>();
        objB.AddComponent<ControlPoint>();

        objA.transform.parent = transform;
        objB.transform.parent = transform;
        var mid = (source.transform.position + destination.transform.position) / 2;
        objA.transform.position = mid;
        objB.transform.position = mid;

        controlPoints[0] = objA;
        controlPoints[1] = objB;
    }

    private void OnDestroy()
    {
        DestroyImmediate(controlPoints[0]);
        DestroyImmediate(controlPoints[1]);
    }

    /**
     * Compute the position on the curve at a given time by using the Bezier curve formula
     * @param _t: Time
     * @return: Position on the curve at time _t
     */
    public Vector3 GetPositionOnCurve(float _t)
        => Mathf.Pow(1 - _t, 3) * source.transform.position +
           3 * Mathf.Pow(1 - _t, 2) * _t * controlPoints[0].transform.position +
           3 * (1 - _t) * Mathf.Pow(_t, 2) * controlPoints[1].transform.position +
           Mathf.Pow(_t, 3) * destination.transform.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (float t = 0; t <= 1; t += 0.05f)
        {
            // Compute the position on the curve at time t
            // to display the path helper spheres
            gizmosPosition = Mathf.Pow(1 - t, 3) * source.transform.position +
                             3 * Mathf.Pow(1 - t, 2) * t * controlPoints[0].transform.position +
                             3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[1].transform.position +
                             Mathf.Pow(t, 3) * destination.transform.position;

            Gizmos.DrawSphere(gizmosPosition, 0.15f);
        }

        Gizmos.DrawLine(source.transform.position, controlPoints[0].transform.position);
        Gizmos.DrawLine(controlPoints[1].transform.position, destination.transform.position);
    }
}