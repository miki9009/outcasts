using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public enum Anchor
        {
            Left,
            Right,
            Bottom,
            Top,
            None
        }

        public float X
        {
            get
            {
                return rect.anchoredPosition.x;
            }
        }

        public float Y
        {
            get
            {
                return rect.anchoredPosition.x;
            }
        }
        public bool visibleOnStart;
        public static List<UIWindow> windows = new List<UIWindow>();
        public string ID;
        public Anchor anchor;
        public float slideSpeed = 50;
        public float fadeSpeed = 10;

        RectTransform rect;
        Vector2 desiredPos;
        CanvasGroup canvasGroup;

        private void OnEnable()
        {
            rect = GetComponent<RectTransform>();
            windows.Add(this);
        }

        private void OnDisable()
        {
            windows.Remove(this);
        }

        private void Start()
        {
            if (anchor == Anchor.Bottom)
            {
                rect.anchoredPosition = new Vector2(Screen.height, 0);
            }
            else if (anchor == Anchor.Left)
            {
                rect.anchoredPosition = new Vector2(-Screen.width, 0);
            }
            else if (anchor == Anchor.Right)
            {
                rect.anchoredPosition = new Vector2(Screen.width, 0);
            }
            else if (anchor == Anchor.Top)
            {
                rect.anchoredPosition = new Vector2(0, -Screen.height);
            }
            canvasGroup = GetComponent<CanvasGroup>();

            if (visibleOnStart)
            {
                rect.anchoredPosition = desiredPos;
            }
            else
            {
                enabled = false;
                canvasGroup.alpha = 0;
            }
            if (anchor == Anchor.None)
            {
                rect.anchoredPosition = Vector2.zero;
            }
        }


        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowE());
        }

        public void Hide()
        {
            StartCoroutine(HideE());
        }

        public void FadeOut()
        {
            StartCoroutine(FadeOutE());
        }
        IEnumerator FadeOutE()
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            gameObject.SetActive(false);
            yield return null;
        }
        public void FadeIn()
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeInE());
        }
        IEnumerator FadeInE()
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            yield return null;
        }

        IEnumerator ShowE()
        {
            switch (anchor)
            {
                case Anchor.Left:
                    while (X < 0)
                    {
                        rect.anchoredPosition += Vector2.right * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Right:
                    while (X > 0)
                    {
                        rect.anchoredPosition -= Vector2.right * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Bottom:
                    while (Y > 0)
                    {
                        rect.anchoredPosition += Vector2.down * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Top:
                    while (Y < 0)
                    {
                        rect.anchoredPosition += Vector2.up * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                default:
                    yield break;
            }
            rect.anchoredPosition = desiredPos;
            yield return null;
        }

        IEnumerator HideE()
        {
            switch (anchor)
            {
                case Anchor.Left:
                    while (X > -Screen.width)
                    {
                        rect.anchoredPosition -= Vector2.right * slideSpeed;
                        if (canvasGroup.alpha > 0) { canvasGroup.alpha -= Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Right:
                    while (X < Screen.width)
                    {
                        rect.anchoredPosition += Vector2.right * slideSpeed;
                        if (canvasGroup.alpha > 0) { canvasGroup.alpha -= Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Bottom:
                    while (Y < Screen.height)
                    {
                        rect.anchoredPosition += Vector2.up * slideSpeed;
                        if (canvasGroup.alpha > 0) { canvasGroup.alpha -= Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Top:
                    while (Y > -Screen.height)
                    {
                        rect.anchoredPosition += Vector2.down * slideSpeed;
                        if (canvasGroup.alpha > 0) { canvasGroup.alpha -= Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                default:
                    yield break;
            }
            gameObject.SetActive(false);
            yield return null;
        }

        public void GamePauseEnter()
        {
            Engine.Pause.Instance.PauseEnter();
        }

        public void GamePaseLeave()
        {
            Engine.Pause.Instance.PauseLeave();
        }

    }
}

