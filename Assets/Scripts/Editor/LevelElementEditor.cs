using Engine;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelElement), true)]
[CanEditMultipleObjects]
public class GardenCustomEditor : Editor
{
    public void OnEnable()
    {
        var instance = (LevelElement)target;
        instance.elementID = Level.GetID();
    }
}