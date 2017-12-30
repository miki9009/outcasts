using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticParticles : MonoBehaviour
{
    public ParticleSystem crateExploded;
    public ParticleSystem smokeExplosion;
    public ParticleSystem starsExplosion;

    public static StaticParticles Instance
    {
        get;private set;
    }
    private void Awake()
    {
        Instance = this;
    }

}
