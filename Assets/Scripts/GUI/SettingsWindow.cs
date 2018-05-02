using Engine.GUI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SettingsWindow : UIWindow
{
    public Text resolutionLabel;
    int index;
    Vector2 currentRes = new Vector2(1920, 1080);

    private void Awake()
    {
        BeginShow += ShowStartRes;
    }

    private void ShowStartRes()
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
        var settings = (Settings.Container)DataManager.Instance.GetData(DataManager.Containers.SETTINGS);
        settings.resolution = currentRes;
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


}