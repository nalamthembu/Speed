using System;
using ThirdPersonFramework.UserInterface;
using UnityEngine;

namespace ThirdPersonFramework.GameManagers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public static Action EventOnGamePaused;
        public static Action EventOnGameResume;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void OnEnable()
        {
            PauseMenu.OnPauseMenuOpened += OnGamePaused;
            PauseMenu.OnPauseMenuClosed += OnGameResume;
        }

        private void OnDisable()
        {
            PauseMenu.OnPauseMenuOpened -= OnGamePaused;
            PauseMenu.OnPauseMenuClosed -= OnGameResume;
        }

        private void OnGamePaused() => EventOnGamePaused?.Invoke();
        private void OnGameResume() => EventOnGameResume?.Invoke();

        public void OnPlayerQuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void OnCancelQuitGame()
        {
            //Do nothing.
        }
    }
}