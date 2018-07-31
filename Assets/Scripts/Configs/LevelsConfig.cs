using Engine.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = key)]
public class LevelsConfig : Config
{
    public const string key = "Configs/LevelsConfig";
    public string levelPaths;
    public string levelElementsPath;
    public List<LevelGroup> levels;


}
[System.Serializable]
public class LevelGroup
{
    [LevelSelector]
    public string sceneName;
    public string levelName;
}


