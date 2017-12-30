using Engine;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ActivationTrigger : MonoBehaviour
{
    public static bool isActivated;
    IActivationTrigger component;
    static int activators = 0;
    int thisActivators = 0;
    Button button;
    SphereCollider col;


    private void OnTriggerEnter(Collider other)
    {
            activators++;
            thisActivators++;
            component.Activated = true;
            if (!button.gameObject.activeInHierarchy)
            {
                ActivateButton(true);
            }
       // Debug.Log("Trigger: Activated");
    }

    public void OnTriggerExit(Collider other)
    {      
        activators--;
        thisActivators--;
        component.Activated = true;
        if (activators <= 0)
        {
            ActivateButton(false);
        }
    }

    public void DeactivateTrigger()
    {
        col.enabled = false;
        GameGUI.GetButtonByName("Action").OnTapPressed.RemoveListener(component.Activate);
        component.Activated = false;
        activators -= thisActivators;
        if (activators <= 0)
        {
            ActivateButton(false);
        }
    }


    private void Start()
    {
        button = GameGUI.GetButtonByName("Action");
        component = GetComponent<IActivationTrigger>();
        if (component != null)
        {
            try
            {
                button.OnTapPressed.AddListener(component.Activate);
            }
            catch { }

        }
        col = GetComponent<SphereCollider>();
    }

    void ActivateButton(bool set)
    {
        
        if (set)
        {
            Effects.ScalePulse(button.transform, 2, 1);
            button.gameObject.SetActive(set);
        }
        else
        {
            Effects.ScalePulseVanish(button.transform, 2, 1, () => button.gameObject.SetActive(set));
        }
    }

    private void OnDestroy()
    {
        if (component != null)
        {
           var button = GameGUI.GetButtonByName("Action");
            if (button != null)
            {
                button.OnTapPressed.RemoveListener(component.Activate);
            }
        }
    }
}

public interface IActivationTrigger
{
    bool Activated { get; set; }
    bool Used { get; set; }
    void Activate();
}

