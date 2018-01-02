using Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    //public GameObject cam;
    public UIWindow guiWindow;

    private void Start()
    {
        GameManager.OnLevelLoaded += () => guiWindow.FadeIn();
    }

    //public void ActivateCamera()
    //{
    //    if (cam != null)
    //    {
    //        cam.SetActive(true);
    //    }
    //}

    //public void DeactivateCamera()
    //{
    //    if (cam != null)
    //    {
    //        cam.SetActive(false);
    //    }
    //}
}
