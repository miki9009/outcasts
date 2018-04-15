using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Crate : MonoBehaviour, IDestructible
{
    public Transform Transform { get { return transform; } set { Transform = value; } }
    public GameObject collection;
    public int collectionAmmount;
    public LayerMask layer;

    ParticleSystem crateExplosion;
    Renderer rend;
    BoxCollider boxCol;
    SphereCollider sCol;

    private void Start()
    {
        crateExplosion = StaticParticles.Instance.crateExploded;
        rend = GetComponent<Renderer>();
        boxCol = GetComponent<BoxCollider>();
        sCol = GetComponent<SphereCollider>();
        for (int i = 0; i < collectionAmmount; i++)
        {
            PoolingObject.AddSpawn(collection.name, Instantiate(collection));
        }
    }

    readonly Vector3[] dirs = {
        new Vector3(0,-0.5f,0),
        new Vector3(1, -0.5f, 1),
         new Vector3(1,-0.5f,0),
          new Vector3(1,-0.5f,-1),
           new Vector3(0,-0.5f,1),
            new Vector3(-1,-0.5f,1),
             new Vector3(-1,-0.5f,0),
              new Vector3(-1,-0.5f,1)
    };
    const int LENGTH = 8;
    public IEnumerator CollectionCreate()
    {
        if (collectionAmmount > LENGTH)
        {
            collectionAmmount = LENGTH;
        }
        var coins = new GameObject[collectionAmmount];
        for (int i = 0; i < collectionAmmount; i++)
        {
            coins[i] = PoolingObject.GetSpawn(collection.name, transform.position, Quaternion.identity);
            coins[i].GetComponent<Collection>().enabled = false;
            yield return null;
        }
        int steps = 15;
        for (int i = 0; i < steps; i++)
        {
            for (int j = 0; j < coins.Length; j++)
            {
                if (coins[j] != null)
                {
                     coins[j].transform.position += dirs[j] * 0.1f;
                }
            }
            yield return null;
        }
        for (int i = 0; i < coins.Length; i++)
        {
            if (coins[i] != null)
            coins[i].GetComponent<Collection>().enabled = true;
        }
        gameObject.SetActive(false);
        yield return null;
    }

    public void Hit()
    {
        crateExplosion.transform.position = transform.position;
        crateExplosion.Play();
        StartCoroutine(CollectionCreate());
        rend.enabled = false;
        boxCol.enabled = false;
        sCol.enabled = false;
    }
}
