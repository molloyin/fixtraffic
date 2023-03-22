using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public int id;
    public int from = 0;
    public int to = 0;
    public TrafficManager manager;
    public Waypoint current;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = manager.GetWaypoint(from).transform.position;
        current = manager.GetWaypoint(from);
    }

    private Waypoint FindNextWaypoint()
    {
        if (current.successors.Count == 0) return null;
        if (current.successors.Count == 1) return manager.GetWaypoint(current.successors[0]);

        bool[] marks = new bool [manager.CountWaypoints()];
        Stack<int> stack = new Stack<int>();
        stack.Push(current.id);
        int next = -1;
        bool founded = false;

        while (stack.Count > 0 && !founded)
        {
            int visiting = stack.Pop();

            if (current.successors.Contains(visiting))
                next = visiting;

            if (visiting == to)
                founded = true;
            else
            {
                foreach (var successor in manager.GetWaypoint(visiting).successors)
                {
                    int successorIndex = manager.GetWaypointIndex(successor);
                    if (!marks[successorIndex])
                    {
                        stack.Push(successor);
                    }
                }
            }
        }

        return manager.GetWaypoint(next);
    }

    // Update is called once per frame
    void Update()
    {
        if (current != null)
        {
            if (Vector3.Distance(transform.position, current.transform.position) < 1 && current.successors.Count > 0)
            {
                current = current.id == to ? null : FindNextWaypoint();
            }

            if (current != null)
            {
                transform.LookAt(current.transform);
                transform.Translate(0, 0, current.speed * Time.deltaTime);
            }
        }
    }

    public void OnDestroy()
    {
        manager.RemoveVehicle(this);
    }
}