using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectionObject : MonoBehaviour
{
    public CollectionType type;
    public int val;
    public bool emmitParticles;
    public int particlesAmmount;
    public bool collected = false;

    private Character character;
    private Vector3 localScale;

    public delegate void Collect(GameObject collector);
    public event Collect OnCollected;
    public event Action<GameObject> OnLeaveTrigger;
    [HideInInspector] public Rigidbody rigid;

    protected Coroutine collectedCoroutine;

    [NonSerializedAttribute]
    public CollectionDisplay display;

    public void OnTriggerEnter(Collider other)
    {
        var obj = other.gameObject;
        if (obj.layer == 14)
        {
            if (collected) return;
            collected = true;
            if (OnCollected != null)
            {
                OnCollected(obj);
            }
            display.ShowDisplay();
            character = other.GetComponentInParent<Character>();
            int playerID = character.ID;
            CollectionManager.Instance.SetCollection(playerID, type, val);
            collectedCoroutine = StartCoroutine(Collected());
            if (emmitParticles)
            {
                CollectionManager.Instance.EmmitParticles(type, transform.position + Vector3.up, particlesAmmount);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (OnLeaveTrigger != null)
        {
            OnLeaveTrigger(other.gameObject);
        }
    }

    private void Awake()
    {
        localScale = transform.localScale;
        if (!GameManager.LevelLoaded)
        {
            GameManager.OnLevelLoaded += AssignDisplayOnLoad;
        }
        else
        {
            display = CollectionDisplayManager.Instance.AssignDisplayObject(type);
        }
    }

    protected virtual void Start()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    void AssignDisplayOnLoad()
    {
        try
        {
            display = CollectionDisplayManager.Instance.AssignDisplayObject(type);
        }
        catch(Exception ex)
        {
            enabled = false;
            Debug.Log(ex.Message);
        }
    }

    private void Update()
    {
        transform.rotation = rotation;
    }

    protected virtual IEnumerator Collected()
    {
        while(transform.localScale.x > 0.05)
        {
            transform.localScale /= 1.05f;
            yield return null;
        }

        yield return new WaitForSeconds(deactivationTime);
        Deactivate();
        collectedCoroutine = null;
        yield return null;
    }

    public float deactivationTime = 0.1f;
    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public static Vector3 eulers = Vector3.zero;
    public static Quaternion rotation = Quaternion.identity;

    public void BackToCollection()
    {
        if (character == null)
        {
            Debug.LogError("Character is null");
            return;
        }
        gameObject.SetActive(true);
        if (collectedCoroutine != null)
        {
            StopCoroutine(collectedCoroutine);
        }
        collected = true;
        GetComponent<Collider>().enabled = true;
        transform.position = character.transform.position;
        OnLeaveTrigger += SetCollectionObjectActive;
        int collection = CollectionManager.Instance.GetCollection(character.ID, type);
        CollectionManager.Instance.SetCollection(character.ID, type, collection - val);
        transform.localScale = localScale;
    }

    void SetCollectionObjectActive(GameObject gameObject)
    {
        var character = gameObject.GetComponentInParent<Character>();
        if (character != null && character == this.character)
        {
            collected = false;
            OnLeaveTrigger -= SetCollectionObjectActive;
        }
    }
}

public enum CollectionType
{
    Coin = 0,
    Emmerald = 1,
    Health = 2,
    Clock = 3,
    Magnet = 4,
    Weapon = 5,
    Throwable = 6,
    Invincibility = 7,
    KeyGold = 8,
    KeySilver = 9,
    KeyBronze = 10
}

