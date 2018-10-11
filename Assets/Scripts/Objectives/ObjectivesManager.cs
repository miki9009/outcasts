﻿using UnityEngine;
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
        public List<LevelElement> levelElementReferences;
        
        public static bool EndingGame { get; private set; }

        int sequenceIndex = 0;

        static ObjectivesManager instance;

        int[] references;

#if UNITY_EDITOR
        public bool catchReferences = true;
#endif


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
            Level.ElementsLoaded += CatchReferences;
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

            references = new int[levelElementReferences.Count];
            for (int i = 0; i < levelElementReferences.Count; i++)
            {
                references[i] = levelElementReferences[i].elementID;
            }
            if (data != null)
            {
                data["Sequence"] = sequence;
                data["References"] = references;
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
                if (data.ContainsKey("References"))
                {
                    references = (int[])data["References"];
                }
                catchReferences = true;
            }
        }

        public static IEnumerator EndGame(GameManager.GameState state, float time = 1)
        {
            EndingGame = true;
            yield return new WaitForSeconds(time);
            GameManager.Instance.EndGame(state);
            EndingGame = false;
        }

        private void OnDestroy()
        {
            Level.ElementsLoaded -= CatchReferences;
        }

        public void CatchReferences()
        {
            levelElementReferences = new List<LevelElement>();
            if (references == null) return;
            for (int i = 0; i < references.Length; i++)
            {
                if (Level.loadedElements.ContainsKey(references[i]))
                    levelElementReferences.Add(Level.loadedElements[references[i]]);
            }
        }

    }
}