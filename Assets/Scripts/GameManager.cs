using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(999)]
public class GameManager : MonoBehaviour
{
    public static event Action LevelLoaded;
    public static event Action<string> LevelChanged;
    public static event Action GameFinished;
    public static event Action Restart;
    public static bool IsLevelLoaded { get; private set; }
    public static bool isLevel;
    public static string CurrentLevel { get; set; }
    
    public static GameManager Instance { get; private set; }


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
        Instance = this;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.activeSceneChanged += OnLevelChanged;
    }

    private void Start()
    {
        IsLevelLoaded = true;
    }

    public void OnLevelChangedEvent(string levelName)
    {
        if (LevelChanged != null)
        {
            LevelChanged(LevelName);
        }
    }

    void OnLevelChanged(Scene scene, Scene scene2)
    {
        OnLevelChangedEvent(LevelName);
        Debug.Log("Game Manager: Level Changed to: " + LevelName);
    }


    public void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Game Manager: Level Loaded: " + LevelName);
        if(scene.name == CurrentLevel)
            StartCoroutine(LevelLoadedCor());
    }

    public static void OnRestart()
    {
        if(Restart != null)
        {
            Restart();
        }
    }

    IEnumerator LevelLoadedCor()
    {
        if (Controller.Instance == null) yield break;
        yield return Engine.Game.WaitForFrames(1);
        if (LevelLoaded != null)
        {
            Debug.Log("Level Loaded: " + SceneManager.GetActiveScene().name);
            LevelLoaded();
        }
        yield return null;
    }


    public void OnGameFinished()
    {
        var data = DataManager.Collections;
        var collectionManger = CollectionManager.Instance;
        int localID = Character.GetLocalPlayer().ID;
        data.coins += collectionManger.GetCollection(localID, CollectionType.Coin);
        data.emmeralds += collectionManger.GetCollection(localID, CollectionType.Emmerald);
        data.goldKeys += collectionManger.GetCollection(localID, CollectionType.KeyGold);
        data.silverKeys += collectionManger.GetCollection(localID, CollectionType.KeySilver);
        data.bronzeKeys += collectionManger.GetCollection(localID, CollectionType.KeyBronze);
        data.restarts += collectionManger.GetCollection(localID, CollectionType.Restart);

        DataManager.SaveData();
        Debug.Log("Game Saved");
        CollectionManager.Instance.ResetCollections();
        if (GameFinished != null)
        {
            GameFinished();
        }
    }


}