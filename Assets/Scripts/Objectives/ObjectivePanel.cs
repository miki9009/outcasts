using UnityEngine;
using Objectives;
using UnityEngine.UI;

public class ObjectivePanel : MonoBehaviour
{
    public Image progressBar;
    public Image circle;
    public Text title;
    public Objective objective;
    public GameObject ok;
    public GameObject failed;
    public Effect finishedEffect;

    public void Initialize()
    {
        objective.ProgressUpdated += OnUpdate;
        objective.Finished += OnFinished;
    }

    void OnUpdate(Objective objective)
    {
        progressBar.fillAmount = objective.Progress;
    }

    private void OnDestroy()
    {
        if(objective != null)
        {
            objective.ProgressUpdated -= OnUpdate;
            objective.Finished -= OnFinished;
        }
    }

    void OnFinished(Objective objective)
    {
        progressBar.gameObject.SetActive(false);
        if (objective.state == State.Completed)
        {
            title.text = "COMPLETED";
            title.color = Color.green;
            ok.SetActive(true);
            finishedEffect.Play(); 
        }
        else
        {
            title.text = "FAILED"; ;
            title.color = Color.red;
            failed.SetActive(true);
        }
    }
}