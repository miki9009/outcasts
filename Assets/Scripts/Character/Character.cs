using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System;

[DefaultExecutionOrder(-50)]
public class Character : MonoBehaviour
{
    public CharacterStatistics stats;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CharacterMovement movement;
    [Header("Metarig")]
    public Transform leftUpperArm;
    public Transform leftLowerArm;
    public Transform rightUpperArm;
    public Transform rightLowerArm;
    
    public IEquipment rightArmItem;
    public IEquipment leftArmobject;

    Identification identity;


    public int ID
    {
        get; private set;
    }

    public void AddRightArmItem(IRightArmItem item)
    {
        if (rightArmItem != null)
        {
            rightArmItem.Remove();
        }
        rightArmItem = item;
    }


    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        identity = new Identification();
        ID = identity.ID;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Controller.Instance.characters.Add(this);
        Controller.Instance.character = this;
    }

    private void OnDestroy()
    {
        Controller.Instance.characters.Remove(this);
    }

}
public class Identification
{
    private static int counter = 0;

    private readonly int id = -1;
    public int ID
    {
        get
        {
            return id;
        }
    }

    public Identification()
    {
        id = counter; 
        counter++;
    }
}

public interface IEquipment
{
    void Remove();
}

public interface IRightArmItem : IEquipment{ }