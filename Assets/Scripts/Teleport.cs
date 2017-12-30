﻿using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Teleport : MonoBehaviour
{
    public Teleport otherTeleport;
    Transform character;
    public bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (canTeleport && otherTeleport.canTeleport)
        {
            canTeleport = false;
            otherTeleport.canTeleport = false;
            character = other.transform.root;
            StartCoroutine(Teleportation());
        }
    }

    private void OnTriggerExit()
    {
        canTeleport = true;
    }

    IEnumerator Teleportation()
    {

        VignetteAndChromaticAberration visualEffect = Controller.Instance.gameCamera.GetComponent<VignetteAndChromaticAberration>();
        Vortex vortex = Controller.Instance.gameCamera.GetComponent<Vortex>();
        visualEffect.enabled = true;
        vortex.enabled = true;
        float intensity = 0.7f;
        float angle = 0;
        while (angle < 359f)
        {
            angle += 30;
            vortex.angle = angle;
            yield return null;
        }

        while (intensity < 0.98f)
        {
            //intensity += 0.01f;
            intensity += 0.02f;
            visualEffect.intensity = intensity;
            yield return null;
        }
        otherTeleport.canTeleport = false;
        character.position = otherTeleport.transform.position;
        visualEffect.intensity = 1;
        yield return new WaitForSeconds(0.5f);
        while (intensity > 0.7f)
        {
            intensity -= 0.02f;
            visualEffect.intensity = intensity;
            yield return null;
        }
        while (angle > 0f)
        {
            angle -= 30;
            vortex.angle = angle;           
            yield return null;
        }
        visualEffect.enabled = false;
        vortex.enabled = false;
        yield return null;
    }
}



