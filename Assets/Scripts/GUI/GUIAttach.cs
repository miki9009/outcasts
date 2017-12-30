using Engine;
using UnityEngine;

public class GUIAttach : MonoBehaviour
{
    public bool destroyOnLevelEnd;

    void Start()
    {
        var rect = GetComponent<RectTransform>();
        transform.parent = GameGUI.Instance.transform;
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        if (destroyOnLevelEnd)
        {

        }
    }
}