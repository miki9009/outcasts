using UnityEngine;
using UnityEditor;

namespace Engine
{
   
    [CustomEditor(typeof(Level))]
    public class LevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var script = (Level)target;

            if (GUILayout.Button("Save"))
            {
                Level.Save(script.levelName);
            }

            if (GUILayout.Button("Load"))
            {
                Level.Load(script.levelName);
            }

            if (GUILayout.Button("Clear"))
            {
                script.Clear();
            }


        }
    }
}