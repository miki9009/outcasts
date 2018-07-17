using Engine.Singletons;
using UnityEngine;

public class SingletonTest : Singleton
{
    public override void Initialize()
    {
        Debug.Log("Hello from Singleton test");
    }
}