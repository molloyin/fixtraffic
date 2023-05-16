using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TrafficLights : MonoBehaviour
{
    public TrafficController controller;
    [NonSerialized] private Transform[] lights;
    private int currentGreenWaypoint = -1;
    public int lightsInterval = 5;
    public int lightsDelay = 1;
    private float nextUpdate = 0;
    private bool isDelayUpdate = false;

    public static GameObject Add(TrafficController controller)
    {
        GameObject obj = new GameObject("Traffic Lights " + controller.trafficLights.Count + 1);
        TrafficLights tlComponent = obj.AddComponent<TrafficLights>();
        tlComponent.controller = controller;

        Vector3 mid = new Vector3();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            Waypoint waypoint = gameObject.GetComponent<Waypoint>();
            if (waypoint != null)
                mid += waypoint.transform.position;
        }

        mid /= Selection.gameObjects.Length;

        obj.transform.position = mid;
        obj.transform.parent = controller.transform;

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
        
        controller.trafficLights.Add(tlComponent);

        return obj;
    }

    public void GroupWaypoints()
    {
        GameObject obj = new GameObject("Group");
        obj.transform.parent = transform;
        obj.AddComponent<GroupLights>();

        Vector3 mid = new Vector3();
        foreach (GameObject waypointObj in Selection.gameObjects)
        {
            Waypoint waypoint = waypointObj.GetComponent<Waypoint>();
            if (waypoint != null)
                mid += waypoint.transform.position;
        }

        mid /= Selection.gameObjects.Length;
        obj.transform.position = mid;
        foreach (GameObject waypointObj in Selection.gameObjects)
            waypointObj.transform.parent = obj.transform;
    }

    public void LoadWaypoints()
    {
        lights = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            lights[i] = child;
            i++;
        }
    }

    public void Delete()
    {
        LoadWaypoints();
        foreach (var lightChild in lights)
        {
            if (lightChild.GetComponent<Waypoint>() == null)
            {
                foreach (Transform waypoint in lightChild)
                    waypoint.parent = controller.transform;
                DestroyImmediate(lightChild.gameObject);
            }
            else
            {
                lightChild.transform.parent = controller.transform;
            }
        }

        DestroyImmediate(transform.gameObject);
    }

    public void OnDestroy()
    {
        controller.trafficLights.Remove(this);
        for (int i = 0; i < controller.trafficLights.Count; i++)
            controller.trafficLights[i].name = "Traffic Lights " + (i + 1);
    }

    public void Start()
    {
        LoadWaypoints();
    }

    private void FixedUpdate()
    {
        if (Time.time >= nextUpdate && lights.Length > 0)
        {
            Waypoint waypoint;

            if (currentGreenWaypoint > -1)
            {
                Transform previousGreen = lights[currentGreenWaypoint];
                waypoint = previousGreen.GetComponent<Waypoint>();
                if (waypoint != null)
                {
                    waypoint.isStopped = true;
                    waypoint.currentSpeedLimit = 0;
                }
                else
                {
                    foreach (Transform child in previousGreen)
                    {
                        waypoint = child.GetComponent<Waypoint>();
                        waypoint.isStopped = true;
                        waypoint.currentSpeedLimit = 0;
                    }
                }
            }

            if (!isDelayUpdate && lightsDelay > 0)
            {
                isDelayUpdate = true;
                nextUpdate = Mathf.FloorToInt(Time.time) + lightsDelay;
                return;
            }

            isDelayUpdate = false;
            
            currentGreenWaypoint = (currentGreenWaypoint + 1) % lights.Length;

            Transform newGreen = lights[currentGreenWaypoint];
            waypoint = newGreen.GetComponent<Waypoint>();
            if (waypoint != null)
            {
                waypoint.isStopped = false;
                waypoint.currentSpeedLimit = waypoint.speedLimit;
            }
            else
            {
                foreach (Transform child in newGreen.transform)
                {
                    waypoint = child.GetComponent<Waypoint>();
                    waypoint.isStopped = false;
                    waypoint.currentSpeedLimit = waypoint.speedLimit;
                }

                waypoint = null;
            }
            
            nextUpdate = Mathf.FloorToInt(Time.time) + (waypoint == null
                ? newGreen.GetComponent<GroupLights>().greenDuration
                : lightsInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>() != null)
            {
                Gizmos.DrawSphere(child.transform.position, 1f);
            }
        }

        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}