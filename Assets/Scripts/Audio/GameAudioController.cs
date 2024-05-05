using System.Collections;
using System.Collections.Generic;
using ThirdPersonFramework.UserInterface;
using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    public static GameAudioController Instance;

    [SerializeField] AudioControllerData audioControllerData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (audioControllerData == null)
        {
            Debug.LogError("AUDIO CONTROLLER DATA IS NULL!");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        CameraController.OnIdleCamera += OnCameraIdling;
        CameraController.OnOutIdleCamera += OnOutOfIdleCamera;
        LevelManager.OnStartLoadingLevel += OnSceneTransition;
        LevelManager.OnLevelLoadingFinished += OnSceneTransitionComplete;
        PauseMenu.OnPauseMenuOpened += OnGamePaused;
        PauseMenu.OnPauseMenuClosed += OnGameResumed;
    }

    private void OnSceneTransitionComplete()
    {
        if (LevelManager.Instance.GetCurrentLevel() == 1)
            SetAudioState("Garage", 2);
        else
        {
            SetAudioState("Default", 2);
        }
    }

    private void OnGamePaused() => SetAudioState("Frontend Only", 2);
    private void OnGameResumed() => SetAudioState("Default", 1);
    private void OnSceneTransition() => SetAudioState("Scene Transition", 2);
    private void OnOutOfIdleCamera() => SetAudioState("Default", 2);
    private void OnCameraIdling() => SetAudioState("Idle Camera", 2);

    private void OnDisable()
    {
        CameraController.OnIdleCamera -= OnCameraIdling;
        CameraController.OnOutIdleCamera -= OnOutOfIdleCamera;
        LevelManager.OnStartLoadingLevel -= OnSceneTransition;
        LevelManager.OnLevelLoadingFinished -= OnSceneTransitionComplete;
        PauseMenu.OnPauseMenuOpened -= OnGamePaused;
        PauseMenu.OnPauseMenuClosed -= OnGameResumed;
    }

    public void SetAudioState(string stateName, float timeToReach)
    {
        if (audioControllerData && audioControllerData.TryGetAudioState(stateName, out var state))
        {
            state.snapshot.TransitionTo(timeToReach);
            Debug.Log("Transitioning to audio state : " + stateName);
        }
    }
}