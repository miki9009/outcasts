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
    public PathMovement pathMovement;
    public float looseTargetDistance = 10;

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
    Vector3 nextPathPoint;
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
        path = pathMovement.GetPath(pathMovement.GetRandomPointOnNavMesh());
        pathIndex = 0;
        Debug.Log("Path points: " + path.Length);
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


    float pathUpdater = 1;
    float curTimeUpdater = 0;
    protected virtual void Update()
    {
        if (isAttacking) return;
        if(target != null)
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
        }
        if (path != null && Vector3.Distance(transform.position, path[pathIndex]) > 2)
        {

            transform.rotation = Engine.Math.RotateTowardsTopDown(transform, path[pathIndex], Time.deltaTime * 50);
            rb.velocity = transform.forward * speed;
            anim.SetFloat("hSpeed", speed);
        }
        else
        {
            if (pathIndex + 1 < path.Length)
            {
                pathIndex++;
            }
            else
            {
                path = pathMovement.GetPath(pathMovement.GetRandomPointOnNavMesh());
                pathIndex = 0;
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
