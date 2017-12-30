using System;
using UnityEngine;
public class UIInitializer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public string mainMenuName;

    void Init()
    {

    }
}