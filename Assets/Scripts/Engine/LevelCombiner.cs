using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Engine;

public class LevelCombiner : MonoBehaviour
{
    public bool useLevelCombiner;
    public MeshMerge meshCombiner;
    public List<MeshRenderer> meshes;
    public float chunkSizeX = 10;
    public float chunkSizeZ = 10;
    public float maxX = 200;
    public float maxZ = 200;

    public Material[] materials;

    private void Start()
    {
        if (!useLevelCombiner) return;
        Execute();
    }
    public void Execute()
    {
        if(meshes == null || meshes.Count == 0)
        {
            Debug.Log("No meshes selected");
            return;
        }
        materials = GetMaterials();
    }

    public void GetAllMeshes()
    {
        meshes = FindObjectsOfType<MeshRenderer>().ToList();
    }

    Material[] GetMaterials()
    {
        Dictionary<int, Material> materialsDictionary = new Dictionary<int, Material>();

        for (int i = 0; i < meshes.Count; i++)
        {
            var mat = meshes[i].sharedMaterial;
            if (!materialsDictionary.ContainsKey(mat.GetHashCode()))
                materialsDictionary.Add(mat.GetHashCode(), mat);
        }

        var mats = new Material[materialsDictionary.Count];
        int index = 0;
        foreach(var mat in materialsDictionary.Values)
        {
            mats[index] = mat;
            index++;
        }
        return mats;
    }

    public void CreateChunks()
    {
        //var chunks = new 
    }

    struct Chunk
    {
        public float maxX;
        public float maxZ;
        public float minX;
        public float minZ;
    }

#if UNITY_EDITOR
    public bool drawGrid;
    public float y = 0;
    public float gizmoHeight = 5;
    private void OnDrawGizmos()
    {
        Vector3 startPos = transform.position;
        int length = (int)(maxZ / chunkSizeZ);
        int longLength = (int) (maxX / chunkSizeX);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < longLength; j++)
            {
                Gizmos.DrawWireCube(startPos + new Vector3(i * chunkSizeX - chunkSizeX / 2, y, j * chunkSizeX - chunkSizeX / 2), new Vector3(chunkSizeX, gizmoHeight, chunkSizeZ));
            }
        }
    }
#endif


}