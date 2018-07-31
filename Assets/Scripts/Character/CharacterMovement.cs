using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine;
using Engine.GUI;

public class CharacterMovement : MonoBehaviour, IThrowable, IStateAnimator
{
    public bool infiniteMovement;
    public float laneSize = 5;
    public bool onGround = true;
    public ParticleSystem smoke2;
    public Transform model;
    public LayerMask enemyLayer;
    public ParticleSystem attackParticles;
    bool rotationEnabled = true;
    [NonSerialized]
    public bool movementEnabled = true;
    [HideInInspector]
    public Character character;
    protected CharacterStatistics stats;
    [NonSerialized]
    public Animator anim;
    //public event Action<CharacterMovement> MoveUp;
    //public event Action<CharacterMovement> MoveDown;

    private BoxCollider jumpCollider;

    private ParticleSystem smoke;

    float verInput = 0;
    float horInput = 0;
    float jumpInput = 0;
    [NonSerialized]
    public Rigidbody rb;
    Vector3 curPos;
    public bool attack;
    public bool isAttacking = false;

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
    [System.NonSerialized] public BezierCurve bezier;
    public BezierAssigner bezierAssigner;


    //ANIMATIONS
    int throwAnimationHash;
    int attackAnimationHash;

    int direction2D = 1;
    float disY;

    public bool Invincible { get; set; }
    bool isInfinite;

    public AnimatorBehaviour AnimatorBehaviour
    {
        get;set;
    }

    Action Movement;

    Transform cam;
    public Transform Camera
    {
        get
        {
            if(cam==null)
            {
                cam = Controller.Instance.gameCamera.transform;
            }
            return cam;
        }
    }

    private void Awake()
    {
        character = GetComponent<Character>();
        jumpCollider = GetComponent<BoxCollider>();
        smoke = GetComponentInChildren<ParticleSystem>();
        StartPosition = transform.position;
        disY = Screen.height / 8;
    }

    Button btnLeft;
    Button btnRight;
    Button btnJump;
    Button btnAttack;
    Button btnForward;
    Button btnMovement;
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
        isInfinite = InfiniteManager.Instance != null;
    }

    void InitButtons()
    {
        try
        {
            btnMovement = GameGUI.GetButtonByName("ButtonMovement");
            ButtonsInput = Controller.Instance.ButtonMovement;
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnJump = GameGUI.GetButtonByName("ButtonForward");
            btnAttack.OnTapPressed.AddListener(Attack);
            if (infiniteMovement)
            {
                horInput = 1;
                Movement = GestureMovement;
                DeactivateButtons();
                bezier = bezierAssigner.curve;
            }
            else
            {
                if (ButtonsInput)
                {
                    btnAttack.gameObject.SetActive(true);

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
                    DeactivateButtons();
                    Movement = GestureMovement;
                }
            }
        }
        catch
        {
            Movement = ButtonsMovement;
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement");
            buttonsInitialized = false;
        }
        
    }

    private void DeactivateButtons()
    {
        GameGUI.GetButtonByName("ButtonLeft").gameObject.SetActive(false);
        GameGUI.GetButtonByName("ButtonRight").gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        onGround = false;
        smoke.Stop();
    }

    public void Die()
    {
        character.isDead = true;
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
            if (enemy == null) return;
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
        hspeed = rb.velocity.magnitude;
        if(isInfinite)
        {
            hspeed = hspeed / 2;
        }
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
            anim.Play("Jump");
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
    float horDistance;
    Vector3 lastAttackTouchPosition;
    Vector3 curHorTouched;
    Vector3 pointingDir;
    float forwardPower;
    float angle;
    Vector3 targetEuler;
    public bool Touched
    {
        get
        { return horPressed; }
    }
    public Vector2 StartTouchedPosition
    {
        get
        {
            return horTouched;
        }
    }
    public Vector2 CurrentTouchedPosition
    {
        get
        {
            return curHorTouched;
        }
    }
    void GestureMovement()
    {
        bool pressedHorizontalCurrent = false;
        int touchCount = Input.touchCount;
        var touches = Input.touches;
        jumpInput = 0;
        horDistance = 0;
        forwardPower = 0;
        pressedHorizontalCurrent = false;
        angle = 0;

        if (btnMovement.isTouched || horPressed)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (touches[i].position.x < Screen.width / 2)
                {
                    pressedHorizontalCurrent = true;
                    if (!horPressed)
                    {
                        horPressed = true;
                        horTouched = btnMovement.transform.position;
                    }
                    else
                    {
                        curHorTouched = touches[i].position;
                        horDistance = Vector3.Distance(horTouched, curHorTouched);
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
                        horTouched = btnMovement.transform.position;
                    }
                    else
                    {
                        curHorTouched = Input.mousePosition;
                        horDistance = Vector3.Distance(horTouched, curHorTouched);
                    }
                }
            }
#endif
        }

