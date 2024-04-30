using ThirdPersonFramework.GameManagers;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ThirdPersonFramework.UserInterface
{
    // Subclasses
    [Serializable]
    public class PauseMenuButton
    {
        [SerializeField] string Name;
        [SerializeField] string Description;
        [SerializeField] Button ButtonComp;
        [SerializeField] bool IsConsoleCompatible; //Example : A Quit Game Button Wouldn't exist on console.
        [SerializeField] bool SendsPlayerToMainMenu;
        [SerializeField] bool QuitsGame;
        [SerializeField] bool ResumesGame;

        public string GetName() => Name;
        public bool GetIsConsoleCompatible() => IsConsoleCompatible;
        public void Hide() => ButtonComp.gameObject.SetActive(false);
        public void Show() => ButtonComp.gameObject.SetActive(true);
        public bool IsHidden() => !ButtonComp.gameObject.activeSelf;
        public void SetInteractable(bool value) => ButtonComp.interactable = value;
        public bool CanSendPlayerToMainMenu() => SendsPlayerToMainMenu;
        public bool CanQuitGame() => QuitsGame;
        public bool CanResumeGame() => ResumesGame;
        public Button GetButton() => ButtonComp;
    }

    public class PauseMenu : BaseGameMenu
    {
        [SerializeField] PauseMenuButton[] m_PauseMenuButtons;

        public static Action OnPauseMenuOpened;
        public static Action OnPauseMenuClosed;

        protected override void Initialise()
        {
            base.Initialise();

            foreach (var button in m_PauseMenuButtons)
            {
                if (!button.GetIsConsoleCompatible() && IsOnConsole())
                    button.Hide();

                //Add Listener for button sounds
                button.GetButton().onClick.AddListener(() =>
                {                       
                    //Don't allow the button to work when a prompt screen is active
                    if (PromptScreen.Instance && PromptScreen.Instance.IsActive)
                        return;

                    //TODO : Play Sound
                });

                //Check for resume game Button
                if (button.CanResumeGame())
                {
                    button.GetButton().onClick.AddListener(() =>
                    {
                        //This will unpause the game if its already paused.
                        OnGamePaused();
                    });
                }

                //Check for quit game button
                if (button.CanQuitGame())
                {
                    button.GetButton().onClick.AddListener(() =>
                    {
                        string message = $"You are about to {button.GetName()}. Are you sure?";

                        if (GameManager.Instance == null)
                        {
                            Debug.LogError("There is no Game Manager scene, could not retrieve quit game events.");
                            return;
                        }

                        PromptScreen.Instance.ShowPrompt(message, GameManager.Instance.OnPlayerQuitGame, GameManager.Instance.OnCancelQuitGame);
                    });
                }

                //Check for quit to main menu
                if (button.CanSendPlayerToMainMenu())
                {
                    button.GetButton().onClick.AddListener(() =>
                    {
                        string message = $"You are about to {button.GetName()}. Are you sure?";

                        if (GameManager.Instance == null)
                        {
                            Debug.LogError("There is no Game Manager scene, could not retrieve quit game events.");
                            return;
                        }

                        PromptScreen.Instance.ShowPrompt(message, GameManager.Instance.OnPlayerQuitGame, GameManager.Instance.OnCancelQuitGame);
                    });
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerController.OnPaused += OnGamePaused;
        }
        

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerController.OnPaused -= OnGamePaused;
        }

        public override void Show(bool quick = false)
        {
            base.Show(quick);
            OnPauseMenuOpened?.Invoke();

            //Make Cursor Visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public override void Hide(bool quick = false)
        {
            base.Hide(quick);
            OnPauseMenuClosed?.Invoke();

            //Make Cursor Visible
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        protected override void OnGamePaused()
        {
            if (PromptScreen.Instance && PromptScreen.Instance.IsActive)
                return;

            if (IsHidden() && !IsShowing)
                Show();

            if (!IsHidden() && !IsHidding)
                Hide();
        }
    }
}