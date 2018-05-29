using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeAppearance : MonoBehaviour
{
    public bool activated;
    public List<ActiveObject> objects;

    private void Awake()
    {
        objects = GetAllChildren();
        DetachChildren();
    }

    private void Start()
    {
        if (!activated)
        {
            StartCoroutine(DeactivateChildren());
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!activated)
        {
            StartCoroutine(ActivateChildren());
        }
        activated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (activated)
        {
            StartCoroutine(DeactivateChildren());
            activated = false;
        }
    }

    private List<ActiveObject> GetAllChildren()
    {
        var list = new List<ActiveObject>();
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            var child = transform.GetChild(i);
            var active = child.gameObject.AddComponent<ActiveObject>();
            active.Manager = this;
             list.Add(active);
        }
        return list;
    }

    private void DetachChildren()
    {
        transform.DetachChildren();
    }

    private IEnumerator DeactivateChildren()
    {
        var wait = new WaitForSeconds(0.1f);

        var scaleFactor = Vector3.one * 0.1f;
        var scale = Vector3.one;
        int j = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj.gameObject != null && obj.gameObject.activeInHierarchy)
            {
                obj.DeactivatedByManager = true;
                obj.gameObject.SetActive(false);
            }
            j++;
            if (j == 5)
            {
                j = 0;
                yield return null;
            }
        }
        yield return null;
    }

    private IEnumerator ActivateChildren()
    {
        var wait = new WaitForSeconds(0.1f);

        var scaleFactor = Vector3.one * 0.1f;
        var scale = Vector3.one;
        int j = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj.gameObject != null && !obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.transform.localScale = obj.scale;
                obj.gameObject.SetActive(true);
            }
            j++;
            if (j == 5)
            {
                j = 0;
                yield return wait;
            }
        }
        yield return null;
    }




}
