using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(999)]
public class GameManager : MonoBehaviour
{
    public static event Action OnLevelLoaded;
    public static event Action OnLevelChanged;
    public static event Action OnGameFinished;
    public static bool LevelLoaded { get; private set; }
    public static bool isLevel;
    
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
        SceneManager.activeSceneChanged += LevelChanged;
    }

    private void Start()
    {
        LevelLoaded = true;
    }

    void LevelChanged(Scene scene, Scene scene2)
    {
        if (OnLevelChanged != null)
        {
            OnLevelChanged();
        }
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


    public void GameFinished()
    {
        var data = (CollectionsContainer.Container)DataManager.Instance.GetData("Collections");
        data.coins += CollectionManager.Instance.GetCollection(Controller.Instance.character.ID, CollectionType.Coin);
        data.emmeralds += CollectionManager.Instance.GetCollection(Controller.Instance.character.ID, CollectionType.Emmerald);
        data.goldKeys += CollectionManager.Instance.GetCollection(Controller.Instance.character.ID, CollectionType.KeyGold);
        data.silverKeys += CollectionManager.Instance.GetCollection(Controller.Instance.character.ID, CollectionType.KeySilver);
        data.bronzeKeys += CollectionManager.Instance.GetCollection(Controller.Instance.character.ID, CollectionType.KeyBronze);

        Debug.Log("Coins = " + data.coins);
        DataManager.SaveData();
        Debug.Log("Game Saved");
        CollectionManager.Instance.ResetCollections();
        if (OnGameFinished != null)
        {
            OnGameFinished();
        }
    }


}