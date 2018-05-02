using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    BezierCurve curve;
    public Transform platform;
    public float speed;
    public bool rotate;
    public float rotationSpeed;
    public TriggerBroadcast triggerBroadcast;
    public float lerpSpeed;

    [Range(0, 1f)]
    public float pos;
    Vector3 prevPos;
    float calculatedSpeed;
    float calculatedRotationSpeed;

    Dictionary<int, Transform> platformers;
    Vector3 lastPos;

    float dis;
    Vector3 dir;

    private void Awake()
    {
        platformers = new Dictionary<int, Transform>();
        curve = GetComponentInChildren<BezierCurve>();
        calculatedSpeed = speed * 0.0016f;
        prevPos = platform.position;
        calculatedRotationSpeed = rotationSpeed * 0.016f;
        lastPos = platform.position;
        triggerBroadcast.TriggerEntered += BroadCastTriggerEnter;
        triggerBroadcast.TriggerExit += BroadCastTriggerExit;
    }

    void BroadCastTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.layer == Layers.Character)
        {
            var transform = other.transform.root;
            transform.parent = platform;
            platformers.Add(other.GetInstanceID(), transform);
        }
    }

    void BroadCastTriggerExit(Collider other)
    {
        Debug.Log("Moving platform TriggerExit");
        if (platformers.ContainsKey(other.GetInstanceID()))
        {
            int id = other.GetInstanceID();
            var transform = platformers[id];
            transform.parent = null;
            platformers.Remove(id);
        }
    }

    private void FixedUpdate()
    {
        if (pos + calculatedSpeed >= 1) pos -= 1f;
            pos += calculatedSpeed;
        platform.position = Vector3.Lerp(platform.position, curve.GetPointAt(pos), Time.deltaTime * speed);
        if (rotate)
            platform.rotation = Quaternion.Lerp(platform.rotation, Quaternion.LookRotation(Engine.Vector.Direction(platform.position, curve.GetPointAt(Mathf.Clamp01(pos)))), calculatedRotationSpeed);
    }


}