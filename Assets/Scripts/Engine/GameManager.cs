using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(999)]
public class GameManager : MonoBehaviour
{
    public static event Action OnLevelLoaded;
    public static event Action OnLevelChanged;
    public static bool LevelLoaded { get; private set; }
    public static bool isLevel;
    

    private string currentName;
    public string LevelName
    {
        get
        {
            return SceneManager.GetActiveScene().name;
        }
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.activeSceneChanged += LevelChanged;
    }

    private void Start()
    {
        LevelLoaded = true;
    }

    void LevelChanged(Scene scene, Scene scene2)
    {
        Debug.Log("Game Manager: Level Changed to: " + LevelName);
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Game Manager: Level Loaded: " + LevelName);
        StartCoroutine(LevelLoadedCor());
    }

    IEnumerator LevelLoadedCor()
    {
        if (Controller.Instance == null) yield break;
        yield return Engine.Game.WaitForFrames(1);
        if (OnLevelLoaded != null)
        {
            OnLevelLoaded();
        }
        yield return null;

    }


}