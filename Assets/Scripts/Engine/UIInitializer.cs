using System;
using UnityEngine;
public class UIInitializer : MonoBehaviour
{
    public static UIInitializer Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public string mainMenuName;

}