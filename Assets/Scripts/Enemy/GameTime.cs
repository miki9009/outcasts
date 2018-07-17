using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    float curTime = 0;
    int seconds = 0;

    public event Action OnTimeElapsed;
    public event Action<int> OnTimeAdded;
    public event Action OnEndOfTime;
    public static bool TimerRunning { get; set; }

    private static GameTime instance;
    public static GameTime Instance
    {
        get    
        {
            if (instance == null)
            {
                var go = new GameObject("GameTime");
                go.AddComponent<GameTime>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    public int timeToFinish;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("Instance of GameTime was already defined");
        }
        GameManager.LevelLoaded += () => TimerRunning = true;
    }

    private void FixedUpdate()
    {
        if (TimerRunning)
        {
            curTime += Time.fixedDeltaTime;
            if (curTime >= 1)
            {
                curTime = 0;
                seconds++;
                if (timeToFinish > 0)
                {
                    timeToFinish--;
                }
                else
                {
                    Debug.Log("TIME ELAPSED");
                    if (OnEndOfTime != null)
                    {
                        OnEndOfTime();
                    }
                }
                if (OnTimeElapsed != null)
                {
                    OnTimeElapsed();
                }
            }
        }
    }

    public void AddTime(int time)
    {
        timeToFinish += time;
        if (OnTimeAdded != null)
        {
            OnTimeAdded(time);
        }
    }
}