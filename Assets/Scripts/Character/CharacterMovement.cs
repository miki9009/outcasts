using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using Engine.GUI;

public class CharacterMovement : MonoBehaviour, IThrowable, IStateAnimator
{

    public bool onGound = true;
    public ParticleSystem smoke2;
    public Transform model;
    public LayerMask enemyLayer;
    public ParticleSystem attackParticles;
    protected Character character;
    protected CharacterStatistics stats;
    protected Animator anim;

    private BoxCollider jumpCollider;

    private ParticleSystem smoke;

    float verInput = 0;
    float horInput = 0;
    float jumpInput = 0;
    float shoot = 0;
    Rigidbody rb;
    Vector3 curPos;
    public bool attack;

    public Vector3 velocity;

    float timeLastJump = 0;
    bool jumpReleased = true;
    bool doubleJump = false;
    float modelZ = 0;
    ParticleSystem smokeExplosion;
    ParticleSystem starsExplosion;
    [HideInInspector] public CharacterHealth characterHealth;
    public Action MeleeAttack;
    public event Action<IThrowable, Vector3> Thrown;
    public ThrowableObject ThrowObject { get; set; }
    int throwAnimationHash;

    public AnimatorBehaviour AnimatorBehaviour
    {
        get;set;
    }



    private void Awake()
    {
        character = GetComponent<Character>();
        jumpCollider = GetComponent<BoxCollider>();
        smoke = GetComponentInChildren<ParticleSystem>();
    }

    Button btnLeft;
    Button btnRight;
    Button btnJump;
    Button btnAttack;
    Button btnForward;
    bool buttonsInitialized = true;
    bool canMove = true;

    // Use this for initialization
    void Start ()
    {
        curPos = transform.position;
        stats = character.stats;
        rb = character.rb;
        anim = character.anim;
        InitButtons();
        smokeExplosion = StaticParticles.Instance.smokeExplosion;
        starsExplosion = StaticParticles.Instance.starsExplosion;
        
    }

    void InitButtons()
    {
        try
        {
            btnLeft = GameGUI.GetButtonByName("ButtonLeft");
            btnRight = GameGUI.GetButtonByName("ButtonRight");
            btnJump = GameGUI.GetButtonByName("ButtonJump");
            btnForward = GameGUI.GetButtonByName("ButtonForward");
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnAttack.OnTapPressed.AddListener(Attack);
        }
        catch
        {
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement");
            buttonsInitialized = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        onGound = false;
        smoke.Stop();
    }

    public void Die()
    {
        anim.Play("Die");
        enabled = false;
        StopAllCoroutines();
    }

    bool resetAttack = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != 12 && other.gameObject.layer != 13)
        {
            if (!smoke.isPlaying)
            {
                smoke.Play();
            }
            onGound = true;
            attack = false;
            anim.SetBool("attackStay", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            var enemy = other.transform.root.gameObject.GetComponent<Enemy>();
            if (!enemy.dead && enemy.isAttacking)
            {
                enemy.isAttacking = false;
                Hit(enemy);
            }
        }
        else
        {
            onGound = true;
            smoke2.Emit(15);
        }
    }


    public void Hit(Enemy enemy = null, int hp = 1)
    {
        if (stats.health <= 0) return;
        hp = Mathf.Clamp(hp,1,stats.health);

        for (int i = 0; i < hp; i++)
        {
            characterHealth.RemoveHealth(stats.health -i-1);
        }

        stats.health -= hp;

        if (stats.health > 0)
        {
            anim.SetTrigger("hit");
            rb.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
        }
        else
        {
            if (enemy != null)
            {
                enemy.target = null;
            }
            Die();
        }
        starsExplosion.transform.position = transform.position;
        starsExplosion.Play();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        Move();
        Rotation();
	}

    private void Update()
    {
        curPos = transform.position;
        velocity = rb.velocity;
        Inputs();
        Jump();
        if (attack)
        {
            attack = false;
            AttackCollision();
        }
    }


    void Shoot()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Attack();
        }
