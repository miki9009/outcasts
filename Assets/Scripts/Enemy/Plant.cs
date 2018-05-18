using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using UnityEngine;

public class Plant : MonoBehaviour, Engine.IStateAnimator, IDestructible, IThrowableAffected
{
    public float turnSpeed;
    public float timeBetweenAttacks = 0.75f;
    Transform target;
    Animator anim;
    CollisionBroadcast collisionBroadcast;
    bool canAttack = true;
    int animationHash = Animator.StringToHash("Attack");
    SphereCollider sphere;
    bool dead;
    EnemyDeath enemyDeath;
    ParticleSystem starsExplosion;

    public AnimatorBehaviour AnimatorBehaviour
    {
        get;set;
    }

    public Transform Transform
    {
        get { return transform; }
    }

    public void StateAnimatorInitialized()
    {
        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == animationHash)
            {
                Invoke("PerformAttack", timeBetweenAttacks);
            }
        };
    }

    void PerformAttack()
    {
        canAttack = true;
        Debug.Log("Triggered Invoke");
        if (target != null && Vector3.Distance(transform.position, target.position) < sphere.radius)
        {
            anim.Play("Attack");
        }
    }


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        enemyDeath = GetComponent<EnemyDeath>();
        collisionBroadcast = GetComponentInChildren<CollisionBroadcast>();
        sphere = GetComponent<SphereCollider>();
        collisionBroadcast.CollisionEntered += Attack;
    }

    void Attack(Collision collision)
    {
        if (dead) return;
        if (collision.gameObject.layer == Layers.Character)
        {
            var characterMovement = collision.gameObject.GetComponent<CharacterMovement>();
            if (characterMovement != null && canAttack)
            {
                canAttack = false;
                characterMovement.Hit();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dead) return;
        if (other.gameObject.layer == Layers.Character)
        {
            target = other.transform;
            anim.Play("Attack");
        }
    }


    public void Start()
    {
        turnSpeed = turnSpeed * Time.deltaTime;
        starsExplosion = StaticParticles.Instance.starsExplosion;
    }



    private void OnEnable()
    {
        var characters = Controller.Instance.characters;
        var dis1 = Mathf.Infinity;
        float dis2;
        foreach (var character in characters)
        {
            dis2 = Vector3.Distance(character.transform.position, transform.position);
            if (dis2 < dis1)
            {
                Debug.Log(character.name);
                target = character.transform;
                dis1 = dis2;
            }
        }
    }

    public void Hit()
    {
        starsExplosion.transform.position = transform.position;
        starsExplosion.Play();
        collisionBroadcast.CollisionEntered -= Attack;
        CancelInvoke();
        Debug.Log("Plant Hit");
        //Invoke("PlayDie", 0.1f);
        dead = true;
        sphere.enabled = false;
        enemyDeath.StartCoroutine(enemyDeath.DestroyMe());
        PlayDie();
        this.enabled = false;

    }

    void PlayDie()
    {
        Debug.Log("Played Dead");
        anim.Play("Die");
        this.enabled = false;
    }



    private void Update()
    {
        if (target != null && !dead)
        transform.rotation = Engine.Math.RotateTowardsTopDown(transform, target.position, turnSpeed);
    }

    public void OnHit()
    {
        Hit();
    }
}