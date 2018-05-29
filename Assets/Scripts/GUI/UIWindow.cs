using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
                return rect.anchoredPosition.y;
            }
        }
        public bool visibleOnStart;
        static List<UIWindow> windows = new List<UIWindow>();
        public string ID;
        public Anchor anchor;
        public float slideSpeed = 50;
        public float fadeSpeed = 10;

        public event Action Hidden;
        public event Action Shown;
        public event Action BeginHide;
        public event Action BeginShow;

        public static UIWindow GetWindow(string ID)
        {
            var window = windows.SingleOrDefault(x => x.ID == ID);
            if (window == null)
            {
                Debug.LogError("Window with ID: " + ID + " not found.");
            }
            return window;
        }

        public const string END_SCREEN = "EndGameScreen";


        RectTransform rect;
        Vector2 desiredPos;
        Vector2 startPos;
        CanvasGroup canvasGroup;

        private void Awake()
        {
            windows.Add(this);
        }

        private void OnDestroy()
        {
            windows.Remove(this);
        }

        private void OnEnable()
        {
            rect = GetComponent<RectTransform>();
        }

        private void OnDisable()
        {
            Debug.Log("Disabled: " + name);
            rect.anchoredPosition = startPos;
        }

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            SetStartPos();
            if (visibleOnStart)
            {
                rect.anchoredPosition = desiredPos;
            }
            else
            {
                gameObject.SetActive(false);
                canvasGroup.alpha = 0;
            }
            if (anchor == Anchor.None)
            {
                rect.anchoredPosition = Vector2.zero;
            }
            startPos = rect.anchoredPosition;
        }

        void SetStartPos()
        {
            if (anchor == Anchor.Bottom)
            {
                rect.anchoredPosition = new Vector2(0, -Screen.height);
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
            if (BeginShow != null)
            {
                BeginShow();
            }
            switch (anchor)
            {
                case Anchor.Left:
                    while (X <= 0)
                    {
                        rect.anchoredPosition += Vector2.right * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Right:
                    while (X >= 0)
                    {
                        rect.anchoredPosition -= Vector2.right * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Bottom:
                    while (Y < 0)
                    {
                        rect.anchoredPosition += Vector2.up * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Top:
                    while (Y > 0)
                    {
                        rect.anchoredPosition += Vector2.down * slideSpeed;
                        if (canvasGroup.alpha < 1) { canvasGroup.alpha += Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                default:
                    yield break;
            }
            canvasGroup.alpha = 1;
            rect.anchoredPosition = desiredPos;
            if (Shown != null)
            {
                Show();
            }
            yield return null;
        }

        IEnumerator HideE()
        {
            if (BeginHide != null)
            {
                BeginHide();
            }
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
                    while (Y > -Screen.height)
                    {
                        rect.anchoredPosition += Vector2.down * slideSpeed;
                        if (canvasGroup.alpha > 0) { canvasGroup.alpha -= Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                case Anchor.Top:
                    while (Y < Screen.height)
                    {
                        rect.anchoredPosition += Vector2.up * slideSpeed;
                        if (canvasGroup.alpha > 0) { canvasGroup.alpha -= Time.deltaTime * slideSpeed / 10; }
                        yield return null;
                    }
                    break;
                default:
                    yield break;
            }
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
            if (Hidden != null)
            {
                Hidden();
            }
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

        public void GoToMenu()
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            //Camera.main.gameObject.SetActive(true);
        }

        public void RestartLevel()
        {
            levelName = SceneManager.GetActiveScene().name;
            SceneManager.UnloadSceneAsync(levelName);
            SceneManager.sceneUnloaded += Restart;

        }

        void Restart(Scene scene)
        {
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += SetActiveScene;
            SceneManager.sceneUnloaded -= Restart;
        }

        string levelName;
        void SetActiveScene(Scene scene, LoadSceneMode mode)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
            SceneManager.sceneLoaded -= SetActiveScene;
        }

        public void GoToNextLevel()
        {
            levelName= LevelManager.Instance.NextLevel();
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            GoToLevel(levelName);
        }

        public void GoToLevel(string name)
        {
            try
            {
                Camera.main.gameObject.SetActive(false);
            }
            catch { }
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += SetActiveScene;
        }

    }
}

