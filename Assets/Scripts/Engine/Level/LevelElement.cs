using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Engine
{
    public class LevelElement : MonoBehaviour
    {
        public Dictionary<string, object> data;
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
        }

        public virtual void OnLoad()
        {
            if (data.ContainsKey("Position"))
                transform.position = (Vector)data["Position"];
            if (data.ContainsKey("Rotation"))
                transform.rotation = (Float4)data["Rotation"];
            if (data.ContainsKey("Scale"))
                transform.localScale = (Vector)data["Scale"];
        }

    }
}