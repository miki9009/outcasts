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
    float hspeed;
    float vspeed;
    public float pipeFactor;

    float timeLastJump = 0;
    bool jumpReleased = true;
    float modelZ = 0;
    ParticleSystem smokeExplosion;
    ParticleSystem starsExplosion;
    [HideInInspector] public CharacterHealth characterHealth;
    public Action MeleeAttack;
    public event Action<IThrowable, Vector3> Thrown;
    public ThrowableObject ThrowObject { get; set; }
    public Vector3 StartPosition { get; private set; }
    public bool ButtonsInput { get; set; }

    //ANIMATIONS
    int throwAnimationHash;
    int attackAnimationHash;

    int direction2D = 1;

    public bool Invincible { get; set; }

    public AnimatorBehaviour AnimatorBehaviour
    {
        get;set;
    }

    Action Movement;

    private void Awake()
    {
        character = GetComponent<Character>();
        jumpCollider = GetComponent<BoxCollider>();
        smoke = GetComponentInChildren<ParticleSystem>();
        StartPosition = transform.position;
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
            ButtonsInput = Controller.Instance.ButtonMovement;
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnJump = GameGUI.GetButtonByName("ButtonForward");
            if (ButtonsInput)
            {
                btnAttack.gameObject.SetActive(true);
                btnAttack.OnTapPressed.AddListener(Attack);
                btnLeft = GameGUI.GetButtonByName("ButtonLeft");
                btnLeft.gameObject.SetActive(true);
                btnRight = GameGUI.GetButtonByName("ButtonRight");
                btnRight.gameObject.SetActive(true);              
                btnJump.gameObject.SetActive(true);
                var rect = GameGUI.GetButtonByName("Action").GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(700, rect.anchoredPosition.y);
                Movement = ButtonsMovement;

            }
            else
            {
                btnAttack.gameObject.SetActive(false);
                GameGUI.GetButtonByName("ButtonLeft").gameObject.SetActive(false);
                GameGUI.GetButtonByName("ButtonRight").gameObject.SetActive(false);
                btnJump.gameObject.SetActive(false) ;
                var rect = GameGUI.GetButtonByName("Action").GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, rect.anchoredPosition.y);
                Movement = GestureMovement;
            }
        }
        catch
        {
            Movement = ButtonsMovement;
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
        DieNonAnimation();
    }

    public void DieNonAnimation()
    {
        enabled = false;
        StopAllCoroutines();
        Controller.Instance.OnPlayerDead(character);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == Layers.Environment)
        {
            onGround = true;
        }
        if (other.gameObject.layer != 12 && other.gameObject.layer != 13)
        {
            if (!smoke.isPlaying)
            {
                smoke.Play();
            }
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
        velocity = rb.velocity;
        hspeed = Mathf.Abs(velocity.x + velocity.z); //Mathf.Lerp(hspeed, velocity.x + velocity.z, 0.05f);
        vspeed = velocity.y; //Mathf.Lerp(vspeed,velocity.y, 0.05f);
        anim.SetFloat("hSpeed", hspeed);
        anim.SetFloat("vSpeed", vspeed);
        Move();
        Rotation();
    }

    private void Update()
    {
        curPos = transform.position;
        Movement();
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
        if (jumpInput > 0 && onGround && timeLastJump < 0.1f)
        {
            timeLastJump = 1;
            rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
            jumpInput = 0;
            onGround = false;
        }
        else
        {
            timeLastJump -= Time.deltaTime;
        }
    }

    void Inputs()
    {        
        if (canMove)
        {
            if (buttonsInitialized)
            {
                if (verInput < 0)
                {
                    verInput = 1;
                    direction2D = 1;
                }
                if (verInput > 0)
                {
                    verInput = 1;
                    direction2D = -1;
                }
            }

#if UNITY_EDITOR
            //verInput = Input.GetAxisRaw("Vertical");
            if(horInput == 0)
                horInput = Input.GetAxisRaw("Horizontal");

            if (jumpInput == 0)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    jumpInput = 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                //Debug.Log("Attack Invoked");
                btnAttack.OnTapPressedInvoke();
            }
#endif
        }
        //anim.SetFloat("vSpeed", velocity.y);
        if (horInput > 0)
            direction2D = 1;
        else if (horInput < 0)
            direction2D = -1;
        anim.SetBool("onGround", onGround);
    }

    Vector2 horTouched;
    Vector2 verTouched;
    bool horPressed;
    bool verPressed;
    float verDistance;
    float timeJumpWait = 1;
    void GestureMovement()
    {
        bool pressedHorizontalCurrent = false;
        bool pressedVerticalCurrent = false;
        int touchCount = Input.touchCount;
        var touches = Input.touches;
        jumpInput = 0;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (touches[i].position.x < Screen.width / 2)
            {
                pressedHorizontalCurrent = true;
                if (!horPressed)
                {
                    horPressed = true;
                    horTouched = touches[i].position;
                }
                else
                {
                    horInput = Mathf.Clamp((touches[i].position.x - horTouched.x)/50, -1, 1);
                }
            }
            else
            {
                pressedVerticalCurrent = true;
                if(!verPressed)
                {
                    verPressed = true;
                    verTouched = touches[i].position;
                }
                else
                {
                    verDistance = Vector3.Distance(verTouched, touches[i].position);
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                pressedHorizontalCurrent = true;
                if (!horPressed)
                {
                    horPressed = true;
                    horTouched = Input.mousePosition;
                }
                else
                {
                    horInput = Mathf.Clamp((Input.mousePosition.x - horTouched.x), -1, 1);
                }
            }
            else
            {
                pressedVerticalCurrent = true;
                if (!verPressed)
                {
                    verPressed = true;
                    verTouched = Input.mousePosition;
                }
                else
                {
                    verDistance = Vector3.Distance(verTouched, Input.mousePosition);
                }
            }
        }
#endif
        if (!pressedHorizontalCurrent)
        {
            horPressed = false;
            horInput = Mathf.Lerp(horInput, 0, Time.deltaTime * 10);
            if(Mathf.Abs(horInput) < 0.1f)
            {
                horInput = 0;
            }
        }
        if (verPressed && verDistance > 30)
        {
            verPressed = false;
            jumpInput = 1;
        }

        if (verPressed && !pressedVerticalCurrent)
        {
            verPressed = false;
            if (verDistance < 50)
            {
                jumpInput = 0;
                Attack();
            }
        }
        verDistance = Mathf.Clamp(verDistance - 1, 0, 50);
    }

    void ButtonsMovement()
    {
        horInput = 0;
        if (buttonsInitialized)
        {
            if (btnRight.isTouched) horInput = 1;
            if (btnLeft.isTouched) horInput = -1;
            if (btnJump.isTouched)
                jumpInput = 1;
            else
                jumpInput = 0;
        }
    }


    public void Move()
    {
        var velo = rb.velocity;
        //anim.SetFloat("hSpeed", velo.magnitude);
        if (horInput != 0 && Mathf.Abs(velo.magnitude) < stats.runSpeed && movementEnabled)
        {
            velo += rotationEnabled ? transform.forward * verInput : Vector3.right * horInput;
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
        //if (!onGround && Mathf.Abs(velocity.y) > 5)
        //{
        //    rb.velocity = Vector3.down * stats.attackForce;
        //    attack = true;
        //    anim.SetTrigger("attack");
        //    anim.SetBool("attackStay", true);
        //}
        //else
        //{
            if (Thrown != null)
            {
                //Debug.Log("Throw");
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
        //}
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
            //Debug.Log("Hit: " + hits[i].transform.name);
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

}
