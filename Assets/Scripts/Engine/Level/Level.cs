using UnityEngine;
using System.Collections.Generic;
using System;

namespace Engine
{
    public class Level : MonoBehaviour
    {
        static LevelsConfig config;

        static LevelsConfig Config
        {
            get
            {
                if(config == null)
                {
                    config = Engine.Config.Config.GetConfigEditor<LevelsConfig>(LevelsConfig.key);
                }
                return config;
            }
        }

        public static string LevelElementsPath
        {
            get
            {
                return Config.levelElementsPath;
            }
        }

        static Dictionary<object, string> levelElements;
        [CustomLevelSelector]
        public string levelName;
        public static void Save(string levelName)
        {
            levelElements = new Dictionary<object, string>();
            var elements = GameObject.FindObjectsOfType<LevelElement>();
            try
            {
                foreach (var element in elements)
                {
                    element.OnSave();
                    levelElements.Add(element.data, element.GetName());
                }
                string savePath = Application.dataPath + "/Resources/" + Config.levelPaths + levelName + ".txt";
                Data.Save(savePath, levelElements, true);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            foreach (var element in elements)
            {
                if(element!=null && element.gameObject!=null)
                    DestroyImmediate(element.gameObject);
            }
        }

        public void Clear()
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
            foreach (var element in elements)
            {
                if (element != null && element.gameObject != null)
                    DestroyImmediate(element.gameObject);
            }
        }

        public static void Load(string levelName)
        {
            var elements = GameObject.FindObjectsOfType<LevelElement>();
            if (Application.isPlaying)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    Destroy(elements[i].gameObject);
                }
            }
            else
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    DestroyImmediate(elements[i].gameObject);
                }
            }
            TextAsset asset = Resources.Load(Config.levelPaths + levelName) as TextAsset;
            if(asset == null)
            {
                Debug.Log("Level was null");
                return;
            }
            Debug.Log("Path: " + Config.levelPaths + levelName);
            var bytes = asset.bytes;
            var data = Data.DesirializeFile(bytes);
            levelElements = (Dictionary<object, string>)data;
            foreach (var element in levelElements)
            {
                string path = Config.levelElementsPath + element.Value;
                var res = Resources.Load(path) as GameObject;
                GameObject obj = null;
                if (Application.isPlaying)
                    obj = (GameObject)Instantiate(res);
                else
                {
#if UNITY_EDITOR
                    obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(res);
#endif
                }
                if (obj != null)
                {
                    var levelElement = obj.GetComponent<LevelElement>();
                    levelElement.data = (Dictionary<string, object>)element.Key;
                    levelElement.OnLoad();
                }
                else
                {
                    Debug.LogError("Object was null, make sure it was in the LevelElements folder set in the config");
                }
            }
        }
    }
}