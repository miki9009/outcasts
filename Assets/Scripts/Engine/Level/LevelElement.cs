using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Engine
{
    public class LevelElement : MonoBehaviour
    {
        public int elementID = -1;
        public TargetPointerActivator ArrowActivator { get; set; }
        public bool arrowTarget;
        public Dictionary<string, object> data;

        int[] hierarchy;

        public string GetName()
        {

#if UNITY_EDITOR
            return PrefabUtility.GetPrefabParent(gameObject).name;
#endif
            Debug.LogError("Path not found of Prefab doesn't exist, or Trying to get path during gameplay.");
            return "";
        }

        public virtual void OnSave()
        {
            data = new Dictionary<string, object>();
            Vector pos = transform.position;
            data.Add("Position", pos);
            Float4 rotation = transform.rotation;
            data.Add("Rotation", rotation);
            Vector scale = transform.localScale;
            data.Add("Scale", scale);
            data.Add("ArrowTarget", arrowTarget);
            data.Add("ID", elementID);

            var childCount = transform.childCount;
            List<int> children = new List<int>();
            for (int i = 0; i < childCount; i++)
            {
                var e = transform.GetChild(i).GetComponent<LevelElement>();
                if (e != null)
                    children.Add(e.elementID);
            }
            hierarchy = children.ToArray();

            data.Add("Hierarchy", hierarchy);
        }

        public virtual void OnLoad()
        {
            GameManager.LevelClear += OnLevelClear;

            if (data.ContainsKey("Position"))
                transform.position = (Vector)data["Position"];
            if (data.ContainsKey("Rotation"))
                transform.rotation = (Float4)data["Rotation"];
            if (data.ContainsKey("Scale"))
                transform.localScale = (Vector)data["Scale"];
            if (data.ContainsKey("ArrowTarget"))
                arrowTarget = (bool)data["ArrowTarget"];
            if(data.ContainsKey("ID"))
            {
                elementID = (int)data["ID"];
            }
            if (data.ContainsKey("Hierarchy"))
            {
                hierarchy = (int[])data["Hierarchy"];
            }

#if UNITY_EDITOR
            name += " (" + elementID +")";
#endif


            Character.CharacterCreated += CheckTargetPointer;
        }

        public void BuildHierarchy()
        {
            for (int i = 0; i < hierarchy.Length; i++)
            {
                if (Level.loadedElements.ContainsKey(hierarchy[i]))
                    Level.loadedElements[hierarchy[i]].transform.SetParent(transform);
            }
        }

        void CheckTargetPointer(Character character)
        {
            if (arrowTarget)
            {
                if (character != null)
                    TargetPointerManager.PrepareArrow(character.transform, transform);
            }
        }

        protected virtual void OnLevelClear()
        {
            if (gameObject == null) return;
            Character.CharacterCreated -= CheckTargetPointer;
            GameManager.LevelClear -= OnLevelClear;
            Destroy(gameObject);
        }
    }
}