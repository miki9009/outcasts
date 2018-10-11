using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Objectives
{
    public class ObjectivesManager : LevelElement
    {
        public static List<Objective> Objectives { get; private set; }

        public List<Objective> activeObjectives = new List<Objective>();
        public ObjectiveSequence sequence;
        
        public static bool EndingGame { get; private set; }

        int sequenceIndex = 0;

        static ObjectivesManager instance;

        private void Start()
        {
            instance = this;

            Objectives = new List<Objective>();
            foreach (var sequence in sequence.sequences)
            {
                for (int i = 0; i < sequence.objectives.Count; i++)
                {
                    Objectives.Add(sequence.objectives[i]);
                }
            }
            TriggerSequence();
        }

        void TriggerSequence()
        {
            activeObjectives = new List<Objective>();

            for (int i = 0; i < sequence.sequences[sequenceIndex].objectives.Count; i++)
                activeObjectives.Add(sequence.sequences[sequenceIndex].objectives[i]);

            foreach (var objective in activeObjectives)
            {
                ObjectivesPanelManager.AddPanel(objective);
                objective.Start();
            }
            sequenceIndex++;
        }

        public static event System.Action<Objective> ObjectiveEnded;
        public static void OnObjectiveEnded(Objective objective)
        {
            bool allObjectivesConpleted = true;
            foreach (var o in Objectives)
            {
                if (!o.IsFinished && !o.optional)
                {
                    allObjectivesConpleted = false;
                    break;
                }
            }

            if (allObjectivesConpleted)
                GameManager.State = GameManager.GameState.Completed;
            //GameManager.Instance.EndGame(GameManager.GameState.Completed);

            ObjectiveEnded?.Invoke(objective);
                        if (((CollectionObjective)objective).triggerSequence)
                instance.TriggerSequence();
        }

        public override void OnSave()
        {
            base.OnSave();
            if (data != null)
            {
                data["Sequence"] = sequence;
            }
        }

        public override void OnLoad()
        {
            base.OnLoad();
            if(data!=null)
            {
                if(data.ContainsKey("Sequence"))
                {
                    sequence = (ObjectiveSequence)data["Sequence"];
                }
            }
        }

        public static IEnumerator EndGame(GameManager.GameState state, float time = 1)
        {
            EndingGame = true;
            yield return new WaitForSeconds(time);
            GameManager.Instance.EndGame(state);
            EndingGame = false;
        }
    }
}