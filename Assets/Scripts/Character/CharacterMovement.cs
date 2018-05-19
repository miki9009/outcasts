using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using Engine.GUI;

public class CharacterMovement : MonoBehaviour, IThrowable, IStateAnimator
{

    public bool onGround = true;
    public ParticleSystem smoke2;
    public Transform model;
    public LayerMask enemyLayer;
    public ParticleSystem attackParticles;
    bool rotationEnabled = true;
    [NonSerialized]
    public bool movementEnabled = true;
    protected Character character;
    protected CharacterStatistics stats;
    [NonSerialized]
    public Animator anim;

    private BoxCollider jumpCollider;

    private ParticleSystem smoke;

    float verInput = 0;
    float horInput = 0;
    float jumpInput = 0;
    [NonSerialized]
    public Rigidbody rb;
    Vector3 curPos;
    public bool attack;
    bool isAttacking = false;

    public Vector3 velocity;
    public float pipeFactor;

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

    //ANIMATIONS
    int throwAnimationHash;
    int attackAnimationHash;
    int attackAnimationHash2;

    int direction2D = 1;

    public bool Invincible { get; set; }

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
        rotationEnabled = Controller.Instance.gameType == Controller.GameType.Perspective;
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
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnAttack.OnTapPressed.AddListener(Attack);
            btnJump = GameGUI.GetButtonByName("ButtonJump");

            if (rotationEnabled)
            {
                btnLeft = GameGUI.GetButtonByName("ButtonLeft");
                btnRight = GameGUI.GetButtonByName("ButtonRight");
                btnForward = GameGUI.GetButtonByName("ButtonForward");
            }
            else
            {
                btnLeft = GameGUI.GetButtonByName("ButtonLeft");
                btnRight = GameGUI.GetButtonByName("ButtonRight");
                btnJump.gameObject.SetActive(false);
                btnJump = GameGUI.GetButtonByName("ButtonForward");
            }


        }
        catch
        {
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement");
            buttonsInitialized = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        onGround = false;
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
            //onGound = true;
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
        else if(other.gameObject.layer == Layers.Environment || other.gameObject.layer == Layers.Destructible)
        {
            //Debug.Log("OnGround = true");
            onGround = true;
            smoke2.Emit(15);
        }
    }


    public void Hit(Enemy enemy = null, int hp = 1, bool heavyAttack = false)
    {
        if (stats.health <= 0 || Invincible || (isAttacking && !heavyAttack)) return;
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

        //hspeed = Mathf.Lerp(hspeed, anim.GetFloat("hSpeed"), 0.05f);
        //vspeed = Mathf.Lerp(, anim.GetFloat("vSpeed"), 0.05f);
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
            if (jumpInput > 0 && (Physics.Raycast(transform.position, Vector3.down,1)/* || doubleJump*/))
            {
                timeLastJump = 1;
                rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
                if (onGround)
                {
                    onGround = false;
                    //doubleJump = true;
                }
                //else
                //{
                //    doubleJump = false;
                //}
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
                if (btnJump.isTouched) jumpInput = 1;

                if (!rotationEnabled)
                {
                    if (btnRight.isTouched)
                    {
                        verInput = 1;
                        direction2D = 1;
                    }
                    if (btnLeft.isTouched)
                    {
                        verInput = 1;
                        direction2D = -1;
                    }
                }
                else
                {

                    if (btnRight.isTouched) horInput = 1;
                    if (btnLeft.isTouched) horInput = -1;
                    if (btnForward.isTouched) verInput = 1;

                }
            }

#if UNITY_EDITOR
            if (rotationEnabled)
            {
                if (verInput == 0)
                {
                    verInput = Input.GetAxisRaw("Vertical");
                }

                if (horInput == 0)
                {
                    horInput = Input.GetAxisRaw("Horizontal");
                }
            }
            else
            {
                if (verInput == 0)
                {
                    verInput = Input.GetAxisRaw("Horizontal");
                    if(verInput > 0)
                    {
                        direction2D = 1;
                    }else if(verInput < 0)
                    {
                        direction2D = -1;
                        verInput = 1;
                    }
                }
            }

            if (jumpInput == 0)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    jumpInput = 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Log("Attack Invoked");
                btnAttack.OnTapPressedInvoke();
            }
#endif
        }


        anim.SetFloat("vSpeed", velocity.y);
        anim.SetBool("onGround", onGround);
    }


    public void Move()
    {
        var velo = rb.velocity;
        anim.SetFloat("hSpeed", velo.magnitude);
        if (verInput != 0 && Mathf.Abs(velo.magnitude) < stats.runSpeed && movementEnabled)
        {
            velo += rotationEnabled ? transform.forward * verInput : Vector3.right * verInput * direction2D;
            rb.velocity = velo;
            rb.rotation = transform.rotation;
        }
        rb.maxAngularVelocity = 0;
    }

    void Rotation()
    {
        if (rotationEnabled)
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
                else if (modelZ < 0)
                {
                    modelZ += 0.25f;
                }
            }

            model.localRotation = Quaternion.Euler(0, 0, -modelZ);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.right * direction2D), Time.deltaTime * stats.turningSpeed / 4);
        }


    }

    void Attack()
    {
        if (!enabled || attack) return;
        if (!onGround && Mathf.Abs(velocity.y) > 5)
        {
            rb.velocity = Vector3.down * stats.attackForce;
            attack = true;
            anim.SetTrigger("attack");
            anim.SetBool("attackStay", true);
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
                    //Debug.Log(hit.transform.name);
                    transform.rotation = Engine.Math.RotateTowards(transform.position, hit.point);
                }
                anim.Play("Throw");
                Thrown(this, transform.forward);
            }
            else
            {
                //Debug.Log("Ground Attack");
                attack = true;
                anim.Play("Attack");
                isAttacking = true;
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
        attackAnimationHash = Animator.StringToHash("Attack");
        AnimatorBehaviour.StateExit += (animatorStateInfo) =>
        {
            if (animatorStateInfo.shortNameHash == throwAnimationHash)
            {
                canMove = true;
            }
            else if (animatorStateInfo.shortNameHash == attackAnimationHash)
            {
                isAttacking = false;
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

    public void MovementEnable(bool enable)
    {
        movementEnabled = enable;
    }

    public void SetAnimation(string animationName)
    {
        anim.Play(animationName);
    }

    public void AnimationSetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    float hspeed;
    float vspeed;
    void OnGUI()
    {

        Draw.TextColor(10, 10, 255, 255, 255, 1, string.Format( "Vspeed: {0:0.0}", vspeed));
        Draw.TextColor(10, 50, 255, 255, 255, 1, string.Format("Hspeed: {0:0.0}", hspeed));
    }

}
