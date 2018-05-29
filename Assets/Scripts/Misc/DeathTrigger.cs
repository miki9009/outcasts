using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponentInParent<Character>();
        if (character != null)
        {
            Controller.Instance.gameCamera.GetComponent<GameCamera>().target = null;
            character.movement.DieNonAnimation();
            character.movement.characterHealth.RemoveHealth(character.stats.health);
        }
        else
        {
            Debug.LogError("This should trigger only when character collides. Check Layers that Activations collide with");
        }
    }
}