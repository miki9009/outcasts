using System.Collections.Generic;
using UnityEngine;
using Engine.Config;
using System.Linq;
using System;

public class ConfigsManager : MonoBehaviour
{
    public List<UnityEngine.Object> configs;

    static ConfigsManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static T GetConfig<T>() where T : Config
    {
        return (T)instance.configs.SingleOrDefault(x => x.GetType() == typeof(T));
    }

    public static T GetConfigs<T>() where T : Config
    {
        return (T)instance.configs.Where(x => x.GetType() == typeof(T));
    }

}
