using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class Controller : MonoBehaviour
{
    public GameObject gameUI;
    Vector2 defaultResolution;
    bool defaultRes = true;


    public static Controller Instance
    {
        get;
        private set;
    }

    public float aspectRatio = 1;


    [HideInInspector] public Character character;

    [HideInInspector] public List<Character> characters = new List<Character>();

    [HideInInspector] public Camera gameCamera;
    [HideInInspector] public GameObject GUI;
    Vector2 startResolution;
    Vector2 curResolution;
    float curResFactor = 1;

    private void Awake()
    {
        Instance = this;
        gameCamera = Camera.main;
        if (gameCamera == null)
        {
            Debug.LogError("Main camera is not set");
        }
        GUI = transform.parent.gameObject;
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
        
    }

    void DeactivateActionButton()
    {
        Debug.Log("Went off");
        var button = GameGUI.GetButtonByName("Action");
        if (button != null && button.gameObject != null)
        {
            button.gameObject.SetActive(false);
        }
    }




    private void OnGUI()
    {
        Draw.DisplayFps(Screen.width / 2, 10, Color.red, 40);
        Draw.TextColorUnity(10, 180, Color.red, "Screen Resolution: " + Screen.width + "x" + Screen.height);
        //if (UnityEngine.GUI.Button(new Rect(Screen.width - 135, 10, 130, 50), "Change Resolution"))
        //{
        //    if (curResFactor > 0.3f)
        //    {
        //        curResFactor -= 0.1f;
        //    }
        //    else
        //    {
        //        curResFactor = 1;
        //        Screen.SetResolution((int)(startResolution.x * curResFactor), (int)(startResolution.y * curResFactor), true);
        //    }
        //    if (Screen.width > 600)
        //    {
        //        Screen.SetResolution((int)(startResolution.x * curResFactor), (int)(startResolution.y * curResFactor), true);
        //    }
        //}
    }

}
