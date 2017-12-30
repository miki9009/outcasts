using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject cam;

    public void ActivateCamera()
    {
        if (cam != null)
        {
            cam.SetActive(true);
        }
    }

    public void DeactivateCamera()
    {
        if (cam != null)
        {
            cam.SetActive(false);
        }
    }
}
