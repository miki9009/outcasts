using Engine;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

namespace Objectives
{

    [CustomEditor(typeof(ObjectivesManager))]
    public class ObjectivesManagerEditor : Editor
    {
        ObjectivesManager objectivesManager;

        bool toggleTxt = false;

        List<bool> foldout;


        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            objectivesManager = (ObjectivesManager)target;
            if (objectivesManager.elementID == -1)
                objectivesManager.elementID = Level.GetID();
            //toggleTxt = GUILayout.Toggle(toggleTxt, "A Toggle text");

            if (foldout == null || foldout.Count != objectivesManager.sequence.sequences.Count)
            {
                int count = objectivesManager.sequence.sequences.Count;

                if (foldout == null)
                    foldout = new List<bool>();
                while(foldout.Count < count)
                {
                    foldout.Add(false);
                }
                while(foldout.Count > count)
                {
                    foldout.RemoveAt(foldout.Count - 1);
                }
            }

            Color defaultColor = GUI.backgroundColor;


            //GUI.contentColor = Color.red;
            //GUI.color = Color.gray;
            Color col = new Color(0.5f, 0.7f, 0.8f);

            for (int i = 0; i < objectivesManager.sequence.sequences.Count; i++)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = col;
                foldout[i] = EditorGUILayout.Foldout(foldout[i], "SEQUENCE: " + i, true);
                if (foldout[i])
                {
                    var sequence = objectivesManager.sequence.sequences[i];

                    for (int j = 0; j < sequence.objectives.Count; j++)
                    {
                        EditorGUILayout.LabelField("Objective", EditorStyles.boldLabel);
                        //GUI.backgroundColor = colors[j % 2];
                        var objective = sequence.objectives[j];
                        GUILayout.BeginVertical(EditorStyles.helpBox);

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Title", GUILayout.Width(50));
                        objective.title = GUILayout.TextField(objective.title, GUILayout.Width(250), GUILayout.MinWidth(50));
                        objective.triggerSequence = GUILayout.Toggle(objective.triggerSequence, "Trigger Sequence");
                        objective.optional = GUILayout.Toggle(objective.optional, "Is Optional");
                        if (GUILayout.Button("X", GUILayout.Width(35)))
                        {
                            sequence.objectives.Remove(objective);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Collection Type: ", GUILayout.Width(100));
                        objective.collectionType =(CollectionType) EditorGUILayout.EnumPopup(objective.collectionType, GUILayout.MinWidth(150), GUILayout.MinWidth(50));
                        EditorGUILayout.LabelField("Amount: ", GUILayout.Width(70));
                        objective.collectionAmount = EditorGUILayout.IntField(objective.collectionAmount);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                        GUILayout.EndVertical();
                        GUILayout.BeginHorizontal();
                        objective.isTimer = GUILayout.Toggle(objective.isTimer, "Is Timer");
                        if(objective.isTimer)
                        {
                            EditorGUILayout.LabelField("Time: ", GUILayout.Width(100));
                            objective.time = EditorGUILayout.FloatField(objective.time);
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Objective"))
                    {
                        sequence.objectives.Add(new CollectionObjective());
                    }
                    if (GUILayout.Button("Remove Sequence"))
                    {
                        objectivesManager.sequence.sequences.Remove(sequence);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
            if (GUILayout.Button("Add Sequence"))
            {
                objectivesManager.sequence.sequences.Add(new Sequence());
            }

#if UNITY_EDITOR
            if(objectivesManager.catchReferences)
            {
                objectivesManager.CatchReferences();
                objectivesManager.catchReferences = false;
            }
#endif

            var references = objectivesManager.levelElementReferences;
            for (int i = 0; i < references.Count; i++)
            {
                references[i] = (LevelElement)EditorGUILayout.ObjectField(references[i], typeof(LevelElement), true);
            }
            if (GUILayout.Button("Add ObjectField"))
            {
                references.Add(null);
            }


            GUI.backgroundColor = defaultColor;

    

        }
    }
}