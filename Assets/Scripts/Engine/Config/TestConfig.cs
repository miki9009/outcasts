using Engine.Config;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = fileName)]
public class TestConfig : Config
{
    public const string fileName = "Configs/Test";

    public string[] weekDays;
}