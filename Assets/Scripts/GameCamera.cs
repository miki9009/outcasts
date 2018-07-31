using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System.Linq;
using UnityStandardAssets.ImageEffects;

public class GameCamera : MonoBehaviour
{
    public Transform target;
    public float x;
    public float y;
    public float z;
    public float forwardFactor = 5;
    public float rotationSpeed;
    public float upFactor;
    public float speed;
    public bool rotateAround;
    public Transform[] camAnchors;
    

    public Component vignatteAberration;
    public MotionBlur motionBlure;
    public CameraMotionBlur camMotionBlur;
    public bool move = true;

    public float minDistance = 5;

    public float UpFactorAtStart { get; private set; }

    Controller.GameType gameType;

    float time = 0;

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
	}

    Quaternion slerp;
    float magnitude;
    private void FixedUpdate()
    {
        if (target == null) return;
        Vector3 pos = transform.position;
        pos.y = target.position.y;
        Vector3 dir = Vector.Direction(target.position, pos);
        pos = target.position + dir * minDistance;
        pos.y += y;
        transform.position = pos;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector.Direction(transform.position, target.position + Vector3.up * upFactor)), rotationSpeed * Time.deltaTime);
        //transform.LookAt(target.position + Vector3.up * upFactor);
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
    public bool collide;
    bool CheckFreePosition()
    {
        Vector3 dir = target.forward * z + Vector3.up * y + target.right * x;
        Debug.DrawLine(target.position + Vector3.up * 2, target.position + dir, Color.red);
        if (Physics.SphereCast(target.position + Vector3.up * 2, 0.5f, dir.normalized, out hit, dir.magnitude, collisionLayer.value, QueryTriggerInteraction.Ignore))
        {
            lastCollideObject = hit.transform;
            Vector3 pos = hit.point - dir.normalized;
            if (pos.y < target.position.y + 2)
            {
                pos.y = target.position.y + 2;
            }
            slerp = Quaternion.LookRotation(Math.DirectionVector(transform.position, target.position + target.forward * 10));
            transform.position = Vector3.Slerp(transform.position, pos, speed * Time.deltaTime);
            return false;
        }
        return true;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        enabled = true;
        gameObject.SetActive(true);
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

}
