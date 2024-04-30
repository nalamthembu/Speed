using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using ThirdPersonFramework.UserInterface;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused;
    public bool IsInRace;
    public bool IsInMenu;

    public bool DEBUG_MODE = false;

    public const float SKIDMARK_LIFETIME = 60.0F;

    public GameObject SKIDMARK_PARENTOBJ { get; private set; }

    public static Action EventOnGamePaused;
    public static Action EventOnGameResume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitialiseVFX();
    }

    private void InitialiseVFX()
    {
        SKIDMARK_PARENTOBJ = new GameObject("SKID_MARK_VFX");
        DontDestroyOnLoad(SKIDMARK_PARENTOBJ);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfIsInMenuLevel;
        PauseMenu.OnPauseMenuOpened += OnGamePaused;
        PauseMenu.OnPauseMenuClosed += OnGameResume;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckIfIsInMenuLevel;
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

    private void CheckIfIsInMenuLevel(Scene scene, LoadSceneMode loadSceneMode)
    {
        print("** DEBUG ** : CHECKING LEVEL -> " + scene.name);

        bool WeAreInMenuLevel = LevelManager.Instance.GetCurrentLevel() == 0 || LevelManager.Instance.GetCurrentLevel() == 1;

        if (WeAreInMenuLevel)
        {
            IsInRace = false;

            IsPaused = false;

            IsInMenu = true;

            print("** DEBUG ** IS IN MENU/GARAGE");
        }
        else
        {
            IsInRace = true;

            IsPaused = false;

            IsInMenu = false;

            print("** DEBUG ** IS RACE");
        }

        MusicManager.instance.FadeInCurrentSong();
        MusicManager.instance.PlayRandomSong();
    }

    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (DEBUG_MODE)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    Debug.LogWarning("** DEBUG : ATTEMPTING TO SAVE **");
                    if (SaveSystem.TrySave(Player.Instance.Vehicle))
                        Debug.Log("** DEBUG : SAVED SUCESSFULLY **");
                    else
                        Debug.LogError("** DEBUG : SAVE FAILED! **");
                }
            }

            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Debug.developerConsoleVisible = !Debug.developerConsoleVisible;
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }
}