using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroupLanes : MonoBehaviour
{
    public TrafficController controller;
    private int LanesCount => transform.childCount;
    private List<MobileObject>[] Vehicles;

    private void Start()
    {
        Vehicles = new List<MobileObject>[LanesCount];
    }

    public static GameObject GroupWaypoints(TrafficController _controller)
    {
        GameObject obj = new GameObject("Lanes Group");
        GroupLanes tlComponent = obj.AddComponent<GroupLanes>();
        tlComponent.controller = _controller;

        Vector3 mid = new Vector3();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            Waypoint waypoint = gameObject.GetComponent<Waypoint>();
            if (waypoint != null)
                mid += waypoint.transform.position;
        }

        mid /= Selection.gameObjects.Length;

        obj.transform.position = mid;
        obj.transform.parent = _controller.transform;

        bool destroy = false;

        foreach (GameObject gameObject in Selection.gameObjects)
        {
            Waypoint waypoint = gameObject.GetComponent<Waypoint>();
            if (waypoint != null && !destroy)
                waypoint.transform.parent = tlComponent.transform;
            else
                destroy = true;
        }

        if (destroy)
        {
            DestroyImmediate(obj);
            return null;
        }

        return obj;
    }

    private void OnEnable()
    {
        Vehicles = new List<MobileObject>[LanesCount];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>() != null)
                Gizmos.DrawSphere(child.transform.position, 1f);
        }

        Gizmos.DrawWireCube(transform.position, new Vector3(8, 8, 8));
    }

    public void Delete()
    {
        Waypoint[] waypoints = GetWaypoints;
        foreach (Waypoint waypoint in waypoints)
            waypoint.transform.parent = controller.transform;

        DestroyImmediate(transform.gameObject);
    }

    private Waypoint[] GetWaypoints => GetComponentsInChildren<Waypoint>();

    public int GetCurrentLane(Waypoint _waypoint)
    {
        int i = 0;
        while (i < LanesCount && transform.GetChild(i).GetComponent<Waypoint>() != _waypoint)
            i++;

        return i;
    }

    public Waypoint GetSiblingWaypoint(MobileObject _vehicle, bool _passCar)
    {
        if ((_passCar && _vehicle.currentLane == LanesCount - 1) || (!_passCar && _vehicle.currentLane == 0))
            return null;
        return transform.GetChild(_vehicle.currentLane + (_passCar ? 1 : -1)).GetComponent<Waypoint>();
    }

    public Waypoint GetWaypoint(MobileObject _vehicle)
    => transform.GetChild(_vehicle.currentLane).GetComponent<Waypoint>();

    public bool CanDownLane(MobileObject _vehicle)
    {
        if (_vehicle.currentLane == 0)
            return false;

        List<MobileObject> vehicles = Vehicles[_vehicle.currentLane - 1];
        if (vehicles == null || vehicles.Count == 0)
            return true;

        int i;
        for (i = 0; i < vehicles.Count && _vehicle.speed <= vehicles[i].speed; i++) ;
        
        return i == vehicles.Count;
    }

    public void AddVehicle(MobileObject _vehicle)
    {
        Vehicles[_vehicle.currentLane] ??= new List<MobileObject>();
        Vehicles[_vehicle.currentLane].Add(_vehicle);
    }

    public void RemoveVehicle(MobileObject _vehicle) => Vehicles[_vehicle.currentLane].Remove(_vehicle);
}