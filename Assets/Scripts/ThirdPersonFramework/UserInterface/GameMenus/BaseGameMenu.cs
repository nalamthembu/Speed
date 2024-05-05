using UnityEngine;

namespace ThirdPersonFramework.UserInterface
{
    public class BaseGameMenu : BaseUI
    {
        protected bool IsOpened;

        public bool GetIsOpen() => IsOpened;

        protected virtual void Initialise()
        {
            Hide(true);
        }

        public override void Hide(bool quick = false)
        {
            base.Hide(quick);

            // TODO : Play Sound
        }

        public override void Show(bool quick = false)
        {
            base.Show(quick);

            // TODO : Play Sound
        }

        protected override void Awake()
        {
            base.Awake();

            Initialise();
        }

        protected bool IsOnConsole() => Application.isConsolePlatform;

        protected override void OnDisable()
        {
            base.OnDisable();
            BaseRace.OnPlayerWonRace -= OnPlayerWinRace;
            BaseRace.OnPlayerLostRace -= OnPlayerLostRace;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BaseRace.OnPlayerWonRace += OnPlayerWinRace;
            BaseRace.OnPlayerLostRace += OnPlayerLostRace;
        }

        protected virtual void OnPlayerLostRace() { }
        protected virtual void OnPlayerWinRace() { }
    }
}