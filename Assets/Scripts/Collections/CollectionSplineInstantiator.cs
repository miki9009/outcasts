using Engine;
using UnityEngine;

public class CollectionSplineInstantiator : LevelElement
{
    public BezierCurve curve;
    public Transform[] points;
    public int amount;
    [SpawnsSelector]
    public string spawnName;


    Transform[] elements;
    Float3[] pointsPos;
    Float4[] pointsRot;
    Float3[] handle1Pos;
    Float3[] handle2Pos;


    private void Awake()
    {
        curve = GetComponentInChildren<BezierCurve>();
    }

    public override void ElementStart()
    {
        base.ElementStart();
        var spawn = (GameObject)Resources.Load(PrefabsPath + spawnName, typeof(GameObject));
        if (spawn != null)
        {
            float splineFactor = 1f / amount;
            elements = new Transform[amount];
            for (int i = 0; i < amount; i++)
            {
                elements[i] = Instantiate(spawn, transform).GetComponent<Transform>();
                elements[i].transform.position = curve.GetPointAt(i * splineFactor);
                elements[i].transform.localScale = Vector3.zero;
                elements[i].gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Spawn was null: " + spawn);
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        int length = points.Length;
        pointsPos = new Float3[length];
        pointsRot = new Float4[length];
        handle1Pos = new Float3[length];
        handle2Pos = new Float3[length];
        for (int i = 0; i < length; i++)
        {
            pointsPos[i] = points[i].localPosition;
            pointsRot[i] = points[i].localRotation;
            var handle = points[i].GetComponent<BezierPoint>();
            handle1Pos[i] = handle.handle1;
            handle2Pos[i] = handle.handle2;
        }
        if (data != null)
        {
            data["Points"] = pointsPos;
            data["Rotations"] = pointsRot;
            data["Handle1"] = handle1Pos;
            data["Handle2"] = handle2Pos;
            data["Spawn"] = spawnName;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data.ContainsKey("Points"))
            pointsPos = (Float3[])data["Points"];
        if (data.ContainsKey("Rotations"))
            pointsRot = (Float4[])data["Rotations"];
        if (data.ContainsKey("Handle1"))
            handle1Pos = (Float3[])data["Handle1"];
        if (data.ContainsKey("Handle2"))
            handle2Pos = (Float3[])data["Handle2"];
        if (data.ContainsKey("Spawn"))
            spawnName = (string)data["Spawn"];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (curve == null) return;
        for (int i = 0; i < amount; i++)
        {
            float factor = 1f / amount;
            Gizmos.DrawSphere(curve.GetPointAt(factor * i), 1);
        }
    }
#endif
}