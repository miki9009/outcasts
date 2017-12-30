using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [CustomEditor(typeof(Button))]
    public class ButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (Button)target;

            DrawDefaultInspector();

        }
    }
}
