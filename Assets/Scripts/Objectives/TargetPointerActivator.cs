﻿using UnityEngine;

public class TargetPointerActivator : MonoBehaviour
{
    TargetPointer arrow;

    public void AssignArrow(TargetPointer arrow)
    {
        this.arrow = arrow;
        if(!gameObject.activeInHierarchy)
        {
            arrow.gameObject.SetActive(false);
        }
    }

    public void Enable(bool val)
    {
        if(arrow!=null)
            arrow.gameObject.SetActive(val);
    }

    private void OnEnable()
    {
        Enable(true);
    }

    private void OnDisable()
    {
        Enable(false);
    }
}