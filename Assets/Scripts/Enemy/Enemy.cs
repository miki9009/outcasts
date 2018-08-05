using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Enemy : MonoBehaviour, IDestructible, IThrowableAffected, IStateAnimator, IPoolObject
{
    [SpawnsNames]
    public string spawnName;
    public LayerMask collisionLayer;
    public float offset = 10;
    public bool randomStartRotation;
    public float speed = 3;
    public PathMovement pathMovement;
    public float looseTargetDistance = 10;
    public float patrolDistance = 10;
    public float attackDistance = 3;

    [Label("Wait time before next path")]
    public float waitTime = 3;

    public int action = 0;

    protected Rigidbody rb;
    protected Vector3 startPos;
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

    /*[System.NonSerialized] */public Transform target;

    [System.NonSerialized] public Animator anim;
    public float attackTime = 1;

    public GameObject stars;

    ParticleSystem starsExplosion;
    private EnemyDeath enemyDeath;
    Vector3 startPosition;
    public bool dead = false;
    protected bool canAttack = false;
    int pathIndex = 0;
    Vector3[] path;

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
        startPos = transform.position;
        Vector3 destination = startPos;
        if(pathMovement.RandomPoint(startPos, patrolDistance, out destination))
        {
            path = pathMovement.GetPath(destination);
        }
        path = pathMovement.GetPath(pathMovement.GetRandomPointOnNavMesh());
        pathIndex = 0;
 //       Debug.Log("Path points: " + path.Length);
    }

    //protected virtual void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.layer == Layers.Character)
    //    {
    //transform.rotation = Quaternion.LookRotation(Vector.Direction(transform.position, other.gameObject.transform.position));
    //        target = other.transform;
    //        if(canAttack)
    //            Attack();
    //    }
    //}


    protected float waitTimeCur;
    float pathUpdater = 1;
    float curTimeUpdater = 0;
    Vector3 prevPos;
    bool isColliding = true;
    float collidingTime;
    protected virtual void Update()
    {
        anim.SetFloat("hSpeed", rb.velocity.magnitude);
        if (target != null)
        {
            if (pathUpdater > curTimeUpdater)
            {
                curTimeUpdater += Time.deltaTime;
            }
            else
            {
                path = pathMovement.GetPath(target.position);
                pathIndex = 0;
                curTimeUpdater = 0;
            }
            var dis = Vector3.Distance(transform.position, target.position);
            if (dis > looseTargetDistance)
            {
                target = null;
            }
            else if(dis < attackDistance)
            {
                rb.velocity = Vector3.zero;
                transform.rotation = Quaternion.LookRotation(Vector.Direction(transform.position, target.position));
                if (canAttack)
                {
                    Attack();
                }
            }
        }
        if (!isAttacking)
        {
            if (target == null && waitTimeCur > 0)
            {
                waitTimeCur -= Time.deltaTime;
                if(isColliding)
                {
                    rb.velocity = Vector3.zero;
                }
                if(collidingTime > 0)
                {
                    collidingTime -= Time.deltaTime;
                }
                else
                {
                    isColliding = Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), 3, collisionLayer.value);
                    collidingTime = 1;
                }

                return;
            }
            if (path != null && Vector3.Distance(transform.position, path[pathIndex]) > 2)
            {
                //transform.rotation = Math.RotateTowardsTopDown(transform, path[pathIndex], Time.deltaTime * 5);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector.Direction(transform.position, path[pathIndex])), 0.25f);
                float y = rb.velocity.y;
                Vector3 velo = transform.forward * speed;
                velo.y = y;
                rb.velocity = velo;
            }
            else
            {
                if (pathIndex + 1 < path.Length)
                {
                    pathIndex++;
                }
                else
                {
                    Vector3 destination;
                    if (pathMovement.RandomPoint(startPos, patrolDistance, out destination))
                    {
                        Debug.Log("Next path");
                        rb.velocity = Vector3.zero;
                        path = pathMovement.GetPath(destination);
                        waitTimeCur = waitTime;
                        pathIndex = 0;
                    }
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
        CollectionManager.Instance.SetCollection(character.character.ID, CollectionType.KillEnemy, 1);
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
        if (path != null && path.Length > 0)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, path[pathIndex]);
        }
    }


#endif
}
