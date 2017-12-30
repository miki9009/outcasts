﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [CustomEditor(typeof(DataManager))]
    public class GameInitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (DataManager)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Save current data"))
            {
                DataManager.Instance.SaveData();
            }
            if (GUILayout.Button("Load data"))
            {
                DataManager.Instance.LoadData();
            }
        }
    }
}
