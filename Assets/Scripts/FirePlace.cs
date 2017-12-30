using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Engine;

public class FirePlace : MonoBehaviour
{
    bool goToNextScene = true;
    public string nextLevel;

    ParticleSystem particles;
    Action OnFirePlacedReached;

    private void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particles.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (goToNextScene)
        {
            goToNextScene = false;
            particles.gameObject.SetActive(true);
            if (OnFirePlacedReached != null)
            {
                OnFirePlacedReached();
            }

            Data.Saved += CanGoToNextLevel;
        }
    }

    void CanGoToNextLevel()
    {
        StartCoroutine(GoToNextScene());
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(1);
        LevelManager.Instance.GoToScene(nextLevel);
        yield return null;
    }

}