using UnityEngine;

public class ThrowableObject : MonoBehaviour, IRightArmItem
{
    private IThrowable throwable;
    private Character character;
    public float force;
    Rigidbody rb;
    Vector3 dir;
    public float upFactor;
    [HideInInspector]
    public CollectionObject collectionObject;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Initialize(Character character)
    {
        character.AddRightArmItem(this);
        this.character = character;
        throwable = (IThrowable)character.movement;
        throwable.ThrowObject = this;
        throwable.Thrown += OnThrow;
    }

    void OnThrow(IThrowable throwable, Vector3 direction)
    {
        if (character == null) return;
        dir = direction;
        Invoke("PerformThrow", 0.5f);
        throwable.Thrown -= OnThrow;
        character.rightArmItem = null;
    }


    void PerformThrow()
    {
        throwable.ThrowObject = null;
        transform.parent = null;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity =  dir * force;
    }

    public void Remove()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        transform.parent = null;
        gameObject.SetActive(false);
        throwable.Thrown -= OnThrow;
        collectionObject.BackToCollection();
    }

    private void OnDestroy()
    {
        if (throwable != null)
        {
            throwable.Thrown -= OnThrow;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == Layers.Enemy)
        {
            var affected = other.gameObject.GetComponent<IThrowableAffected>();
            if (affected != null)
            {
                affected.OnHit();
            }
        }
    }

}

/// <summary>
/// Defines if object can be hit by Throwable object
/// </summary>
public interface IThrowableAffected
{
    void OnHit();
}
