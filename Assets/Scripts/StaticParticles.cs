using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticParticles : MonoBehaviour
{
    public ParticleSystem crateExploded;
    public ParticleSystem smokeExplosion;
    public ParticleSystem starsExplosion;
    public ParticleSystem hitParticles;

    public static StaticParticles Instance
    {
        get;private set;
    }
    private void Awake()
    {
        Instance = this;
    }

    public static void PlayHitParticles(Vector3 position)
    {
        if (Instance == null) return;
        Instance.hitParticles.transform.position = position;
        Instance.hitParticles.Play();
    }

}
