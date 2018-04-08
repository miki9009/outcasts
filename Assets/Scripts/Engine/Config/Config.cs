using UnityEngine;

namespace Engine.Config
{
    //[CreateAssetMenu(menuName ="Configs/Base")]
    public abstract class Config : ScriptableObject
    {
        public static T GetConfig<T>(string fileName) where T : Config
        {
            return Resources.Load<T>(fileName);
        }
    }
}