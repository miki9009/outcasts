using UnityEngine;

public class CrateJumpMetal : MonoBehaviour
{
    public float maxForce = 40;
    public float squashed = 0;
    public float squashedForceBack = 1f;
    public SkinnedMeshRenderer mesh;
    bool addForce = true;

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (addForce && rb != null)
        {
            var dir = rb.velocity.normalized;
            addForce = false;
            other.attachedRigidbody.velocity  = /*new Vector3(dir.x,-dir.y,dir.z)*/ Vector3.up * Mathf.Clamp(Mathf.Pow(other.attachedRigidbody.velocity.magnitude,1.4f), 1, maxForce);
            squashed = rb.velocity.magnitude / maxForce;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        addForce = true;
    }

    private void Update()
    {
        if (squashed > 0)
        {
            mesh.SetBlendShapeWeight(0, squashed * 100);
            squashed -= squashedForceBack * Time.deltaTime;
        }
    }
}