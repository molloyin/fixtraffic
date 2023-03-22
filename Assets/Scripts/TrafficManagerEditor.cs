using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrafficManager))]
public class TrafficManagerEditor : Editor
{
    private TrafficManager manager;
    private SerializedProperty waypoints;
    private SerializedProperty vehicles;

    private void OnEnable()
    {
        manager = (TrafficManager) target;
        waypoints = serializedObject.FindProperty("waypoints");
        vehicles = serializedObject.FindProperty("vehicles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(waypoints);
        EditorGUILayout.PropertyField(vehicles);
        GUIStyle button = new GUIStyle(GUI.skin.button)
        {
            normal =
            {
                textColor = new Color(1f, 1f, 1f, 1f)
            },
            fixedHeight = 32f,
            padding = new RectOffset(0, 0, 0, 0)
        };

        if (GUILayout.Button("Add waypoint", button, GUILayout.Width(128f)))
        {
            Selection.activeObject = manager.AddWaypoint();
        }
        
        if (GUILayout.Button("Add vehicle", button, GUILayout.Width(128f)))
        {
            Selection.activeObject = manager.AddVehicle();
        }
    }
}