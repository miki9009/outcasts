using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;

public class Enemy : MonoBehaviour, IDestructible, IThrowableAffected
{
    public Transform Transform { get { return transform; } set { Transform = value; } }
    PathMovement pathMovement;
    public float speed;
    public float turningSpeed;

    public Transform target;
    public Vector3 velocity;

    Rigidbody rb;
    public float horInput = 0;
    public float verInput = 0;
    [HideInInspector] public Animator anim;
    public float attackTime = 5;
    float curAttackTime = 0;
    public int attackAnimation;

    public bool inRange = false;
    public GameObject stars;

    ParticleSystem starsExplosion;
    public Vector3 targetPoint;
    public bool changePoint = true;
    float range;
    private EnemyDeath enemyDeath;
    Vector3 startPosition;
    public float guardRange;
    public bool dead = false;
    public bool isAttacking = false;

	void Start ()
    {
        startPosition = transform.position;
        pathMovement = GetComponent<PathMovement>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(RefreshPath());
        anim = GetComponentInChildren<Animator>();
        attackAnimation = Animator.StringToHash("Attack");
        starsExplosion = StaticParticles.Instance.starsExplosion;
        ChangeTargetPoint();
        enemyDeath = GetComponent<EnemyDeath>();

    }

	void Update ()
    {
        pathMovement.Inputs(out horInput, out verInput);

        var vec = rb.velocity;

        anim.SetFloat("hSpeed", vec.magnitude);

        curAttackTime += Time.deltaTime;
	}

    void Attack()
    {
        if (target == null || dead) return;
        isAttacking = true;
        anim.SetTrigger("attack");
        curAttackTime = 0;
        var dir = Math.Direction(transform.position, target.position);
        dir.y = 0;
        pathMovement.direction = dir;
        transform.rotation = Quaternion.LookRotation(dir);
        range = GetComponentInChildren<SphereCollider>().radius;
    }


    void MainMovement()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) < 4)
        {
            if (curAttackTime > attackTime)
            {
                curAttackTime = 0;
                Invoke("Attack", 1f);
            }
            if (target == null || Vector3.Distance(transform.position, target.position) > range)
            {
                inRange = false;
            }
        }
        else if (Vector3.Distance(transform.position, targetPoint) < 5)
        {
            changePoint = true;
        }

    }

    private void FixedUpdate()
    {
        var velo = rb.velocity;
        if (verInput != 0 && Mathf.Abs(velo.magnitude) < speed)
        {
            velo += transform.forward * verInput;
            rb.velocity = velo;
        }
        rb.maxAngularVelocity = 0;
        rb.maxAngularVelocity = 0;

        if (horInput != 0)
        {
            var q = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, q.y + horInput * Time.deltaTime * turningSpeed, 0);
        }
    }

    void ChangeTargetPoint()
    {
        // Debug.Log("Destination changed");
        Vector3 targetPos = Vector3.zero;
        if (Vector3.Distance(transform.position, startPosition) > guardRange)
        {
            targetPos = startPosition;
        }
        else
        {
            targetPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * Random.Range(10, 20);
        }
        changePoint = !pathMovement.RandomPoint(startPosition, guardRange, out targetPoint);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        StartCoroutine(RefreshPath());
    }

    IEnumerator RefreshPath()
    {
        Engine.Game.WaitForFrames(Random.Range(1, 25));
        var wait = new WaitForSeconds(0.5f);
        yield return wait;
        while (true)
        {
            if (target != null)
            {
                pathMovement.GetPath(target.position);
            }
            else
            {
                if (changePoint || pathMovement.noPath)
                {
                    pathMovement.noPath = false;
                    ChangeTargetPoint();
                }
                else
                {
                    pathMovement.GetPath(targetPoint);
                }
            }
            MainMovement();
            yield return wait;
        }
    }

    public void Hit()
    {
        anim.SetTrigger("hit");
        dead = true;
        enemyDeath.StartCoroutine(enemyDeath.DestroyMe());
        this.enabled = false;
        var colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        rb.useGravity = false;
        stars.SetActive(true);
        rb.velocity = Vector3.zero;
        starsExplosion.transform.position = transform.position;
        starsExplosion.Play();
    }

    public void OnHit()
    {
        Hit();
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, guardRange);
    }


#endif
}
