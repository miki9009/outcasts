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
    public Transform[] camAnchors;

    public Component vignatteAberration;
    public MotionBlur motionBlure;
    public CameraMotionBlur camMotionBlur;
    public bool move = true;

    public float UpFactorAtStart { get; private set; }

    Controller.GameType gameType;

    float time = 0;

	void Start ()
    {
        gameType = Controller.Instance.gameType;
        try
        {
            target = Controller.Instance.character.transform;
            UpFactorAtStart = upFactor;
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
        if (move)
        {
            if (target)
            {
                if (gameType == Controller.GameType.Perspective)
                {
                    collide = CheckFreePosition();
                    if (collide)
                    {
                        time = time > 0 ? time - Time.deltaTime : 0;
                        transform.position = Vector3.Lerp(transform.position, target.position + target.forward * z + Vector3.up * y + target.right * x, (time > 0 ? speed * (2 - time) : speed) * Time.deltaTime);
                        slerp = Quaternion.LookRotation(Math.DirectionVector(transform.position, target.position + Vector3.up * upFactor));
                        transform.rotation = Quaternion.Slerp(transform.rotation, slerp, rotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        time = 2;
                        transform.rotation = Quaternion.Slerp(transform.rotation, slerp, rotationSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    time = time > 0 ? time - Time.deltaTime : 0;
                    transform.position = Vector3.Lerp(transform.position, target.position + target.forward * forwardFactor + new Vector3(x,y,z), (time > 0 ? speed * (2 - time) : speed) * Time.deltaTime);
                    slerp = Quaternion.LookRotation(Math.DirectionVector(transform.position, target.position + Vector3.up * upFactor + target.forward * forwardFactor));
                    transform.rotation = Quaternion.Slerp(transform.rotation, slerp, rotationSpeed * Time.deltaTime);
                }
            }           
        }
        else
        {
            transform.LookAt(target);
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

    }


}
