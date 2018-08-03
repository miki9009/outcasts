using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    float curTime = 0;
    int seconds = 0;

    public event Action TimeElapsed;
    public event Action<int> TimeAdded;
    public event Action EndOfTime;
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

    bool finished;
    private void FixedUpdate()
    {
        if (TimerRunning)
        {
            curTime += Time.fixedDeltaTime;
            if (curTime >= 1 || finished)
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
                    finished = true;
                    if (EndOfTime != null)
                    {
                        EndOfTime();
                    }
                }
                if (TimeElapsed != null)
                {
                    TimeElapsed();
                }
            }
        }
    }

    public void AddTime(int time)
    {
        timeToFinish += time;
        if (TimeAdded != null)
        {
            TimeAdded(time);
        }
    }
}