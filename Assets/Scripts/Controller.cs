﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Engine;
using Engine.GUI;
using Engine.Config;

[DefaultExecutionOrder(-100)]
public class Controller : MonoBehaviour
{
    public BloomOptimized bloom;
    public Antialiasing antialiasing;
    public enum GameType { Perspective, Ortographic}
    public GameType gameType = GameType.Perspective;
    public GameObject gameUI;
    Vector2 defaultResolution;
    public Color levelColor;
    public Checkpoint LastCheckpoint { get; set; }
    public int restarts = 5;
    public static Controller Instance
    {
        get;
        private set;
    }
    public bool IsRestarting { get; private set; }

    static SpawnsConfig spawnsConfig;
    public static SpawnsConfig SpawnsConfig
    {
        get
        {
            if(spawnsConfig == null)
            {
                spawnsConfig = ConfigsManager.GetConfig<SpawnsConfig>();
            }
            return spawnsConfig;
        }
    }

    public float aspectRatio = 1;

    public bool ButtonMovement { get; set; }
    bool showFps;

    [HideInInspector] public Character character;

    [HideInInspector] public List<Character> characters = new List<Character>();

    [HideInInspector] public GameCamera gameCamera;
    [HideInInspector] public GameObject GUI;
    Vector2 startResolution;


    public VignetteAndChromaticAberration ChromaticAbberration { get; private set; }
    public Vortex Vortex { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameCamera = Camera.main.GetComponentInParent<GameCamera>();
        if (gameCamera == null)
        {
            Debug.LogError("Main camera is not set");
        }
        GUI = transform.parent.gameObject;
        ChromaticAbberration = gameCamera.GetComponent<VignetteAndChromaticAberration>();
        Vortex = gameCamera.GetComponent<Vortex>();
        GameManager.Restart += OnRestart;
        if (DataManager.Exists())
        {
            ButtonMovement = DataManager.Settings.buttonMovement;
            showFps = DataManager.Settings.showFps;
        }
    }

    private void OnDestroy()
    {
        GameManager.Restart -= OnRestart;
    }

    void OnRestart()
    {
        IsRestarting = false;
        RestartCharacter(Character.GetLocalPlayer());
    }

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
        defaultResolution = new Vector2(Screen.width, Screen.height);
        aspectRatio = (float)Screen.width / (float)Screen.height;
        //try
        //{
        //    //GameGUI.GetButtonByName("ButtonRestart").OnTapPressed.AddListener(Restart);
        //    //GameGUI.GetButtonByName("ButtonPause").OnTapPressed.AddListener(Restart);
        //}
        //catch { }
        startResolution = new Vector2(Screen.width, Screen.height);
        GameManager.LevelLoaded += DeactivateActionButton;
        PlayerDead += DeactivateActionButtonOnPlayerDeath;
        Draw.ResetMedianFps();
    }

    void DeactivateActionButtonOnPlayerDeath(Character character)
    {
        DeactivateActionButton();
        ActivationTrigger.activatedTriggers = 0;
    }

    void DeactivateActionButton()
    {
        //Debug.Log("Went off");
        var button = GameGUI.GetButtonByName("Action");
        if (button != null && button.gameObject != null)
        {
            button.gameObject.SetActive(false);
        }
    }

    public event System.Action<Character> PlayerDead;
    public void OnPlayerDead(Character character)
    {
        float time = character.isDead ? 3 : 0f;
        StartCoroutine(PlayerDeadCoroutine(character, time));
        if(PlayerDead != null)
        {
            PlayerDead(character);
        }
    }

    IEnumerator PlayerDeadCoroutine(Character character, float waitTime)
    {
        if (IsRestarting)
            yield break;
        IsRestarting = true;
        yield return new WaitForSeconds(waitTime);

        int currentRestarts = CollectionManager.Instance.GetCollection(character.ID, CollectionType.Restart);
        var collections = DataManager.Collections;
        if(collections.restarts > 0 || currentRestarts > 0)
        {
            RestartCharacter(character);
        }
        else
        {
            GameManager.Instance.EndGame(GameManager.GameState.Failed);
            //UIWindow.GetWindow(UIWindow.END_SCREEN).RestartLevel();
        }
        yield return null;
    }

    void RestartCharacter(Character character)
    {
        character.movement.enabled = true;
        character.stats.health = 1;
        character.movement.characterHealth.AddHealth(character.stats.health);
        character.movement.anim.Play("Idle");
        int currentRestarts = CollectionManager.Instance.GetCollection(character.ID, CollectionType.Restart);
        var collections = DataManager.Collections;
        if (currentRestarts > 0)
        {
            CollectionManager.Instance.SetCollection(character.ID, CollectionType.Restart, currentRestarts - 1);
        }
        else
        {
            collections.restarts--;
            DataManager.SaveData();
        }
        character.rb.velocity = Vector3.zero;
        //character.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
        gameCamera.GetComponent<GameCamera>().target = character.transform;
        if (LastCheckpoint != null)
        {
            LastCheckpoint.ResetToCheckpoint(character);
        }
        else
        {
            character.transform.position = character.movement.StartPosition;
        }
    }



    bool rayOn = true;
    GameObject[] rays;
    private void OnGUI()
    {
        if (showFps)
        {
            Draw.DisplayFpsMedian(Screen.width / 2, 10, Color.red, 40);
            //Draw.DisplayMedianFps(Screen.width / 2 - Screen.width * 0.1f, 70);
        }

        if (UnityEngine.GUI.Button(new Rect(10, 60, 100, 50), "Bloom: " + bloom.enabled))
        {
            if (bloom != null) bloom.enabled = !bloom.enabled;
        }
        if (UnityEngine.GUI.Button(new Rect(110, 60, 100, 50), "Antialiasing: " + antialiasing.enabled))
        {
            if (antialiasing != null) antialiasing.enabled = !antialiasing.enabled;
        }




    }

}
