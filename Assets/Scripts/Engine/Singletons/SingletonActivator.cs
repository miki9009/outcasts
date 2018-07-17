using System.Reflection;
using UnityEngine;
using System.Linq;
using System;

namespace Engine.Singletons
{
    public class SingletonActivator : MonoBehaviour
    {
        private void Awake()
        {
            var singletons = Assembly
             .GetAssembly(typeof(Singleton))
             .GetTypes()
             .Where(t => t.IsSubclassOf(typeof(Singleton)));

            foreach (var singleton in singletons)
            {
                Activator.CreateInstance(singleton);
            }
        }
    }
}