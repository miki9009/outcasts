using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Engine
{
    public class Benchmark : MonoBehaviour
    {
        [LevelSelector]
        public string nextLevel;
        Vector2 resolution;
        Vector2 curResolution;
        float curResFactor = 1;
        private void Start()
        {
            resolution = new Vector2(Screen.width, Screen.height);
            Debug.Log(resolution);
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            yield return new WaitForSeconds(1);
            int test = 10;
            curResolution = resolution;
            while(test > 0)
            {
                test--;
                if (fps < 50)
                {
                    if (curResFactor > 0.3f)
                    {
                        curResFactor -= 0.1f;
                    }
                    if (Screen.width > 600)
                    {
                        Screen.SetResolution((int)(resolution.x * curResFactor), (int)(resolution.y * curResFactor), true);
                        curResolution = new Vector2((int)(resolution.x * curResFactor), (int)(resolution.y * curResFactor));
                    }

                }
                yield return new WaitForSeconds(1);
            }
            SettingsContainer.Settings data = (SettingsContainer.Settings)DataManager.Instance.GetData("Settings");
            data.resolution = new Float2(curResolution.x, curResolution.y);
            data.runBenchmark = false;
            DataManager.Instance.SaveData();
            yield return new WaitForEndOfFrame();
            LevelManager.Instance.GoToScene(nextLevel);
            yield return null;
        }

        private void Update()
        {
            fps = Fps();
        }

        float fps;
        public static float deltaTime = 0.0F;
        public float Fps()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float msec = deltaTime * 1000.0f;
            return 1.0f / deltaTime;
        }
    }

}