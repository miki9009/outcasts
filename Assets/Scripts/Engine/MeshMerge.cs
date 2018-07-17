using UnityEngine;
using System.Collections;
namespace Engine
{
    public class MeshMerge : MonoBehaviour
    {
        public MeshFilter[] meshFilters;
        public Transform parent;
        Vector3 pos;

        public void Merge()
        {
            if(parent != null)
            {
                meshFilters = parent.GetComponentsInChildren<MeshFilter>();
            }
            transform.position = Vector3.zero;
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            pos = Vector3.zero;
            while (i < meshFilters.Length)
            {
                pos += meshFilters[i].transform.position;
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].mesh.uv = meshFilters[i].sharedMesh.uv;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            
            var obj = new GameObject("Combined_"+meshFilters[0].name);
            obj.transform.position = pos/meshFilters.Length;

            var meshRenderer = obj.AddComponent<MeshRenderer>();
            var filter = obj.AddComponent<MeshFilter>();
            filter.sharedMesh = new Mesh();
            filter.sharedMesh.CombineMeshes(combine);
#if UNITY_EDITOR
            UnityEditor.Unwrapping.GenerateSecondaryUVSet(filter.sharedMesh);
#endif
            meshRenderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}