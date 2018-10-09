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
        public ObjectiveSequence sequence;
        
        public static bool EndingGame { get; private set; }

        private void Start()
        {
            Objectives = new List<Objective>();
            foreach (var sequence in sequence.sequences)
            {
                for (int i = 0; i < sequence.objectives.Count; i++)
                {
                    Objectives.Add(sequence.objectives[i]);
                }
            }
            foreach (var objective in Objectives)
            {
                ObjectivesPanelManager.AddPanel(objective);
                if(objective.startOnAwake)
                {
                    objective.Start();
                }
            }
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