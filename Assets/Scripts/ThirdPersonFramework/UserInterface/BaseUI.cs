using System.Collections;
using UnityEngine;

namespace ThirdPersonFramework.UserInterface
{
    /*[PURPOSE]
    *This is the base class for any UI 
    */

    [RequireComponent(typeof(CanvasGroup))]
    public class BaseUI : MonoBehaviour
    {
        protected CanvasGroup m_CanvasGroup;
        protected virtual void Awake() => m_CanvasGroup = GetComponent<CanvasGroup>();
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnEnable()
        {
            // Listen to Game Pause/Resume Events
            PauseMenu.OnPauseMenuOpened += OnGamePaused;
            PauseMenu.OnPauseMenuClosed += OnGameResumed;
        }
        
        protected virtual void OnDisable()
        {
            // Listen to Game Pause/Resume Events
            PauseMenu.OnPauseMenuOpened -= OnGamePaused;
            PauseMenu.OnPauseMenuClosed -= OnGameResumed;
        }

        protected bool IsHidding = false;
        protected bool IsShowing = false;

        // Base Functions

        #region Debug Functions
        [ContextMenu("Debug/Hide Interface")]
        public void Debug_Hide() => Hide();
        [ContextMenu("Debug/Show Interface")]
        public void Debug_Show() => Show();
        #endregion

        public bool IsHidden() => m_CanvasGroup.alpha <= 0;

        public virtual void Show(bool quick = false)
        {
            if (!quick)
                StartCoroutine(FadeAlpha(m_CanvasGroup, true));
            else
                m_CanvasGroup.alpha = 1;
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;
        }

        public virtual void Hide(bool quick = false)
        {
            if (!quick)
                StartCoroutine(FadeAlpha(m_CanvasGroup, false));
            else
                m_CanvasGroup.alpha = 0;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
        }

        protected virtual void OnGamePaused() { }
        protected virtual void OnGameResumed() { }

        protected virtual IEnumerator FadeAlpha(CanvasGroup canvasGroup, bool fadeIn)
        {
            float incrementValue = Time.deltaTime * 3f;

            if (fadeIn)
            {
                while (canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += incrementValue;

                    if (canvasGroup.alpha + incrementValue >= 1)
                        canvasGroup.alpha = 1;

                    IsHidding = true;

                    yield return null;
                }

                IsHidding = false;
            }
            else
            {
                while (canvasGroup.alpha > 0)
                {
                    canvasGroup.alpha -= incrementValue;

                    if (canvasGroup.alpha - incrementValue <= 0)
                        canvasGroup.alpha = 0;

                    IsShowing = true;

                    yield return null;
                }

                IsShowing = false;
            }
        }
    }
}