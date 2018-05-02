using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    BezierCurve curve;
    public Transform platform;
    public float speed;
    public bool rotate;
    public float rotationSpeed;

    [Range(0, 1f)]
    public float pos;
    Vector3 prevPos;
    float calculatedSpeed;
    float calculatedRotationSpeed;

    private void Awake()
    {
        curve = GetComponentInChildren<BezierCurve>();
        calculatedSpeed = speed * 0.0016f;
        prevPos = platform.position;
        calculatedRotationSpeed = rotationSpeed * 0.016f;
    }

    private void Update()
    {
        if (pos + calculatedSpeed >= 1) pos -= 1f;
            pos += calculatedSpeed;
        platform.position = Vector3.Lerp(platform.position, curve.GetPointAt(pos), Time.deltaTime * speed);
        if (rotate)
            platform.rotation = Quaternion.Lerp(platform.rotation, Quaternion.LookRotation(Engine.Vector.Direction(platform.position, curve.GetPointAt(Mathf.Clamp01(pos)))), calculatedRotationSpeed);

    }
}