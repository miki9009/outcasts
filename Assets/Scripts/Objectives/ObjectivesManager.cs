using UnityEngine;
using Engine;
using System.Collections.Generic;
using System.Linq;

namespace Objectives
{
    public class ObjectivesManager : LevelElement
    {
        public List<Objective> Objectives { get; private set; }
        public List<CollectionObjective> collectionObjectives;

        private void Start()
        {
            Objectives = new List<Objective>();
            foreach (var objective in collectionObjectives)
            {
                Objectives.Add(objective);
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

        public override void OnSave()
        {
            base.OnSave();
            if (data != null)
            {
                data["Objectives"] = collectionObjectives;
            }
        }

        public override void OnLoad()
        {
            base.OnLoad();
            if(data!=null)
            {
                if(data.ContainsKey("Objectives"))
                {
                    collectionObjectives = (List<CollectionObjective>)data["Objectives"];
                }
            }
        }
    }
}