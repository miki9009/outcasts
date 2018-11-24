﻿using Engine.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsWindow : UIWindow
{
    public Text resolutionLabel;
    public Text showFpsLabel;
    public Text graphicsLabel;
    int index;
    Vector2 currentRes = new Vector2(1920, 1080);

    public string[] graphicsLevel;
    int selectedGraphics = 1;


    bool buttonsMovement;
    bool showFps;

    private void Awake()
    {
        BeginShow += AssignSettings;
    }

    private void AssignSettings()
    {
        int width = Screen.currentResolution.width;
        int height = Screen.currentResolution.height;
        currentRes = new Vector2(width, height);
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (currentRes == resolutions[i])
            {
                index = i;
            }
        }
        SetResolutionString(width, height);
        buttonsMovement = DataManager.Settings.buttonMovement;
        showFps = DataManager.Settings.showFps;
        selectedGraphics = DataManager.Settings.graphicsLevel;
        graphicsLabel.text = graphicsLevel[selectedGraphics];

    }   

    static Vector2[] resolutions = new Vector2[]
    {
        new Vector2(800,450),
        new Vector2(1024, 576),
        new Vector2(1280,720),
        new Vector2(1366,768),
        new Vector2(1536,846),
        new Vector2(1680, 946),
        new Vector2(1920,1080)
    };

    public void Save()
    {
        Screen.SetResolution((int)currentRes.x, (int)currentRes.y,true);

        DataManager.Settings.resolution = currentRes;
        DataManager.Settings.buttonMovement = buttonsMovement;
        DataManager.Settings.showFps = showFps;
        DataManager.Settings.graphicsLevel = selectedGraphics;
        DataManager.SaveData();
    }

    public void ChangeResolution()
    {
        index++;
        if (index >= resolutions.Length)
        {
            index = 0;
        }
        currentRes = resolutions[index];
        SetResolutionString(currentRes.x, currentRes.y);
    }

    void SetResolutionString(float width, float height)
    {
        resolutionLabel.text = string.Format("{0}x{1}", width, height);
    }


    void SetFpsShowString(bool showFps)
    {
        showFpsLabel.text = showFps ? "ON" : "OFF";
    }

    public void ChangeMovementType()
    {
        buttonsMovement = !buttonsMovement;
    }

    public void ChangeShowFps()
    {
        showFps = !showFps;
        SetFpsShowString(showFps);
    }

    public void SetGraphicsLevel()
    {
        if(selectedGraphics < graphicsLevel.Length-1)
        {
            selectedGraphics++;
        }
        else
        {
            selectedGraphics = 0;
        }
        graphicsLabel.text = graphicsLevel[selectedGraphics];
    }


}