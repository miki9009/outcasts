using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Materials))]
public class MaterialsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (GUILayout.Button("Get All materials on scene"))
        {
            HashSet<Material> materials = new HashSet<Material>();
            var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].materials.Length > 1)
                {
                    Debug.Log(renderers[i].name + "has more than one material!");
                }
                materials.Add(renderers[i].material);
            }
            var script = (Materials)target;
            script.materials = materials.ToArray();
        }
    }
}
