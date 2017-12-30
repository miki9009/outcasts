using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Engine;
using System.IO;

public class LevelManager : MonoBehaviour
{
    static LevelManager instance;
    public static event Action LevelSelected;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("LevelManager").AddComponent<LevelManager>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    public string scenesPath;
    public List<string> scenes;

    public static string[] Scenes
    {
        get
        {
            return GetScenesStatic();
        }
    }


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);

        if (LevelSelected != null)
        {
            LevelSelected();

        }
    }

    public void GetScenes()
    {
        if (scenesPath != null)
        {
            scenes.Clear();
            var path = Application.dataPath + "/" + scenesPath;
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    var f = file.Substring(path.Length + 1);
                    if (f.Contains(".unity") && !f.Contains(".meta"))
                    {
                        var str = f.Split('.');
                        scenes.Add(str[0]);
                    }
                }
            }
        }
    }

    public static string[] GetScenesStatic()
    {
        List<string> scenes = new List<string>();
        scenes.Clear();
        var path = Application.dataPath + "/Scenes";
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                var f = file.Substring(path.Length + 1);
                if (f.Contains(".unity") && !f.Contains(".meta"))
                {
                    var str = f.Split('.');
                    scenes.Add(str[0]);
                }
            }
        }
        else
        {
            Debug.LogError("Make sure that /Scenes exist, and that scenes are inside of it");
        }
        return scenes.ToArray();
    }
}
