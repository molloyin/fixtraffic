using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(TrafficController))]
    public class TrafficManagerEditor : Editor
    {
        private TrafficController controller;
        private SerializedProperty vehicles;
        private SerializedProperty matrix;
        private SerializedProperty simulationSeed;

        private void OnEnable()
        {
            controller = (TrafficController) target;
            vehicles = serializedObject.FindProperty("vehicles");
            matrix = serializedObject.FindProperty("matrix");
            simulationSeed = serializedObject.FindProperty("simulationSeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(vehicles);
            EditorGUILayout.PropertyField(matrix);
            EditorGUILayout.PropertyField(simulationSeed);
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
                Selection.activeObject = controller.AddWaypoint();
            }

            if (GUILayout.Button("Add vehicle", button, GUILayout.Width(128f)))
            {
                Selection.activeObject = controller.AddVehicle();
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}