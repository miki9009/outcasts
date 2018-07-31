using System;

namespace Objectives
{
    [Serializable]
    public class CollectionObjective : Objective
    {
        public int collectionAmount;
        public CollectionType collectionType;
        int collected;

        public override float Progress
        {
            get
            {
                return ((float)collected) / collectionAmount;
            }
        }

        void UpdateProgress(int id, CollectionType collection, int val)
        {
            UnityEngine.Debug.Log("Objective Updated: " + title + " val: " + val);
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
            if (CollectionManager.Instance != null)
                CollectionManager.Instance.Collected -= UpdateProgress;
            state = State.Failed;
            OnFinished();
        }


        public override void Start()
        {
            state = State.InProgress;
            CollectionManager.Instance.Collected += UpdateProgress;
        }

        //~CollectionObjective()
        //{
        //    if (CollectionManager.Instance != null)
        //        CollectionManager.Instance.Collected -= UpdateProgress;
        //}
    }
}