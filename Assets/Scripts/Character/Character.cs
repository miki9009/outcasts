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
    public IEquipment leftArmItem;

    Identification identity;

    


    public int ID
    {
        get; private set;
    }

    static Character localPlayer;
    public static Character GetLocalPlayer()
    {
        return localPlayer;
    }

    public void AddRightArmItem(IRightArmItem item)
    {
        if (rightArmItem != null)
        {
            rightArmItem.Remove();
        }
        rightArmItem = item;
    }

    public void AddItem(IEquipment equipment)
    {       
        if (equipment != null)
        {
            var types = equipment.GetType().GetInterfaces();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == typeof(IRightArmItem))
                {
                    if (rightArmItem != null)
                    {
                        rightArmItem.Remove();
                        //Debug.Log("Right Hand item removed");
                    }
                    //Debug.Log("Right Hand item equipped");
                    rightArmItem = equipment;
                    break;
                }
                else if (types[i] == typeof(ILefttArmItem))
                {
                    if (leftArmItem != null)
                    {
                        leftArmItem.Remove();
                        //Debug.Log("Left Hand item removed");
                    }
                    //Debug.Log("Left Hand item equipped");
                    leftArmItem = equipment;
                    break;
                }
            }
            equipment.Apply();
        }
    }


    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        identity = new Identification();
        ID = identity.ID;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        localPlayer = this;
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
    CollectionObject CollectionObject { get; set; }
    void Apply();
    void Remove();
    void BackToCollection();
}

public interface IRightArmItem : IEquipment{ }
public interface ILefttArmItem : IEquipment { }