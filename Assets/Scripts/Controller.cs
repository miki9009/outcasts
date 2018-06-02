using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using Engine.GUI;

[DefaultExecutionOrder(-100)]
public class Controller : MonoBehaviour
{
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

    public float aspectRatio = 1;
    public bool ButtonMovement { get; set; }
    bool showFps;

    [HideInInspector] public Character character;

    [HideInInspector] public List<Character> characters = new List<Character>();

    [HideInInspector] public Camera gameCamera;
    [HideInInspector] public GameObject GUI;
    Vector2 startResolution;


    public VignetteAndChromaticAberration ChromaticAbberration { get; private set; }
    public Vortex Vortex { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameCamera = Camera.main;
        if (gameCamera == null)
        {
            Debug.LogError("Main camera is not set");
        }
        GUI = transform.parent.gameObject;
        ChromaticAbberration = gameCamera.GetComponent<VignetteAndChromaticAberration>();
        Vortex = gameCamera.GetComponent<Vortex>();
        ButtonMovement = DataManager.Settings.buttonMovement;
        showFps = DataManager.Settings.showFps;
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
        GameManager.OnLevelLoaded += DeactivateActionButton;
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
        StartCoroutine(PlayerDeadCoroutine(character));
        if(PlayerDead != null)
        {
            PlayerDead(character);
        }
    }

    IEnumerator PlayerDeadCoroutine(Character character)
    {
        yield return new WaitForSeconds(3f);
        int currentRestarts = CollectionManager.Instance.GetCollection(character.ID, CollectionType.Restart);
        var collections = DataManager.Collections;
        if(collections.restarts > 0 || currentRestarts > 0)
        {
            character.movement.enabled = true;
            character.stats.health = 1;
            character.movement.characterHealth.AddHealth(character.stats.health);
            character.movement.anim.Play("Idle");
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
        else
        {
            UIWindow.GetWindow(UIWindow.END_SCREEN).RestartLevel();
        }
        yield return null;
    }




    private void OnGUI()
    {
        if (showFps)
        {
            Draw.DisplayFps(Screen.width / 2, 10, Color.red, 40);
            Draw.DisplayMedianFps(Screen.width / 2 - Screen.width * 0.1f, 70);
        }

    }

}
