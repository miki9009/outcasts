using UnityEngine;

public class Weapon : MonoBehaviour, IRightArmItem
{

    public GameObject collectionPrefab;
    public ParticleSystem attackParticles;
    [HideInInspector] public Character character;
    public CollectionObject collectionObject;

    private void Start()
    {
        character.AddRightArmItem(this);
        character = transform.root.GetComponent<Character>();
        character.movement.MeleeAttack += Attack;
    }

    void Attack()
    {
        attackParticles.Play();
    }


    public void Remove()
    {
        Engine.PoolingObject.Recycle(gameObject.GetName(), gameObject, () =>
        {
            if (character != null)
            character.movement.MeleeAttack -= Attack;
         });
        Debug.Log("Remove");
        collectionObject.BackToCollection();
    }

    public void Clear()
    {
        Remove();
    }


    


}

