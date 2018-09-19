using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [LevelSelector]
    public string levelName;
    [CustomLevelSelector]
    public string customLevel;


    public void GoToLevelAdditive()
    {
        try
        {
            Camera.main.gameObject.SetActive(false);
        }
        catch { }
        LevelManager.BeginCustomLevelLoadSequenceAdditive(levelName, customLevel);
    }

}