using System.Collections;
using UnityEngine;

public class CharacterPhoton : Photon.MonoBehaviour
{
    Rigidbody rb;
    Character character;
    CharacterMovement movement;
    Vector3 photonPos;
    Quaternion photonRot;
    Vector3 velo;

    private void Awake()
    {
        character = GetComponent<Character>();
        movement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (PhotonManager.IsMultiplayer)
        {
            if (character != Character.GetLocalPlayer())
                StartCoroutine(HandlePhotonObject());
        }
        else
        {
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        movement.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        movement.OnTriggerExit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        movement.OnTriggerStay(other);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if(rb!=null)
                stream.SendNext(rb.velocity);
        }
        else
        {
            photonPos = (Vector3)stream.ReceiveNext();
            photonRot = (Quaternion)stream.ReceiveNext();
            velo = (Vector3)stream.ReceiveNext();
        }
    }

    IEnumerator HandlePhotonObject()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, photonPos, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, photonRot, 0.1f);
            movement.SetAnimationHorizontal(velo);
            yield return null;
        }
    }
}