#endif
    }

    void Jump()
    {
        if (timeLastJump < 0.7f && jumpReleased)
        {
            jumpReleased = false;
            if (jumpInput > 0 && (Physics.Raycast(transform.position, Vector3.down,1) || doubleJump))
            {
                timeLastJump = 1;
                rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
                if (onGound)
                {
                    onGound = false;
                    doubleJump = true;
                }
                else
                {
                    doubleJump = false;
                }
            }
        }
        else
        {
            timeLastJump -= Time.deltaTime;
        }
        if (jumpInput == 0)
        {
            jumpReleased = true;
        }
    }

    void Inputs()
    {
        verInput = 0;
        horInput = 0;
        jumpInput = 0;
        if (canMove)
        {
            if (buttonsInitialized)
            {
                if (btnForward.isTouched) verInput = 1;

                if (btnRight.isTouched) horInput = 1;
                if (btnLeft.isTouched) horInput = -1;

                if (btnJump.isTouched) jumpInput = 1;
            }

#if UNITY_EDITOR
            if (verInput == 0)
            {
                verInput = Input.GetAxisRaw("Vertical");
            }

            if (horInput == 0)
            {
                horInput = Input.GetAxisRaw("Horizontal");
            }

            if (jumpInput == 0)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    jumpInput = 1;
                }
            }
#endif
        }

        anim.SetFloat("vSpeed", velocity.y);
        anim.SetBool("onGround", onGound);
    }


    void Move()
    {
        var velo = rb.velocity;
        anim.SetFloat("hSpeed", velo.magnitude);
        if (verInput != 0 && Mathf.Abs(velo.magnitude) < stats.runSpeed)
        { 
            velo += transform.forward * verInput;
            rb.velocity = velo;
            rb.rotation = transform.rotation;
        }
         rb.maxAngularVelocity = 0;
        
    }

    void Rotation()
    {
        if (horInput != 0)
        {
            var q = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, q.y + horInput * Time.deltaTime * stats.turningSpeed, 0);
        }

        if (modelZ < 10 && horInput == 1 && velocity.magnitude > 1)
        {
            modelZ += 0.25f;
        }
        else if (horInput == -1 && modelZ > -10 && velocity.magnitude > 1)
        {
            modelZ -= 0.25f;
        }
        else
        {
            if (modelZ > 0)
            {
                modelZ -= 0.25f;
            }
            else if(modelZ < 0)
            {
                modelZ += 0.25f;
            }
        }

        model.localRotation = Quaternion.Euler(0, 0, -modelZ);
    }

    void Attack()
    {
        if (!enabled || attack) return;
        if (!onGound)
        {
            rb.velocity = Vector3.down * stats.attackForce;
            attack = true;
            anim.SetTrigger("attack");
            anim.SetBool("attackStay", true);
            Debug.Log("Air Attack");
        }
        else
        {
            if (Thrown != null)
            {
                Debug.Log("Throw");
                canMove = false;
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 5, transform.forward, out hit, 50, enemyLayer.value, QueryTriggerInteraction.Ignore))
                {
                    Debug.Log(hit.transform.name);
                    transform.rotation = Engine.Math.RotateTowards(transform.position, hit.point);
                }
                anim.Play("Throw");
                Thrown(this, transform.forward);
            }
            else
            {
                Debug.Log("Ground Attack");
                attack = true;
                anim.Play("Attack");
                attackParticles.Play();
                if (MeleeAttack != null)
                {
                    MeleeAttack();
                }
            }

        }
    }

    public LayerMask collisionLayer;
    HashSet<IDestructible> scripts = new HashSet<IDestructible>();


    public void StateAnimatorInitialized()
    {
        throwAnimationHash = Animator.StringToHash("Throw");
        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == throwAnimationHash)
            {
                canMove = true;
            }
        };
    }

    void AttackCollision()
    {
        Ray ray = new Ray(curPos, Vector3.down);
        RaycastHit[] hits = Physics.SphereCastAll(curPos, 2, Vector3.down, 10, collisionLayer.value,QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log("Hit: " + hits[i].transform.name);
            scripts.Add(hits[i].collider.transform.root.gameObject.GetComponent<IDestructible>());
        }
        foreach (var script in scripts)
        {
            if (script != null)
            {
                script.Hit();
                smokeExplosion.transform.position = script.Transform.position;
                smokeExplosion.Play();
                //attack = false;
            }
        }
        scripts.Clear();
    }

}
