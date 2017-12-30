using UnityEngine;

public class PowerUpMagnet : PowerUp
{
    public GameObject prefab;
    [HideInInspector]public Magnet magnet;

    protected override void Start()
    {
        base.Start();
        col = GetComponent<SphereCollider>();
        GetComponent<CollectionObject>().OnCollected += (GameObject obj) =>
        {
            col.enabled = false;
            character = obj.GetComponentInParent<Character>();
            Apply();
        };
    }

    protected override void Apply()
    {
        base.Apply();
        var obj = Instantiate(prefab, character.leftLowerArm);
        magnet = obj.GetComponent<Magnet>();
        magnet.lowerArm = character.leftLowerArm;
        magnet.upperArm = character.leftUpperArm;
        magnet.use = true;
    }

    public override void Disable()
    {

        base.Disable();
        if (magnet != null)
        {
            Destroy(magnet.gameObject);
        }
    }
}