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
        public bool activeAndEnabled = true;
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
            data.Add("ActiveAndEnabled", activeAndEnabled);

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

            transform.position = (Vector)data["Position"];
            transform.rotation = (Float4)data["Rotation"];
            transform.localScale = (Vector)data["Scale"];
            arrowTarget = (bool)data["ArrowTarget"];
            elementID = (int)data["ID"];
            hierarchy = (int[])data["Hierarchy"];
            if (data.ContainsKey("ActiveAndEnabled"))
            {
                activeAndEnabled = (bool)data["ActiveAndEnabled"];
            }

#if UNITY_EDITOR
            name += " (" + elementID + ")";
#endif


            Character.CharacterCreated += CheckTargetPointer;
            Character.CharacterCreated += CheckIfActive;
        }

        void CheckIfActive(Character character)
        {
            if (Application.isPlaying)
                gameObject.SetActive(activeAndEnabled);
            Character.CharacterCreated -= CheckIfActive;
        }

        public void BuildHierarchy()
        {
            if (hierarchy == null) return;
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


#if UNITY_EDITOR
        Bounds bounds;
        bool boundsSearched;
        private void OnDrawGizmos()
        {
            if(!activeAndEnabled)
            {
                if(!boundsSearched)
                {
                    var collid = GetComponent<Collider>();
                    if (collid != null)
                        bounds = collid.bounds;
                    else
                        collid = GetComponentInChildren<Collider>();
                    {
                        if (collid != null)
                            bounds = collid.bounds;
                    }
                    boundsSearched = true;
                }
                var col = Color.gray;
                col.a = 0.6f;
                Gizmos.color = col;
                Gizmos.DrawSphere(transform.position, bounds.extents.x + 1f);
            }

        }
#endif
    }
}