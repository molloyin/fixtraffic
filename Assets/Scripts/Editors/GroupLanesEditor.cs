using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(GroupLanes))]
    public class GroupLanesEditor : Editor
    {
        private GroupLanes group;
        
        private void OnEnable()
        {
            group = (GroupLanes) target;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            
            GUIStyle button = new GUIStyle(GUI.skin.button)
            {
                normal =
                {
                    textColor = new Color(1f, 1f, 1f, 1f)
                },
                fixedHeight = 32f,
                padding = new RectOffset(0, 0, 0, 0)
            };
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Delete (while keeping waypoints)", button, GUILayout.Width(200f)))
                group.Delete();
            
        }
    }
}