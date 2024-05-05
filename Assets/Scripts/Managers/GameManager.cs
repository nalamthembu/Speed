using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused;
    public bool IsInRace;
    public bool IsInMenu;

    public bool DEBUG_MODE = false;

    public const float SKIDMARK_LIFETIME = 60.0F;

    public GameObject SKIDMARK_PARENTOBJ { get; private set; }

    private void Awake()
    {
        if (Instance is null)
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

    private void OnEnable() => SceneManager.sceneLoaded += CheckIfIsInMenuLevel;

    private void OnDisable() => SceneManager.sceneLoaded -= CheckIfIsInMenuLevel;


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

    public void GetCursorMode(out CursorLockMode lockState, out bool isVisible)
    {
        lockState = Cursor.lockState;

        isVisible = Cursor.visible;
    }

    public void SetCursorMode(CursorLockMode lockState, bool visible)
    {
        Cursor.lockState = lockState;

        Cursor.visible = visible;
    }

    public void OnPlayerQuitGame()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }

    internal void OnCancelQuitGame()
    {
        // TODO : DO NOTHING REALLY BUT
        // MAYBE SOMETHING SHOULD BE HERE
    }
}