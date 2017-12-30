﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectionObject : MonoBehaviour
{
    public CollectionType type;
    public int val;
    public bool emmitParticles;
    public int particlesAmmount;
    public bool collected = false;

    public delegate void Collect(GameObject collector);
    public event Collect OnCollected;
    [HideInInspector] public Rigidbody rigid;

    public CollectionDisplay display;

    public void OnTriggerEnter(Collider other)
    {
        var obj = other.gameObject;
        if (obj.layer == 14)
        {
            if (collected) return;
            collected = true;
            if (OnCollected != null)
            {
                OnCollected(obj);
            }
            display.ShowDisplay();
            int playerID = other.GetComponentInParent<Character>().ID;
            CollectionManager.Instance.SetCollection(playerID, type, val);
            StartCoroutine(Collected());
            if (emmitParticles)
            {
                CollectionManager.Instance.EmmitParticles(type, transform.position + Vector3.up, particlesAmmount);
            }
        }
    }

    private void Awake()
    {
        if (!GameManager.LevelLoaded)
        {
            GameManager.OnLevelLoaded += AssignDisplayOnLoad;
        }
        else
        {
            display = CollectionDisplayManager.Instance.AssignDisplayObject(type);
        }
    }

    protected virtual void Start()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void AssignDisplayOnLoad()
    {
        try
        {
            display = CollectionDisplayManager.Instance.AssignDisplayObject(type);
        }
        catch(Exception ex)
        {
            enabled = false;
            Debug.Log(ex.Message);
        }
    }

    private void Update()
    {
        transform.rotation = rotation;
    }

    protected virtual IEnumerator Collected()
    {
        while(transform.localScale.x > 0.05)
        {
            transform.localScale /= 1.05f;
            yield return null;
        }

        yield return new WaitForSeconds(deactivationTime);
        Deactivate();
        yield return null;
    }

    public float deactivationTime = 0.1f;
    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public static Vector3 eulers = Vector3.zero;
    public static Quaternion rotation = Quaternion.identity;


}

public enum CollectionType
{
    Coin = 0,
    Emmerald = 1,
    Health = 2,
    Clock = 3,
    Magnet = 4,
    Weapon = 5
}

