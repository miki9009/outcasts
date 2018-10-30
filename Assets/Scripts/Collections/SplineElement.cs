using UnityEngine;
using Engine;

public class SplineElement : LevelElement
{
    public string prefabsPath = "LevelElements/";
    public BezierCurve curve;
    public float speed;
    public bool rotate;
    public Transform[] points;
    [SpawnsSelector]
    public string[] spawns;
    public bool moving;

    [Range(0, 0.5f)]
    public float splineFactor;


    Vector3 prevPos;
    float calculatedSpeed;
    float calculatedRotationSpeed;
    Vector3 lastPos;
    float dis;
    Vector3 dir;
    Transform[] elements;

    private void Awake()
    {
        curve = GetComponentInChildren<BezierCurve>();
    }

    Float3[] pointsPos;
    Float4[] pointsRot;
    Float3[] handle1Pos;
    Float3[] handle2Pos;

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
            data["Factor"] = splineFactor;
            data["Spawns"] = spawns;
            data["Moving"] = moving;
        }
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if (data != null)
        {
            if (data.ContainsKey("Points"))
                pointsPos = (Float3[])data["Points"];
            if (data.ContainsKey("Rotations"))
                pointsRot = (Float4[])data["Rotations"];
            if (data.ContainsKey("Handle1"))
                handle1Pos = (Float3[])data["Handle1"];
            if (data.ContainsKey("Handle2"))
                handle2Pos = (Float3[])data["Handle2"];
            if (data.ContainsKey("Factor"))
                splineFactor = (float)data["Factor"];
            if (data.ContainsKey("Spawns"))
                spawns = (string[])data["Spawns"];
            if (data.ContainsKey("Moving"))
                moving = (bool)data["Moving"];
            for (int i = 0; i < pointsPos.Length; i++)
            {
                points[i].transform.localPosition = pointsPos[i];
                points[i].transform.localRotation = pointsRot[i];
                var handle = points[i].GetComponent<BezierPoint>();
                handle.handle1 = handle1Pos[i];
                handle.handle2 = handle2Pos[i];
            }

            elements = new Transform[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                var spawn = (GameObject)Resources.Load(prefabsPath, typeof(GameObject));
                elements[i] = Instantiate(spawn, transform).GetComponent<Transform>();
                elements[i].localPosition = curve.GetPointAt(i * splineFactor);
            }

        }
    }


    private void OnDrawGizmos()
    {
        if (curve == null) return;
        for (int i = 0; i < spawns.Length; i++)
        {
            Gizmos.DrawSphere(curve.GetPointAt(splineFactor * i), 1);
        }
    }


}
