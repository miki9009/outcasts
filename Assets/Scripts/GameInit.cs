﻿using UnityEngine;
using Engine;
using Engine.Config;
using System.Collections;

public class GameInit : MonoBehaviour
{
    [LevelSelector]
    public string firstScene;
    [LevelSelector]
    public string benchmarkScene;
    public bool disableFirstRunInEditor;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);        
    }

    private void Start()
    {
        var data = (Settings.Container)DataManager.Instance.GetData(DataManager.Containers.SETTINGS);
#if !UNITY_EDITOR
        if (data.firstStart)
        {
            data.firstStart = false;
            DataManager.SaveData();
        }
#endif
        Init();
        DataManager.LoadData();
    }

    private void OnDestroy()
    {
        DataManager.Loaded -= Init;
    }

    private void Init()
    {
        if (Application.isPlaying)
        {
            DataManager.Loaded += () =>
            {
                var settings = (Settings.Container)DataManager.Instance.GetData(DataManager.Containers.SETTINGS);
#if UNITY_EDITOR
                if (!disableFirstRunInEditor)
                {
                    if (!settings.runBenchmark)
                    {
                        Screen.SetResolution((int)settings.resolution.x, (int)settings.resolution.y, true);
                        LevelManager.Instance.GoToScene(firstScene, false);
                    }
                    else
                    {
                        LevelManager.Instance.GoToScene(benchmarkScene,false);
                    }
                }
                else
                {
                    LevelManager.Instance.GoToScene(firstScene,false);
                }
#else
                if (!settings.runBenchmark)
                {
                    Screen.SetResolution((int)settings.resolution.x, (int)settings.resolution.y, true);
                    LevelManager.Instance.GoToScene(firstScene);
                }
                else
                {
                    LevelManager.Instance.GoToScene(benchmarkScene);
                }
#endif
            };
        }
    }


}