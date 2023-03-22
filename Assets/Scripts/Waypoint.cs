using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{
    public List<int> successors = new();
    public List<int> predecessors = new();
    public List<GameObject>[] pathToNext = new List<GameObject>[3]; //array of lists
    public Vector3 mousePos = new Vector3(0, 0, 0);
    public bool isSelected;
    public TrafficManager manager = null;
    public int id;
    public float speed = 10;

    //MAX CHANGES
    //Enum to track status of node
    private enum nodeType
    {
        ENTRY,
        EXIT,
        NORM
    };

    [SerializeField] private nodeType type = nodeType.NORM; //By default is set to NORM


    private void OnDestroy()
    {
        manager.RemoveWaypoint(this);
    }

    public static void DrawGizmoArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawCube(position, new Vector3(1, 1, 1));
        
        foreach (Waypoint successor in manager.GetSuccessors(this))
        {
            DrawGizmoArrow(position, successor.transform.position - position, Color.cyan, 1, 40);
        }
        
        if (isSelected)
        {
            Gizmos.DrawLine(position, mousePos);
        }
    }

    public List<Waypoint> GetSuccessorsWaypoints() {
        return this.manager.GetSuccessors(this);
    }
}