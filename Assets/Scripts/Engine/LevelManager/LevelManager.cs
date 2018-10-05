using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Engine;
using System.IO;
using System.Collections;
using Engine.GUI;

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

    public string LastCustomLevel
    {
        get
        {
            return customLevelToLoad;
        }
    }

    public int levelIndex;
    public string scenesPath;
    public List<string> scenes;
    public List<Scenes> levels;
    public string currentLevel;
    [LevelSelector]
    public string gameScene;

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
        GameManager.LevelLoaded += () =>
        {
            currentLevel = SceneManager.GetActiveScene().name;
            bool isLevel = levels.Exists(x => x.sceneName == currentLevel);
            if (isLevel)
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    if (levels[i].sceneName == currentLevel)
                    {
                        levelIndex = i;
                        Debug.Log("LEVEL LOADED: " + levels[levelIndex].sceneName);
                    }
                }
            }
            StartCoroutine(HideLoadingScreen());

        };
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);

        LevelSelected?.Invoke();
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

    static void LoadScene(string sceneName, LoadSceneMode mode)
    {
        UIWindow.GetWindow(UIWindow.LOADING_SCREEN).Show();
        Debug.Log("Show loading screen");
        SceneManager.LoadSceneAsync(sceneName, mode);
    }


    static string levelToLoad;
    static string customLevelToLoad;
    public static void BeginLevelLoadSequence(string levelName)
    {
        levelToLoad = levelName;
        Debug.Log("Current level set to: " + levelToLoad);
        GameManager.CurrentLevel = levelToLoad;
        LoadScene(LevelManager.Instance.gameScene, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += instance.AddLevelScene;
    }

    public static void BeginCustomLevelLoadSequenceAdditive(string sceneName, string customLevel)
    {
        levelToLoad = sceneName;
        customLevelToLoad = customLevel;
        Debug.Log("Current level set to: " + levelToLoad);
        GameManager.CurrentLevel = levelToLoad;
        LoadScene(LevelManager.Instance.gameScene, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += instance.AddLevelScene;
        LoadCustomLevel += OnLoadCustomLevel;
    }



    public static void ChangeLevel(string sceneName, string customLevel)
    {
        if(sceneName != GameManager.CurrentLevel)
            SceneManager.UnloadSceneAsync(GameManager.CurrentLevel);
        levelToLoad = sceneName;
        customLevelToLoad = customLevel;
        Debug.Log("Current level set to: " + levelToLoad);
        GameManager.CurrentLevel = levelToLoad;
        instance.AddLevelScene(SceneManager.GetSceneByName(sceneName), LoadSceneMode.Additive);
        LoadCustomLevel += OnLoadCustomLevel;
    }

    void AddLevelScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= AddLevelScene;
        LoadScene(levelToLoad, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += SetActiveScene;
    }

    static event Action LoadCustomLevel;
    void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelToLoad));
        SceneManager.sceneLoaded -= SetActiveScene;
        LoadCustomLevel?.Invoke();
    }

    public static void ReturnToMenu()
    {
        SceneManager.UnloadSceneAsync(instance.gameScene);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    static void OnLoadCustomLevel()
    {
        if(!string.IsNullOrEmpty(customLevelToLoad))
        {
            Level.LoadWithScene(SceneManager.GetActiveScene().name, customLevelToLoad);
        }
        LoadCustomLevel -= OnLoadCustomLevel;
    }


    public string NextLevel()
    {
        int index = 0;
        if (levelIndex+1 < levels.Count)
        {
            index = levelIndex + 1;
        }
        return levels[index].sceneName;
    }

    IEnumerator HideLoadingScreen()
    {
        UIWindow.GetWindow(UIWindow.LOADING_SCREEN).Hide();
        Debug.Log("Screen Hide");
        yield return null;
    }
}

[Serializable]
public class Scenes
{
    [LevelSelector]
    public string sceneName;
}
