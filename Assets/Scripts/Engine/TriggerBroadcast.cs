using UnityEngine;


public class TriggerBroadcast : MonoBehaviour
{
    public event System.Action<Collider> TriggerEntered;
    public event System.Action<Collider> TriggerExit;
    void OnTriggerEnter(Collider collision)
    {
        if (TriggerEntered != null)
        {
            TriggerEntered(collision);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (TriggerExit != null)
        {
            TriggerExit(collision);
        }
    }
}