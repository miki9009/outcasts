using System.Collections.Generic;
using UnityEngine;

public static class SpawnManager 
{
    static Dictionary<string, GameObject> spawnsOriginal = new Dictionary<string, GameObject>();

    static Dictionary<string, Stack<GameObject>> spawns = new Dictionary<string, Stack<GameObject>>();

    static GameObject spawnParent;

    static bool enable;
    public static bool Enable
    {
        get
        {
            return enable;
        }
        set
        {
            enable = value;
        }
    }

    public static void AddSpawn(string name, GameObject prefab, int count)
    {
        var stack = new Stack<GameObject>();
        for (int i = 0; i < count; i++)
        {
            var obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            stack.Push(obj);
        }
        if(!spawns.ContainsKey(name))
        {
            spawns.Add(name, stack);
        }
        else
        {
            spawns[name] = stack;
        }
        if (!spawnsOriginal.ContainsKey(name))
        {
            spawnsOriginal.Add(name, prefab);
        }
        else
        {
            spawnsOriginal[name] = prefab;
        }
    }

    public static GameObject GetSpawn(string name, bool activate = true)
    {
        //Debug.Log("Spawn name Spawn Manager:" + name);
        var stack = spawns[name];
        GameObject obj;
        if(stack.Count > 0)
        {
            obj = stack.Pop();
        }
        else
        {
            Debug.LogError("Not enough spawns "+name+ ", instantiating...");
            obj = Object.Instantiate(spawnsOriginal[name]);
        }
        obj.SetActive(activate);
        return obj;
    }

    public static void Recycle(IPoolObject obj)
    {
        if (!Enable) return;
        obj.Recycle();
        obj.GameObject.transform.position = Vector3.zero;
        obj.GameObject.SetActive(false);
    }
}

public interface IPoolObject
{
    GameObject GameObject { get; }
    void Recycle();
}


