using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public List<Waypoint> waypoints = new();
    public List<Vehicle> vehicles = new();

    public GameObject AddWaypoint(Waypoint _from = null)
    {
        if (_from == null && waypoints.Count > 0) return null;

        int id = waypoints.Count == 0 ? 0 : waypoints[^1].id + 1;

        GameObject obj = new GameObject("Waypoint " + id);
        Waypoint waypoint = obj.AddComponent<Waypoint>();
        waypoint.manager = this;
        obj.transform.parent = transform;

        if (_from == null)
        {
            obj.transform.position = transform.position;
            waypoint.id = id;
        }
        else
        {
            waypoint.id = id;
            _from.successors.Add(waypoint.id);
            waypoint.predecessors.Add(_from.id);
            obj.transform.position = _from.mousePos;
        }

        waypoints.Add(waypoint);
        return obj;
    }

    public List<Waypoint> GetSuccessors(Waypoint _waypoint)
        => waypoints.FindAll(_w => _waypoint.successors.Contains(_w.id));

    public List<Waypoint> GetPredecessors(Waypoint _waypoint)
        => waypoints.FindAll(_w => _waypoint.predecessors.Contains(_w.id));

    public void RemoveWaypoint(Waypoint _waypoint)
    {
        foreach (var successor in GetSuccessors(_waypoint))
            successor.predecessors.Remove(_waypoint.id);

        foreach (var predecessor in GetPredecessors(_waypoint))
            predecessor.successors.Remove(_waypoint.id);

        waypoints.Remove(_waypoint);
    }

    public void MergeWaypoint(Waypoint _from)
    {
        int i = 0;
        while (i < waypoints.Count && i > -1)
        {
            Waypoint to = waypoints[i];
            if (Vector3.Distance(_from.transform.position, to.transform.position) < 1 &&
                _from.id != to.id)
            {
                i = -1;
                foreach (var successor in GetSuccessors(_from)
                             .Where(_successor => !to.successors.Contains(_successor.id) && to.id != _successor.id))
                {
                    to.successors.Add(successor.id);
                    successor.predecessors.Add(to.id);
                }

                foreach (var predecessor in GetPredecessors(_from)
                             .Where(_predecessor =>
                                 !to.predecessors.Contains(_predecessor.id) && _predecessor.id != to.id))
                {
                    to.predecessors.Add(predecessor.id);
                    predecessor.successors.Add(to.id);
                }

                DestroyImmediate(_from.transform.gameObject);
            }
            else
            {
                i++;
            }
        }
    }

    public Waypoint GetWaypoint(int _id)
    {
        return waypoints.Find(_w => _w.id == _id);
    }

    public int GetWaypointIndex(int _id)
    {
        return waypoints.FindIndex(_w => _w.id == _id);
    } 

    public int CountWaypoints()
    {
        return waypoints.Count;
    }

    public GameObject AddVehicle()
    {
        int id = vehicles.Count + 1;
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "Vehicle " + id;
        obj.transform.position = transform.position;
        Vehicle vehicle = obj.AddComponent<Vehicle>();
        vehicle.id = id;
        obj.transform.parent = transform;
        vehicle.manager = this;
        vehicles.Add(vehicle);

        return obj;
    }

    public void RemoveVehicle(Vehicle _vehicle)
    {
        vehicles.Remove(_vehicle);
    }
}