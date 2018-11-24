using Engine;
using Engine.UI;
using System;
using UnityEngine;

public class CharacterMovementPlayer : CharacterMovement, ILocalPlayer
{
    int direction2D = 1;

    protected Button btnLeft;
    protected Button btnRight;
    protected Button btnJump;
    protected Button btnAttack;
    protected Button btnForward;
    protected Button btnMovement;
    protected bool buttonsInitialized = true;
    protected Vector2 horTouched;
    protected Vector2 verTouched;
    protected bool horPressed;
    protected bool verPressed;
    protected float verDistance;
    protected float horDistance;
    protected Vector3 lastAttackTouchPosition;
    protected Vector3 curHorTouched;
    protected Vector3 pointingDir;

    protected float angle;

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

    public override bool IsPlayer
    {
        get
        {
            return true;
        }
    }

    protected override void Initialize()
    {
        Controller.Instance.gameCamera.regularUpdate = false;
        Controller.Instance.gameCamera.ChangeToRegularCharacterView();
        try
        {
            btnMovement = GameGUI.GetButtonByName("ButtonMovement");
            ButtonsInput = Controller.Instance.ButtonMovement;
            btnAttack = GameGUI.GetButtonByName("ButtonAttack");
            btnJump = GameGUI.GetButtonByName("ButtonForward");
            btnAttack.OnTapPressed.AddListener(Attack);

            if (ButtonsInput)
            {
                btnAttack.gameObject.SetActive(true);

                //btnLeft = GameGUI.GetButtonByName("ButtonLeft");
                //btnLeft.gameObject.SetActive(true);
                //btnRight = GameGUI.GetButtonByName("ButtonRight");
                //btnRight.gameObject.SetActive(true);
                btnJump.gameObject.SetActive(true);
                var rect = GameGUI.GetButtonByName("Action").GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(700, rect.anchoredPosition.y);
                Movement = ButtonsMovement;

            }
            else
            {
                //DeactivateButtons();
                Movement = GestureMovement;
            }
            
        }
        catch (Exception ex)
        {
            Movement = ButtonsMovement;
            Debug.Log("Buttons are not initialized, you can still use keyboard for movement. " + ex );
            buttonsInitialized = false;
        }
    }

    Transform cam;
    public Transform Camera
    {
        get
        {
            if (cam == null)
            {
                cam = Controller.Instance.gameCamera.transform;
            }
            return cam;
        }
    }

    protected override void Inputs()
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

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //verInput = Input.GetAxisRaw("Vertical");
            if (horInput == 0)
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
    }

    //private void DeactivateButtons()
    //{
    //    GameGUI.GetButtonByName("ButtonLeft").gameObject.SetActive(false);
    //    GameGUI.GetButtonByName("ButtonRight").gameObject.SetActive(false);
    //}


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

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
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

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
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
            forwardPower = Mathf.Clamp(horDistance, 0, 100) / 100;
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

        if (btnJump.isTouched)
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

    float sinus;
    protected override void Rotation()
    {
        var vec = Controller.Instance.gameCamera.transform.forward;
        vec.y = 0;
        transform.eulerAngles = targetEuler;
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetEuler), 0.2f);
        sinus = Mathf.Lerp(sinus, angle < 90 && angle > -90 ? angle : angle > 0 ? 180 - angle : -180 - angle, Time.deltaTime * 10);
        model.transform.localEulerAngles = new Vector3(0, 0, Mathf.Clamp(-sinus / 3, -15, 15));
    }

    void OnGUI()
    {
        Draw.TextColor(10, 200, 255, 0, 0, 1, "Material: " + character.bodyMeshRenderer.material.name);
        Draw.TextColor(10, 250, 255, 0, 0, 1, "Shader: " + character.bodyMeshRenderer.material.shader.name);
    }
}