using UnityEngine;


public class CollisionBroadcast : MonoBehaviour
{
    public event System.Action<Collision> CollisionEntered;
    void OnCollisionEnter(Collision collision)
    {
 //       Debug.Log("Collision Registered");
        if (CollisionEntered != null)
        {
            CollisionEntered(collision);
        }
    }
}