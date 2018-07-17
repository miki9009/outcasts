using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Enemy : MonoBehaviour, IDestructible, IThrowableAffected, IStateAnimator, IPoolObject
{
    [SpawnsNames]
    public string spawnName;
    public float offset = 10;
    public bool randomStartRotation;
    public float speed = 3;

    int right = 1;
    int left = -1;
    int dir;
    public int action = 0;
    //0-move
    //1-follow
    //2-idle

    Rigidbody rb;
    protected Vector3 startPos;

    float idle = 1;
    public bool isAttacking;

    public Transform Transform { get { return transform; } set { Transform = value; } }

    public AnimatorBehaviour AnimatorBehaviour
    {
        get;set;
    }

    public string SpawnName
    {
        get
        {
            return spawnName;
        }
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    Collider[] colliders;

    [System.NonSerialized] public Transform target;

    [System.NonSerialized] public Animator anim;
    public float attackTime = 1;

    public GameObject stars;

    ParticleSystem starsExplosion;
    private EnemyDeath enemyDeath;
    Vector3 startPosition;
    public bool dead = false;
    protected bool canAttack = false;

    void Awake()
    {
        colliders = GetComponents<Collider>();
    }

	protected virtual void Start ()
    {
        canAttack = true;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        starsExplosion = StaticParticles.Instance.starsExplosion;
        enemyDeath = GetComponent<EnemyDeath>();
        if (randomStartRotation)
        {
            if (Engine.Math.Probability(0.5f))
            {
                dir = right;
                transform.rotation = Quaternion.LookRotation(Vector3.right);
            }
            else
            {
                dir = left;
                transform.rotation = Quaternion.LookRotation(Vector3.left);
            }
        }
        startPos = transform.position;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != Layers.Character)
        {
            dir = dir == left ? right : left;
            transform.rotation = Quaternion.LookRotation(dir == right ? Vector3.right : Vector3.left);
        }
        else
        {
            target = other.transform;
            if(canAttack)
                Attack();
        }
    }

    protected virtual void Update()
    {
        if (canAttack)
        {
            var vec = rb.velocity;
            anim.SetFloat("hSpeed", Mathf.Abs(vec.x));
            if (action == 0)
            {
                rb.velocity = new Vector3(speed * dir, rb.velocity.y, 0);
                if (dir == left)
                {
                    if (transform.position.x < startPos.x - offset)
                    {
                        action = 2;
                        dir = right;
                        rb.velocity = Vector3.zero;
                    }
                }
                else
                {
                    if (transform.position.x > startPos.x + offset)
                    {
                        action = 2;
                        dir = left;
                        rb.velocity = Vector3.zero;
                    }
                }
            }
            else if (action == 1)
            {
                if (target != null)
                {
                    rb.velocity = new Vector3(speed * dir, rb.velocity.y, 0);
                    if (target.position.x > transform.position.x)
                    {
                        dir = right;
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else
                    {
                        dir = left;
                        transform.rotation = Quaternion.LookRotation(Vector3.left);
                    }
                }
                else
                {
                    action = 0;
                }
            }
            else
            {
                if (idle > 0)
                {
                    idle -= Time.deltaTime;
                }
                else
                {
                    if (dir == right)
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    else
                        transform.rotation = Quaternion.LookRotation(Vector3.left);
                    action = 0;
                    idle = 1;
                }
            }
        }
    }

    protected void Attack()
    {
        Debug.Log("Attack");
        canAttack = false;
        if (target == null || dead) return;
        anim.Play("Attack");
        Invoke("CanAttack", attackTime);
    }

    void CanAttack()
    {
        canAttack = true;
        if(!dead)
            anim.Play("Idle");
    }


    public void Hit(CharacterMovement character)
    {
        anim.SetTrigger("hit");
        dead = true;
        enemyDeath.StartCoroutine(enemyDeath.DestroyMe());
        enabled = false;
        if (colliders == null)
        {
            colliders = GetComponentsInChildren<Collider>();
            Debug.Log("Colliders were null could be a problem in Infinite mode.");
        }
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        rb.useGravity = false;
        //stars.SetActive(true);
        rb.velocity = Vector3.zero;
        starsExplosion.transform.position = transform.position;
        //starsExplosion.Play();
    }

    public virtual void Recycle()
    {
        dead = false;
        enemyDeath.StopAllCoroutines();
        anim.Play("Idle");
        enabled = true;
        if(colliders == null)
        {
            colliders = GetComponentsInChildren<Collider>();
            Debug.Log("Colliders were null could be a problem in Infinite mode.");
        }
        if (colliders != null)
        {
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
        }
        rb.useGravity = true;
        stars.SetActive(false);
        rb.velocity = Vector3.zero;
        starsExplosion.transform.position = transform.position;
        starsExplosion.Stop();
    }

    public void OnHit()
    {
        Hit(null);
    }


    protected int attackHashName = Animator.StringToHash("Attack");

    public virtual void StateAnimatorInitialized()
    {
        AnimatorBehaviour.StateEnter += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == attackHashName)
            {
                isAttacking = true;
            }
        };

        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == attackHashName)
            {
                isAttacking = false;
            }
        };
    }

#if UNITY_EDITOR
    public Color gizmoColor = Color.blue;
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position + Vector3.one, new Vector3(offset * 2, 2, 2));
    }


#endif
}
