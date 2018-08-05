﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpot : MonoBehaviour
{
    private Transform target;
    private Enemy enemy;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        target = other.attachedRigidbody.GetComponentInParent<Character>().transform;
         enemy.target = target;
        Debug.Log("Target: " + target);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (target == null) return;
    //    if (other.attachedRigidbody.transform == target)
    //    {
    //        target = null;
    //        enemy.target = null;
    //    }
    //}
}
