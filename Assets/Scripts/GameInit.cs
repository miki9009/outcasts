using UnityEngine;
using Engine;
using System.Collections;

public class GameInit : MonoBehaviour
{
    [LevelSelector]
    public string firstScene;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);        
    }

    private void Start()
    {
        var data = (SettingsContainer.Settings)DataManager.Instance.GetData("Settings");
        if (data.firstStart)
        {
            data.firstStart = false;
            DataManager.Instance.SaveData();
        }
        Init();
        DataManager.Instance.LoadData();
        //StartCoroutine(StartInit());
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
                var settings = (SettingsContainer.Settings)DataManager.Instance.GetData("Settings");
                if (!settings.runBenchmark)
                {
                    Screen.SetResolution((int)settings.resolution.x, (int)settings.resolution.y, true);
                    LevelManager.Instance.GoToScene(firstScene);
                }
                else
                {
                    LevelManager.Instance.GoToScene("benchmark");
                }

            };
        }
    }

}