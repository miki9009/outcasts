using System.Collections;
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
        if (other.gameObject.layer == 9)
        {
            target = other.transform.root;
            enemy.target = target;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            if (target == null) return;
            if (other.transform.root == target)
            {
                target = null;
            }
        }
    }
}
