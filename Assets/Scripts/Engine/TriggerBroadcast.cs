using UnityEngine;


public class TriggerBroadcast : MonoBehaviour
{
    public event System.Action<Collider> TriggerEntered;
    public event System.Action<Collider> TriggerExit;
    void OnTriggerEnter(Collider collider)
    {
        if (TriggerEntered != null)
        {
            TriggerEntered(collider);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (TriggerExit != null)
        {
            TriggerExit(collider);
        }
    }
}