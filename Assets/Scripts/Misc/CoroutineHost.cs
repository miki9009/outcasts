using System.Collections;
using UnityEngine;

public class CoroutineHost : MonoBehaviour
{
    private static CoroutineHost instance; 
    public static void Start(IEnumerator coroutine)
    {
        instance.StartCoroutine(coroutine);
    }

    public static void Stop(IEnumerator coroutine)
    {
        instance.StopCoroutine(coroutine);
    }

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        instance = this;
    }
}