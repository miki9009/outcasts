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

    public void GoToLevel()
    {
        SceneManager.LoadSceneAsync(levelName);
    }
}