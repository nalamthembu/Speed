using UnityEngine;
using UnityEngine.SceneManagement;

[
    RequireComponent
    (
        typeof(GameStateMachine)
    )
]

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

    public void InitPlayer()
    {
        RacingHUD.Instance.SetMaxRev(Player.instance.Vehicle.Transmission.powerData.maxRPM);
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


    private void OnApplicationFocus(bool focus)
    {
        /*
        if (!focus) //Clicks on another window or something.
        {
            IsPaused = true;
        }
        */
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
                    if (SaveSystem.TrySave(Player.instance.Vehicle))
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

        if (IsInRace)
        {
            if (Player.instance == null || RacingHUD.Instance == null || Player.instance.Vehicle is null)
                return;
            

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IsPaused = !IsPaused;

                FEManager.instance.IsReadingInput = IsPaused;
            }

            GetCursorMode(out CursorLockMode lockState, out bool isVisible);

            if (lockState is not CursorLockMode.Confined && isVisible)
            {
                SetCursorMode(CursorLockMode.Confined, false);
            }

            RacingHUD.Instance.SetGearIndicator(Player.instance.Vehicle.Transmission.IsInReverse ? "R" : (Player.instance.Vehicle.Transmission.CurrentGear + 1).ToString());
            RacingHUD.Instance.SetSpeedometer(Player.instance.Vehicle.SpeedKMH);
            RacingHUD.Instance.SetRev(Player.instance.Vehicle.Engine.RPM);
        }
        else
        {
            GetCursorMode(out CursorLockMode lockState, out bool isVisible);

            if (lockState is not CursorLockMode.Confined && !isVisible)
            {
                SetCursorMode(CursorLockMode.Confined, true);
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