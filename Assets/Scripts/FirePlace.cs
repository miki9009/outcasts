using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Engine;
using Engine.GUI;

public class FirePlace : MonoBehaviour
{
    bool goToNextScene = true;
    [LevelSelector]
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
        if (other.gameObject.layer != Layers.Character) return;
        if (goToNextScene)
        {
            goToNextScene = false;
            particles.gameObject.SetActive(true);
            OnFirePlacedReached?.Invoke();

            //Data.Saved += CanGoToNextLevel;
            StartCoroutine(WaitSecond());
        }
    }

    IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(2);
        GameManager.Instance.EndGame(GameManager.GameState.Completed);
    }



    void CanGoToNextLevel()
    {
        StartCoroutine(GoToNextScene());
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("LOADING NEXT SCENE: " + nextLevel);
        LevelManager.Instance.GoToScene(nextLevel);
        yield return null;
    }

}