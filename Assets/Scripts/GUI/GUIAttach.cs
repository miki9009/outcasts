using UnityEngine;

namespace Engine.GUI
{
    public class GUIAttach : MonoBehaviour
    {

        void Start()
        {
            var rect = GetComponent<RectTransform>();
            transform.parent = GameGUI.Instance.transform;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }
    }
}