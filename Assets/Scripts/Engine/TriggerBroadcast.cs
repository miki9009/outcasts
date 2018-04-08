using UnityEngine;


public class TriggerBroadcast : MonoBehaviour
{
    public event System.Action<Collider> TriggerEntered;
    void OnTriggerEnter(Collider collision)
    {
        if (TriggerEntered != null)
        {
            TriggerEntered(collision);
        }
    }
}