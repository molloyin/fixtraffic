using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    private Waypoint waypoint;

    private void OnEnable()
    {
        waypoint = (Waypoint) target;
    }

    public void OnSceneGUI()
    {
        Event current = Event.current;

        if (Selection.activeGameObject == waypoint.transform.gameObject && Selection.objects.Length == 1)
        {
            if (current.shift)
            {
                if (current.type is EventType.MouseMove or EventType.MouseDrag)
                {
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (Physics.Raycast(worldRay, out var hitInfo))
                    {
                        waypoint.mousePos = hitInfo.point;
                        waypoint.isSelected = true;
                        SceneView.RepaintAll();
                    }
                    else
                    {
                        waypoint.isSelected = false;
                        SceneView.RepaintAll();
                    }
                }
                else if (current.type is EventType.MouseDown && current.button == 0)
                {
                    waypoint.manager.AddWaypoint(waypoint);
                }
            }
            else if (current.type is EventType.MouseUp && current.button == 0)
            {
                waypoint.manager.MergeWaypoint(waypoint);
            }
            else
            {
                waypoint.isSelected = false;
            }
        }
        else
        {
            waypoint.isSelected = false;
        }
    }
}