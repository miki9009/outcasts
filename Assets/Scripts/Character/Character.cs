using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using System;

[DefaultExecutionOrder(-50)]
public class Character : MonoBehaviour
{
    public PhotonView networking;
    public CharacterStatistics stats;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CharacterMovement movement;
    [Header("Metarig")]
    public Transform leftUpperArm;
    public Transform leftLowerArm;
    public Transform rightUpperArm;
    public Transform rightLowerArm;
    public SkinnedMeshRenderer bodyMeshRenderer;
    
    public IEquipment rightArmItem;
    public IEquipment leftArmItem;
    public bool isDead { get; set; }


    Identification identity;

    public static event Action<Character> CharacterCreated;


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

    public void CreateLocalPlayer()
    {
        localPlayer = this;
        Controller.Instance.character = this;
        Controller.Instance.gameCamera.SetTarget(transform);
        stats = CharacterSettingsModule.Statistics;
        ChangeArmor(stats.armorType);
        CharacterCreated?.Invoke(this);
    }


    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        identity = new Identification();

        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    void Start()
    {
        Controller.Instance.characters.Add(this);
        if (movement.GetType() == typeof(CharacterMovementPlayer))
        {
            if(networking.isMine || !PhotonManager.IsMultiplayer)
            {
                CreateLocalPlayer();
            }
            else if(PhotonManager.IsMultiplayer)
            {
                DisableOnNotMine();
            }
            ID = networking.viewID;

        }
        else
        {
            ID = identity.ID;
        }

    }

    private void OnDestroy()
    {
        Controller.Instance.characters.Remove(this);
    }

    void DisableOnNotMine()
    {
        movement.isRemoteControl = true;
        rb.useGravity = false;
    }

    public void ChangeArmor(string armorID)
    {
        var config = ConfigsManager.GetConfig<CharacterConfig>();
        bodyMeshRenderer.sharedMesh = config.GetMesh(armorID);
    }




}
public class Identification
{
    private static int counter = 0;

    private readonly int id = 1000;
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