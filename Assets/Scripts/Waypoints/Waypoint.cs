﻿using System.Collections;
using UnityEngine;
using Engine;
using System.Collections.Generic;

public class Waypoint : LevelElement
{
    public int index = 0;
    public float height = 1;
    public float fadeDuration = 1;
    public bool active;

    Vector3 startScale;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        startScale = transform.localScale;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public event System.Action<CharacterMovement> Visited;

    private void Start()
    {
        transform.localScale = new Vector3(startScale.x, 0.1f, startScale.z);
        if (arrowTarget)
        {
            ArrowActivator.Enable(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponentInParent<Character>();
        Visited?.Invoke(character.movement);
        if (active && WaypointManager.Instance.currentWaypoint == this)
        {

            if(character.movement.IsLocalPlayer)
                StartCoroutine(Shrink());

            CollectionManager.Instance.OnCollected(character.ID, CollectionType.WaypointVisited,1);
        }
    }

    private void Initialize()
    {
        WaypointManager.Instance.waypoints.Add(index, this);
        transform.SetParent(WaypointManager.Instance.transform);
        if (index == 0)
        {
            WaypointManager.Instance.currentWaypoint = this;
            StartCoroutine(Expand());
        }
    }


    IEnumerator Expand()
    {
        if (!Application.isPlaying) yield break;
        meshRenderer.enabled = true;
        active = true;
        float duration = 0;
        Easer ease = AutoEase.QuadIn;
        while (duration < 1)
        {
            duration += Time.deltaTime / fadeDuration;
            transform.localScale = new Vector3(startScale.x, Mathf.Lerp(0,height,ease(duration)), startScale.z);
            yield return null; 
        }
        if (arrowTarget)
        {
            ArrowActivator.Enable(true);
        }
    }

    IEnumerator Shrink()
    {
        if (arrowTarget)
        {
            ArrowActivator.Enable(false);
        }
        active = false;
        Waypoint way;
        if(WaypointManager.Instance.waypoints.TryGetValue(index+1, out way))
        {
            WaypointManager.Instance.currentWaypoint = way;
            way.StartCoroutine(way.Expand());
        }
        else
        {
            Debug.Log("No more wayppoints");
        }
        transform.localScale = new Vector3(startScale.x, 0, startScale.z);
        float duration = 0;
        Easer ease = AutoEase.QuadOut;
        while (duration < 1)
        {
            duration += Time.deltaTime / fadeDuration;
            transform.localScale = new Vector3(startScale.x, Mathf.Lerp(height, 0, ease(duration)), startScale.z);
            yield return null;
        }
        meshRenderer.enabled = false;
    }

    public override void OnLoad()
    {
        base.OnLoad();
        if(data!=null)
        {
            object obj;
            if (data.TryGetValue("Index", out obj))
            {
                index = (int)obj;
            }
            if (data.TryGetValue("Height", out obj))
            {
                height = (float)obj;
            }
            if (data.TryGetValue("Fade", out obj))
            {
                fadeDuration = (float)obj;
            }

            Initialize();
        }
    }

    public override void OnSave()
    {
        base.OnSave();
        if (data != null)
        {
            data["Index"] = index;
            data["Height"] = height;
            data["Fade"] = fadeDuration;
        }
    }

    private void OnDestroy()
    {
        if(WaypointManager.Instance!= null && WaypointManager.Instance.waypoints.ContainsKey(index))
            WaypointManager.Instance.waypoints.Remove(index);
    }
}