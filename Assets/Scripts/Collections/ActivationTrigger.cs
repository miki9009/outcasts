using Engine;
using Engine.GUI;
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
    public Character Character { get; set; }


    private void OnTriggerEnter(Collider other)
    {
        if (component.Activated) return;
        activators++;
        thisActivators++;
        component.Activated = true;
        if (button != null && !button.gameObject.activeInHierarchy)
        {
            ActivateButton(true);
        }
        Debug.Log("Activated " + transform.name);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!component.Activated) return;
        activators--;
        thisActivators--;
        component.Activated = false;
        if (activators <= 0)
        {
            ActivateButton(false);
        }
        Debug.Log("Disabled " + transform.name);
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
            Effects.ScalePulse(button.transform, 2, 1, ()=> button.transform.localScale = Vector3.one);
            button.gameObject.SetActive(set);
        }
        else
        {
            Effects.ScalePulseVanish(button.transform, 2, 1, () => { button.gameObject.SetActive(set); button.transform.localScale = Vector3.one; });
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

