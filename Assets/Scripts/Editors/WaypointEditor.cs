using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(Waypoint))]
    public class WaypointEditor : Editor
    {
        private Waypoint waypoint;
        private SerializedProperty pathType;
        private SerializedProperty showPaths;

        private void OnEnable()
        {
            waypoint = (Waypoint) target;
            showPaths = serializedObject.FindProperty("showPaths");
            pathType = serializedObject.FindProperty("pathType");
        }

        public override void OnInspectorGUI()
        {
            int previousTypeIndex = pathType.enumValueIndex;
            bool previousShowPaths = showPaths.boolValue;
            base.OnInspectorGUI();
            serializedObject.Update();

            if (previousTypeIndex != pathType.enumValueIndex) waypoint.UpdatePathType();
            if (previousShowPaths != showPaths.boolValue) waypoint.TogglePaths();

            serializedObject.ApplyModifiedProperties();
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
                            waypoint.Selected = true;
                            SceneView.RepaintAll();
                        }
                        else
                        {
                            waypoint.Selected = false;
                            SceneView.RepaintAll();
                        }
                    }
                    else if (current.type is EventType.MouseDown && current.button == 0)
                    {
                        waypoint.controller.AddWaypoint(waypoint);
                    }
                }
                else if (current.type is EventType.MouseUp && current.button == 0)
                {
                    waypoint.Matrix.UpdateWeights(waypoint);
                    waypoint.Merge();
                }
                else
                {
                    waypoint.Selected = false;
                }
            }
            else
            {
                waypoint.Selected = false;
            }
        }
    }
}