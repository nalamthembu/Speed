using UnityEngine;
using RaceSystem;

[
    RequireComponent
    (
        typeof(GameStateMachine)
    )
]

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Player player;

    public bool IsPaused;
    public bool IsInRace;
    public bool IsInGarage;

    public bool DEBUG_MODE = false;

    public const float SKIDMARK_LIFETIME = 60.0F;

    public GameObject SKIDMARK_PARENTOBJ { get; private set; }

    private void Awake()
    {
        if (instance is null)
            instance = this;
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
    }

    public void InitPlayer()
    {
        FEManager.instance.FE_Racing.maxRev = Player.instance.Vehicle.Transmission.powerData.maxRPM;
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
        if (DEBUG_MODE)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SaveSystem.TrySave(player.Vehicle);

                Debug.LogWarning("** DEBUG : ATTEMPTING TO SAVE **");
            }
        }

        if (IsInRace)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //YOU CAN'T PAUSE UNTIL THE RACE STARTS.
                if (!Race.instance.RaceHasStarted && IsInRace)
                    return;

                IsPaused = !IsPaused;

                FEManager.instance.IsReadingInput = IsPaused;
            }

            GetCursorMode(out CursorLockMode lockState, out bool isVisible);

            if (lockState is not CursorLockMode.Confined && isVisible)
            {
                SetCursorMode(CursorLockMode.Confined, false);
            }

            FEManager.instance.FE_Racing.gearIndicator.text = Player.instance.Vehicle.Transmission.IsInReverse ? "R" : (Player.instance.Vehicle.Transmission.CurrentGear + 1).ToString();
            FEManager.instance.FE_Racing.speedometer.text = Mathf.Floor(Player.instance.Vehicle.SpeedKMH) + "KM/H";
            FEManager.instance.FE_Racing.currentRev = Player.instance.Vehicle.Engine.RPM;
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