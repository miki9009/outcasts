﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class GameCamera : MonoBehaviour
{
    public Transform target;
    public bool fixedUpdate;
    public float x;
    public float y;
    public float z;
    public float rotationSpeed;
    public float upFactor;
    public float speed;

    public Component vignatteAberration;
    public bool move = true;
    public Vector3 stayPos;

    float time = 0;

	void Start ()
    {
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
        if (move)
        {
            if (fixedUpdate && target)
            {
                collide = CheckFreePosition();
                if (collide)
                {
                    time = time > 0 ? time - Time.deltaTime : 0;
                    transform.position = Vector3.Lerp(transform.position, target.position + target.forward * z + Vector3.up * y + target.right * x, (time > 0 ?  speed * (2 -time) : speed) * Time.deltaTime);
                    slerp = Quaternion.LookRotation(Math.DirectionVector(transform.position, target.position + Vector3.up * upFactor));
                    transform.rotation = Quaternion.Slerp(transform.rotation, slerp, rotationSpeed * Time.deltaTime);
                }
                else
                {
                    time = 2;
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
