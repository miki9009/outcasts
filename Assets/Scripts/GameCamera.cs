using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System.Linq;
using UnityStandardAssets.ImageEffects;

public class GameCamera : MonoBehaviour
{
    public Transform target;
    public Camera mainCamera;
    public float x;
    public float y;
    public float z;
    public float forwardFactor = 5;
    public float rotationSpeed;
    public float upFactor;
    public float speed;
    public bool rotateAround;
    public Transform[] camAnchors;
    public float maxDistance = 20;
    

    public Component vignatteAberration;
    public MotionBlur motionBlure;
    public CameraMotionBlur camMotionBlur;
    public bool move = true;
    
    public float minDistance = 5;

    public float UpFactorAtStart { get; private set; }
    public bool regularUpdate;
    Controller.GameType gameType;
    TriggerBroadcast triggerBroadcast;
    Collision lastCollision;
    System.Action Body;
    float time = 0;
    public bool collides;
    Collider col;
	void Start ()
    {
        gameType = Controller.Instance.gameType;
        UpFactorAtStart = upFactor;
        try
        {
            target = Controller.Instance.character.transform;
        }
        catch
        {
            Debug.Log("GameCamera target is null");
            enabled = false;
        }
        magnitude = new Vector3(x, y, z).magnitude;
        triggerBroadcast = GetComponentInChildren<TriggerBroadcast>();
        triggerBroadcast.TriggerEntered += (x) => { collides = true; };
        triggerBroadcast.TriggerExit += (x) => collides = false;
        Body = MainUpdate;
        GameManager.LevelClear += ResetCamera;
    }

    Quaternion slerp;
    float magnitude;

    private void OnDestroy()
    {
        GameManager.LevelClear -= ResetCamera;
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetView();
        }
#endif
        if(regularUpdate)
            Body();
    }


    bool isResetting;
    public void ResetView()
    {
        transform.position = target.position - target.forward * minDistance + Vector3.up * y;
        transform.rotation = Quaternion.LookRotation(Vector.Direction(transform.position, target.position + Vector3.up * upFactor));
    }

    void ResetCamera()
    {
        Body = MainUpdate;
    }


    private void FixedUpdate()
    {
        if (!regularUpdate)
            Body();
        //CheckFreePosition();
    }

    void MainUpdate()
    {
        if (target == null) return;
        Vector3 pos = transform.position;
        pos.y = target.position.y;
        Vector3 dir = Vector.Direction(target.position, pos);
        pos = target.position + dir * minDistance;
        pos.y += y;
        transform.position = pos;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector.Direction(transform.position, target.position + Vector3.up * upFactor)), rotationSpeed * Time.deltaTime);
    }

    void WagonUpdate()
    {
        if (target == null) return;
        Vector3 pos = transform.position;
        pos.y = target.position.y;
        Vector3 dir = Vector.Direction(target.position, pos);
        pos = target.position + dir * minDistance;
        pos.y += y;
        transform.position = target.position - target.forward * minDistance + Vector3.up * y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector.Direction(transform.position, target.position + Vector3.up * upFactor)), rotationSpeed * Time.deltaTime);
    }

    float freeTime; 
    void CheckFreePosition()
    {
        if (collides && Vector3.Distance(mainCamera.transform.position, target.position) > 5)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position - target.forward * 3 + target.up * 2, Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, target.rotation, Time.deltaTime);
            freeTime = 0.5f;
        }
        else
        {
            if (freeTime <= 0)
            {
                mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, Vector3.zero, Time.deltaTime);
                mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, Quaternion.identity, Time.deltaTime);
            }
            else
                freeTime -= Time.deltaTime;
        }

    }

    public void Stay(bool move)
    {
        this.move = move;
    }

    public void Stay(bool move, Vector3 pos)
    {
        this.move = move;
        transform.position = pos;
    }

    public Transform lastCollideObject;
    RaycastHit hit;
    public LayerMask collisionLayer;

    public void SetTarget(Transform target)
    {
        if (target == null)
        {
            enabled = false;
            this.target = null;
            return;
        }
        else if (!enabled)
            enabled = true;

        this.target = target;
        enabled = true;
        gameObject.SetActive(true);
        transform.position = target.position + target.forward * -10;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time">How long lasts</param>
    /// <param name="force"> maximum force</param>
    /// <param name="amplitude"> Range 0 - 1 position change per frame</param>
    public void Shake(float time = 1, float force = 10, float amplitude = 0.1f)
    {
        if(shakeCor == null)
            shakeCor = StartCoroutine(ShakeCor(time,force, amplitude));
    }

    Coroutine shakeCor;
    IEnumerator ShakeCor(float time, float force, float amplitude)
    {
        float x, y, z;
        while (time > 0)
        {
            x = Random.Range(-force, force);
            y = Random.Range(-force, force);
            z = Random.Range(-force, force);
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(x, y, z), amplitude);
            time -= Time.deltaTime;
            yield return null;
        }
        shakeCor = null;
    }

    public void ChangeToWagonCamera()
    {
        Body = WagonUpdate;
    }

    private void OnDrawGizmos()
    {
        if(target!=null)
            Gizmos.DrawLine(target.position +Vector3.up, target.position + Vector3.up + Vector.Direction(target.position, transform.position) * minDistance);
    }



}
