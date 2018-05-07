using Engine;
using System.Collections;
using UnityEngine;


public class Pipe : MonoBehaviour, IActivationTrigger
{
    public float pipeForce;
    public int numberOfLeaves = 25;
    public bool blur = true;
    public ParticleSystem parts;
    public ActivationTrigger trigger;
    public bool Activated{get;set;}

    public bool Used { get; set; }
    public BezierCurve curve;
    Vector3 currentCharacterPos;
    CharacterMovement characterMovement;
    Quaternion characterRotation;
    SphereCollider sphere;



    void Awake()
    {
        sphere = GetComponent<SphereCollider>();
    }

    public void Activate()
    {
        CharacterMovement movement = Character.GetLocalPlayer().movement;
        if (!Used && Vector3.Distance(movement.transform.position, transform.position) < sphere.radius)
        {
            StartCoroutine(JumpToPipeCor(movement));
        }
    }

    IEnumerator JumpToPipeCor(CharacterMovement movement)
    {
        var cam = Controller.Instance.gameCamera.GetComponent<GameCamera>();
        float lastSpeed = cam.speed;
        cam.speed = lastSpeed / 6;
        characterMovement = movement;
        movement.rb.AddForce(Vector3.up * pipeForce, ForceMode.VelocityChange);
        movement.MovementEnable(false);
        Used = true;
        bool pipeEntered = false;
        float animation = 0;
        movement.SetAnimation("JumpUp");
        if(curve == null)
        {
            curve = GetComponentInChildren<BezierCurve>();
        }
        while (!pipeEntered)
        {
            var dis = Vector3.Distance(transform.position, currentCharacterPos);
            currentCharacterPos = movement.transform.position;
            characterRotation = movement.transform.rotation;
            Vector3 pipePos = new Vector3(transform.position.x, currentCharacterPos.y, transform.position.z);
            if (dis < 1)
            {
                pipeEntered = true;
            }
            if (animation < 1)
            {
                animation += Time.deltaTime * dis / 10;
            }
            if (Vector3.Distance(transform.position, pipePos) > 1)
            {
                characterRotation = Quaternion.Slerp(characterRotation, Quaternion.LookRotation(Vector.Direction(currentCharacterPos, pipePos + Vector3.forward)),Time.deltaTime * 10);
                currentCharacterPos = Vector3.Lerp(currentCharacterPos, pipePos, animation);
            }

            yield return null;
        }
        //parts.Emit(numberOfLeaves);
        float progress = 0;
        cam.motionBlure.enabled = blur;
        while (progress < 1)
        {
            Vector3 aim = curve.GetPointAt(0.99f);
            aim.y = currentCharacterPos.y;
            characterRotation = Quaternion.Slerp(characterRotation, Quaternion.LookRotation(Vector.Direction(currentCharacterPos, aim)),Time.deltaTime * 10);
            progress += Time.deltaTime / 2;
            cam.motionBlure.blurAmount = 1 - progress;
            currentCharacterPos = curve.GetPointAt(progress);
            yield return null;
        }
        cam.motionBlure.enabled = false;
        movement.rb.velocity = Vector3.zero;
        Used = false;
        movement.MovementEnable(true);
        cam.speed = lastSpeed;
    }

    void FixedUpdate()
    {
        if (Used && characterMovement != null)
        {
            characterMovement.transform.position = currentCharacterPos;
            characterMovement.transform.rotation = characterRotation;
        }
    }
}