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
            if (OnFirePlacedReached != null)
            {
                OnFirePlacedReached();
            }

            //Data.Saved += CanGoToNextLevel;
            StartCoroutine(WaitSecond());
        }
    }

    IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(2);
        EndGame();
    }

    void EndGame()
    {
        GameManager.Instance.GameFinished();
        Pause.Instance.PauseEnter();
        UIWindow.GetWindow("EndGameScreen").Show();
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