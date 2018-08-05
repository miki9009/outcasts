using System.Collections;
using UnityEngine;
using Engine;

public class Waypoint : LevelElement
{
    public int index = 0;
    public float height = 1;
    public float fadeDuration = 1;
    public bool active;

    Vector3 startScale;


    private void Awake()
    {
        startScale = transform.localScale;
        WaypointManager.Instance.waypoints.Add(index, this);
        transform.SetParent(WaypointManager.Instance.transform);
    }

    private void Start()
    {
        transform.localScale = new Vector3(startScale.x, 0.1f, startScale.z);
        if (index == 0)
        {
            WaypointManager.Instance.currentWaypoint = this;
            StartCoroutine(Expand());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!active && WaypointManager.Instance.currentWaypoint == this)
            StartCoroutine(Shrink());
    }

    IEnumerator Expand()
    {
        active = true;
        float duration = 0;
        while(duration < 1)
        {
            duration += Time.deltaTime / fadeDuration;
            transform.localScale = new Vector3(startScale.x, Mathf.Lerp(0,height,duration), startScale.z);
            yield return null; 
        }
    }

    IEnumerator Shrink()
    {
        active = false;
        Waypoint way;
        if(WaypointManager.Instance.waypoints.TryGetValue(index+1, out way))
        {
            WaypointManager.Instance.currentWaypoint = way;
        }
        else
        {
            Debug.Log("No more wayppoints");
        }
        transform.localScale = new Vector3(startScale.x, 0, startScale.z);
        float duration = 0;
        while (duration < 1)
        {
            duration += Time.deltaTime / fadeDuration;
            transform.localScale = new Vector3(startScale.x, Mathf.Lerp(height, 0, duration), startScale.z);
            yield return null;
        }
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
}