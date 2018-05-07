using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == Character.GetLocalPlayer().transform)
        {
            Controller.Instance.gameCamera.GetComponent<GameCamera>().target = null;
        }
    }
}