#if UNITY_EDITOR
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        if (hor < 0) angle = 270;
        if (hor > 0) angle = 90;
        if (ver > 0) angle = 0;
        if (ver < 0) angle = 180;
        if (ver > 0 && hor > 0) angle = 45;
        if (ver > 0 && hor < 0) angle = 315;
        if (ver < 0 && hor > 0) angle = 135;
        if (ver < 0 && hor < 0) angle = 225;
        if (hor != 0 || ver != 0)
        {
            forwardPower = 1;
        }
        targetEuler = new Vector3(0, Camera.eulerAngles.y + angle, 0);
#endif

        if (horDistance > 1)
        {
            pointingDir = Vector.Direction(horTouched, curHorTouched);
            angle = -Vector2.SignedAngle(Vector2.up, pointingDir);
            forwardPower = Mathf.Clamp(horDistance, 0, 50)/50;
            targetEuler = new Vector3(0, Camera.eulerAngles.y + angle, 0);
        }

        if (!pressedHorizontalCurrent && horPressed)
        {
            horPressed = false;
        }

        if (!verPressed)
        {
            if (verDistance > 30)
            {
                if (verTouched.y < lastAttackTouchPosition.y)
                {
                    jumpInput = 1;
                    verTouched.y = 0;
                    lastAttackTouchPosition.y = 0;
                    verDistance = 0;
                }
                else
                {
                    if (!onGround)
                    {
                        lastAttackTouchPosition.y = 0;
                        rb.velocity = Vector3.down * stats.attackForce;
                        anim.SetTrigger("attack");
                        anim.SetBool("attackStay", true);
                    }
                }
            }
        }

        if(btnJump.isTouched)
        {
            jumpInput = 1;
        }
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
        float y = velo.y;
        velo.y = 0;
        float mag = velo.magnitude;
        rb.rotation = transform.rotation;
        if (Mathf.Abs(velo.magnitude) < stats.runSpeed)
        {
            velo = transform.forward * (mag + forwardPower);
            velo.y = y;
            rb.velocity = velo;
            rb.rotation = transform.rotation;
        }
    }
    float sinus;
    void Rotation()
    {
        var vec = Controller.Instance.gameCamera.transform.forward;
        vec.y = 0;
        transform.eulerAngles = targetEuler;
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetEuler), 0.2f);
        sinus = Mathf.Lerp(sinus, angle < 90 && angle > -90 ? angle : angle > 0 ? 180 - angle : -180 - angle, Time.deltaTime * 10);
        model.transform.localEulerAngles = new Vector3(0, 0, Mathf.Clamp(-sinus/3, -15, 15));
    }

    void Attack()
    {
        if (!enabled || attack) return;

            if (Thrown != null)
            {
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
            Debug.Log("Hit: " + hits[i].transform.name);
            Debug.Log(hits[i].collider.GetType());
            scripts.Add(hits[i].collider.GetComponentInParent<IDestructible>());
        }
        foreach (var script in scripts)
        {
            if (script != null)
            {
                script.Hit(this);
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

    //void OnGUI()
    //{
    //    Draw.TextColor(10,200, 255, 0, 0, 1, sinus);
    //}

    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rotation * 10);
    //}



}
