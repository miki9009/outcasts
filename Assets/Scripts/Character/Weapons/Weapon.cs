using UnityEngine;

public class Weapon : MonoBehaviour, IEquipment
{
    public ParticleSystem attackParticles;
    [HideInInspector] public CharacterMovement characterMovement;

    private void Start()
    {
        characterMovement = transform.root.GetComponent<CharacterMovement>();
        characterMovement.MeleeAttack += Attack;
    }

    void Attack()
    {
        attackParticles.Play();
    }


    public void Remove()
    {
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {
            if (characterMovement != null)
            characterMovement.MeleeAttack -= Attack;
         });
    }
}

public interface IEquipment
{
    void Remove();
}