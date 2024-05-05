using UnityEngine;
using UnityEngine.UI;

namespace ThirdPersonFramework.UserInterface
{
    public class PlayerHUD : BaseUI
    {
        [Header("Player Health Components")]
        [SerializeField] Slider m_HealthSlider;
        [SerializeField] Slider m_ArmourSlider;

        protected override void Update() => base.Update();
        
        protected override void OnEnable()
        {
            base.OnEnable();
            PauseMenu.OnPauseMenuOpened += OnGamePaused;
            PauseMenu.OnPauseMenuClosed += OnGameResumed;
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            PauseMenu.OnPauseMenuOpened -= OnGamePaused;
            PauseMenu.OnPauseMenuClosed -= OnGameResumed;
        }

        private void OnGamePaused() => Hide();
        private void OnGameResumed() => Show();


        private void UpdateHealthSliders(float health, float armor)
        {
            m_HealthSlider.value = health;
            m_ArmourSlider.value = armor;

            if (IsHidden() && !IsShowing)
                Show();
        }
    }
}