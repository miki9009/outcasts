using UnityEngine;
using System;
using UnityEngine.Events;

namespace Objectives
{
    [Serializable]
    public class Objective
    {
        public string title;
        public bool startOnAwake;

        public virtual float Progress { get; private set; }
        [HideInInspector]
        public State state;
        public event Action<Objective> Started;
        public event Action<Objective> ProgressUpdated;
        public event Action<Objective> Finished;

        public virtual void Start()
        {

        }

        protected void OnStart()
        {
            if(Started != null)
            {
                Started(this);
            }
        }

        protected void OnProgressUpdated()
        {
            if(ProgressUpdated!= null)
            {
                ProgressUpdated(this);
            }
        }

        protected virtual void OnFinished()
        {
            if(Finished!= null)
            {
                Finished(this);
            }
        }



    }

    public enum State
    {
        InProgress,
        Failed,
        Completed
    }
}