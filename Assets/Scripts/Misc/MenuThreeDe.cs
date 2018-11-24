using Engine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuThreeDe : MonoBehaviour
{
    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu3D"));
        var window = UIWindow.GetWindow(UIWindow.LOADING_SCREEN);
        if(window!= null)
        {
            window.Hide();
        }
    }
}