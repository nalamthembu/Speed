using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThirdPersonFramework.UserInterface
{
    /*[PURPOSE]
     * This is a dynamic prompt screen that takes
     * positive and negative options and acts accordingly.
     * This means that there is no need to create a prompt screen
     * for type of prompt.
     */

    public class PromptScreen : BaseGameMenu
    {
        [SerializeField] TMP_Text m_PromptMessageText;
        [SerializeField] Button m_PositiveBtn;
        [SerializeField] Button m_NegativeBtn;

        public static PromptScreen Instance;

        private Action positiveAction;
        private Action negativeAction;
        public static Action OnActive;
        public static Action OnDeactivate;

        public bool IsActive { get { return !IsHidden(); } }

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void ShowPrompt(string message, Action positiveAction, Action negativeAction)
        {
            m_PromptMessageText.text = message;
            this.positiveAction = positiveAction;
            this.negativeAction = negativeAction;

            m_PositiveBtn.onClick.AddListener(() => {OnPositiveButtonClick();});
            m_NegativeBtn.onClick.AddListener(() => {OnNegativeButtonClick();});

            Show();

            OnActive?.Invoke();
        }

        private void OnPositiveButtonClick()
        {
            positiveAction?.Invoke();
            
            // TODO : Play Sound

            m_PositiveBtn.onClick.RemoveAllListeners();

            Hide();

            OnDeactivate?.Invoke();
        }

        private void OnNegativeButtonClick()
        {
            negativeAction?.Invoke();
            
            // TODO : Play Sound

            m_NegativeBtn.onClick.RemoveAllListeners();

            Hide();

            OnDeactivate?.Invoke();
        }
    }
}