using ThirdPersonFramework.UserInterface;
using UnityEngine;

public class Drivetrain : MonoBehaviour
{
    protected bool m_GamePaused { get ; private set; }

    protected virtual void Update() { }
    protected virtual void OnDestroy() { }
    protected virtual void Start() { }
    protected virtual void FixedUpdate() { }
    protected virtual void LateUpdate() { }
    protected virtual void Awake() { }

    protected virtual void OnEnable()
    {
        PauseMenu.OnPauseMenuOpened += OnGamePaused;
        PauseMenu.OnPauseMenuClosed += OnGameResume;
    }
    protected virtual void OnDisable()
    {
        PauseMenu.OnPauseMenuOpened -= OnGamePaused;
        PauseMenu.OnPauseMenuClosed -= OnGameResume;
    }

    protected virtual void OnGamePaused() => m_GamePaused = true;
    protected virtual void OnGameResume() => m_GamePaused = false;
}