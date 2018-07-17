using UnityEngine;

public class GameCameraSettings : MonoBehaviour
{
    public float x = 0;
    public float y = 5;
    public float z = -9;
    public float forwardFactor = 1.5f;
    public float rotationSpeed = 5;
    public float upFactor = 3;
    public float speed = 10;

    private void Start()
    {
        Apply();
    }

    private void Apply()
    {
        var cam = Controller.Instance.gameCamera.GetComponent<GameCamera>();
        cam.x = x;
        cam.y = y;
        cam.z = z;
        cam.forwardFactor = forwardFactor;
        cam.rotationSpeed = rotationSpeed;
        cam.upFactor = upFactor;
        cam.speed = speed;

    }
}