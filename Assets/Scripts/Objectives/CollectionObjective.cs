﻿using System;
using System.Collections;
using UnityEngine;

namespace Objectives
{
    [Serializable]
    public class CollectionObjective : Objective
    {
        public int collectionAmount;
        public CollectionType collectionType;
        int collected;
        public bool isTimer;
        public float time;

        [NonSerialized]
        public float startTimer;

        public event Action ClockUpdate;

        public override float Progress
        {
            get
            {
                return ((float)collected) / collectionAmount;
            }
        }

        void UpdateProgress(int id, CollectionType collection, int val)
        {
            if (collection == collectionType)
            {
                collected += val;
                OnProgressUpdated();
                if(collected >= collectionAmount)
                {
                    state = State.Completed;
                    UnityEngine.Debug.Log("Objective Finished: " + title);
                    if(CollectionManager.Instance!=null)
                     CollectionManager.Instance.Collected -= UpdateProgress;
                    OnFinished();
                }
            }
        }

        public void Failed()
        {
            state = State.Failed;
            OnFinished();
            if (!optional && !ObjectivesManager.EndingGame)
            {
                CoroutineHost.Start(ObjectivesManager.EndGame(GameManager.GameState.Failed));
            }
        }

        void Timer()
        {
            if(time > 0)
            {
                time--;
            }
            else
            {
                Failed();
            }
            ClockUpdate?.Invoke();
        }



        public override void Start()
        {
            state = State.InProgress;
            CollectionManager.Instance.Collected += UpdateProgress;
            if(isTimer)
            {
                GameTime.Instance.TimeElapsed += Timer;
                startTimer = time;
            }
        }

        protected override void OnFinished()
        {
            base.OnFinished();
            if (CollectionManager.Instance != null)
            {
                CollectionManager.Instance.Collected -= UpdateProgress;
            }
            if (isTimer)
                GameTime.Instance.TimeElapsed -= Timer;
        }
    }
}