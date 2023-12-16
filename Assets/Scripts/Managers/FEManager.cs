using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RaceSystem;

//Frontend Manager
public class FEManager : MonoBehaviour
{
    public static FEManager instance;

    public Racing FE_Racing;

    public Leaderboard FE_Leaderboard;

    public NotificationSystem FE_Notifications;

    public Transitions FE_Transitions;

    public GarageMenu garageMenu;

    public GameObject FE_QuitGamePrompt;

    [SerializeField] FEInput frontEndInput;

    public bool IsReadingInput;

    private void Awake()
    {
        if (instance is null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        //FADE FROM BLACK
        StartCoroutine(MakeVisible(FE_Transitions.canvasGroup, false));

        //SET MAIN MENU IS MENU CONTEXT IF IN GARAGE
        if (GameManager.instance.IsInGarage)
        {
            SetGarageMenuContext(garageMenu.submenus[0].id);
        }
        else
        {
            FE_Racing.Start();
        }

    }

    private void Update()
    {
        if (GameManager.instance.IsInRace)
        {
            if (!FE_Racing.Visible)
            {
                FE_Racing.Visible = true;

                StartCoroutine(MakeVisible(FE_Racing.canvasGroup, true));
            }

            //UPDATE FE_RACING_UI

            FE_Racing.Update();
        }

        frontEndInput.ReadInput();

        if (frontEndInput.pauseButton)
        {
            FE_Racing.pausedMenu.SetActive(!FE_Racing.pausedMenu.activeSelf);

            //HIDE OTHER RACING UI
        }

        //GARAGE MENUS
        if (GameManager.instance.IsInGarage)
        {
            for (int i = 0; i < garageMenu.submenus.Length; i++)
            {
                if (garageMenu.submenus[i].id.Equals(garageMenu.ContextID))
                {
                    if (garageMenu.submenus[i].isActive)
                    {
                        //RESIZING
                        if (!IsDoneResizing(ref garageMenu.transform, garageMenu.submenus[i].size))
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }

    //FRONT END METHODS

    public void ShowExitGamePrompt() => FE_QuitGamePrompt.SetActive(true);

    public void SetGarageMenuContext(string menuID)
    {
        for (int i = 0; i < garageMenu.submenus.Length; i++)
        {
            if (garageMenu.submenus[i].id == garageMenu.ContextID)
            {
                garageMenu.submenus[i].isActive = false;

                for (int j = 0; j < garageMenu.submenus[i].menuElements.Length; j++)
                {
                    garageMenu.submenus[i].menuElements[j].SetActive(false);
                }

                StartCoroutine(MakeVisible(garageMenu.submenus[i].canvasGroup, false));

                break;
            }
        }

        garageMenu.ContextID = menuID;

        for (int i = 0; i < garageMenu.submenus.Length; i++)
        {
            if (garageMenu.submenus[i].id == garageMenu.ContextID)
            {
                garageMenu.submenus[i].isActive = true;

                for (int j = 0; j < garageMenu.submenus[i].menuElements.Length; j++)
                {
                    garageMenu.submenus[i].menuElements[j].SetActive(true);
                }

                StartCoroutine(MakeVisible(garageMenu.submenus[i].canvasGroup, true));

                GarageManager.instance.GoToCameraView(menuID + "_view");

                break;
            }
        }
    }

    Vector2 sizeDeltaVel;

    private bool IsDoneResizing(ref RectTransform rectTransform, Vector2 newSize)
    {
        if (rectTransform.sizeDelta != newSize)
        {
            rectTransform.sizeDelta = Vector2.SmoothDamp(rectTransform.sizeDelta, newSize, ref sizeDeltaVel, 0.5F);

            return false;
        }

        return true;
    }

    public void UpdateText(ref TMP_Text toChange, string newValue) => toChange.text = newValue;

    public void ShowNotification(string message) => StartCoroutine(IEShowNotification(message));

    public IEnumerator IEShowNotification(string message)
    {
        yield return new WaitForEndOfFrame();

        FE_Notifications.notificationText.text = message;

        while (FE_Notifications.canvasGroup.alpha != 1)
        {
            FE_Notifications.canvasGroup.alpha += Time.deltaTime;

            if (FE_Notifications.canvasGroup.alpha >= 0.99F)
            {
                FE_Notifications.canvasGroup.alpha = Mathf.Ceil(FE_Notifications.canvasGroup.alpha);
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(4.0F);

        yield return new WaitForEndOfFrame();

        while (FE_Notifications.canvasGroup.alpha != 0)
        {
            FE_Notifications.canvasGroup.alpha -= Time.deltaTime;

            if (FE_Notifications.canvasGroup.alpha <= 0.01F)
            {
                FE_Notifications.canvasGroup.alpha = Mathf.Floor(FE_Notifications.canvasGroup.alpha);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MakeVisible(CanvasGroup FE_Panel, bool value)
    {
        yield return new WaitForEndOfFrame();

        if (value is true)
        {
            while (FE_Panel.alpha != 1)
            {
                FE_Panel.alpha += Time.deltaTime;

                if (FE_Panel.alpha >= 0.99F)
                {
                    FE_Panel.alpha = Mathf.Ceil(FE_Panel.alpha);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        if (value is false)
        {
            yield return new WaitForEndOfFrame();

            while (FE_Panel.alpha != 0)
            {
                FE_Panel.alpha -= Time.deltaTime;

                if (FE_Panel.alpha <= 0.01F)
                {
                    FE_Panel.alpha = Mathf.Floor(FE_Panel.alpha);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public IEnumerator MakeTextVisible(TMP_Text text, bool value)
    {
        yield return new WaitForEndOfFrame();

        if (value is true)
        {
            while (text.alpha != 1)
            {
                text.alpha += Time.deltaTime;

                if (text.alpha >= 0.99F)
                {
                    text.alpha = Mathf.Ceil(text.alpha);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        if (value is false)
        {
            yield return new WaitForEndOfFrame();

            while (text.alpha != 0)
            {
                text.alpha -= Time.deltaTime;

                if (text.alpha <= 0.01F)
                {
                    text.alpha = Mathf.Floor(text.alpha);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}

[System.Serializable]
public struct FEInput
{
    public bool pauseButton;
    public bool selectButton;
    public Vector2 directionalInput;

    public void ReadInput()
    {
        pauseButton = Input.GetKeyDown(KeyCode.Escape);
        selectButton = Input.GetKeyDown(KeyCode.Return);
        directionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}

//VARIOUS USER INTERFACES BELOW
[System.Serializable]
public struct Racing
{
    [Header("Tach UI")]

    public const float MAX_REV_ANGLE = -135;
    public const float ZERO_REV_ANGLE = 135;

    public float maxRev;
    public float currentRev;

    public Transform RevLevelTemplate;
    public Transform tachNeedle;
    public TMP_Text speedometer;
    public TMP_Text gearIndicator;

    [Header("Mini Map")]
    public Camera minimapCamera;
    private Vehicle playerVehicle;
    public Transform vehicleIconTemplate;
    private List<Transform> vehicleIcons;
    [SerializeField] float miniMapHeight;
    [SerializeField][Range(1, 500)] float miniMapMinScale;
    [SerializeField][Range(1, 500)] float miniMapMaxScale;
    [SerializeField][Range(1, 100)] float speedAtMaxScale;


    public void Start()
    {
        InitialiseRevCounter();

        FindPlayerAndAttachMiniMap();

        InitialiseVehicleIcons();
    }

    private void InitialiseVehicleIcons()
    {
        vehicleIconTemplate.gameObject.SetActive(false);

        vehicleIcons = new();

        Racer[] racers = Object.FindObjectsOfType<Racer>();

        for (int i = 0; i < racers.Length; i++)
        {
            Transform icon = 
                Object.Instantiate(
                    vehicleIconTemplate,
                    racers[i].Vehicle.transform.position + Vector3.up,
                    racers[i].Vehicle.transform.rotation,
                    racers[i].Vehicle.transform
                    );

            if (icon.TryGetComponent<SpriteRenderer>(out var sprite))
            {

                sprite.color = i switch
                {
                    0 => Color.red,
                    1 => Color.green,
                    2 => Color.yellow,
                    3 => Color.blue,
                    4 => Color.black,
                    5 => Color.magenta,
                    6 => Color.cyan,
                    7 => Color.white,
                    8 => Color.gray,
                    _ => Color.white
                };
            }

            icon.eulerAngles = Vector3.right * 90 + Vector3.up * 180;

            icon.gameObject.SetActive(true);

            vehicleIcons.Add(icon);
        }
    }

    private void InitialiseRevCounter()
    {
        currentRev = 0;
        maxRev = 10000;

        RevLevelTemplate.gameObject.SetActive(false);

        CreateRevLabels();
    }

    private void FindPlayerAndAttachMiniMap()
    {
        playerVehicle = Player.instance.Vehicle;
    }

    private void CreateRevLabels()
    {
        int labelCount = 10;

        float totalAngleSize = ZERO_REV_ANGLE - MAX_REV_ANGLE;

        for (int i = 0; i <= labelCount; i++)
        {
            Transform revLabelTransform = Object.Instantiate(RevLevelTemplate, tachNeedle.parent);

            float labelRevNormalised = (float)i / labelCount;

            float revLabelAngle = ZERO_REV_ANGLE - labelRevNormalised * totalAngleSize;

            revLabelTransform.eulerAngles = new Vector3(0, 0, revLabelAngle);

            revLabelTransform.GetComponentInChildren<TMP_Text>().text = Mathf.RoundToInt(labelRevNormalised * maxRev / 1000.0F).ToString();

            revLabelTransform.GetComponentInChildren<TMP_Text>().transform.eulerAngles *= 0;

            revLabelTransform.gameObject.SetActive(true);
        }
    }

    public void SetRev(float currentRev)
    {
        this.currentRev = currentRev;

        if (this.currentRev > maxRev) this.currentRev = maxRev;
    }

    public void Update()
    {
        float angle = tachNeedle.eulerAngles.z;

        float targetAngle = GetRevRotation();

        float finalAngle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * 6.0F);

        tachNeedle.eulerAngles = new Vector3(0, 0, finalAngle);

        TrackPlayerWithMiniMapCamera();
    }

    private void TrackPlayerWithMiniMapCamera()
    {
        minimapCamera.transform.position = playerVehicle.transform.position + Vector3.up * miniMapHeight;
        minimapCamera.transform.eulerAngles = Vector3.right * 90 + Vector3.forward * playerVehicle.transform.eulerAngles.y;
        minimapCamera.orthographicSize = Mathf.Lerp(miniMapMinScale, miniMapMaxScale, playerVehicle.SpeedKMH / speedAtMaxScale);
    }

    private float GetRevRotation()
    {
        float totalAngleSize = ZERO_REV_ANGLE - MAX_REV_ANGLE;

        float revNormalised = currentRev / maxRev;

        return ZERO_REV_ANGLE - revNormalised * totalAngleSize;
    }

    [Header("Game UI")]
    public GameObject pausedMenu;
    public CanvasGroup canvasGroup;
    public TMP_Text countdownTimer;

    public bool Visible;
}

[System.Serializable]
public struct Leaderboard
{
    public TMP_Text playerPosition;
    public TMP_Text raceProgress;
    public TMP_Text timeElapsed;
    public CanvasGroup canvasGroup;

    public bool Visible;
}

[System.Serializable]
public struct NotificationSystem
{
    public TMP_Text notificationText;
    public CanvasGroup canvasGroup;
}

[System.Serializable]
public struct Transitions //BLACK_SCREEN
{
    public CanvasGroup canvasGroup;
}

[System.Serializable]
public struct GarageMenu
{
    public TMP_Text menuTitle;
    public CanvasGroup canvasGroup;

    public GarageSubMenu[] submenus;

    public string ContextID { get; set; }
    public RectTransform transform;
}

[System.Serializable]
public struct GarageSubMenu
{
    public string name;
    public string id;
    public Vector2 size;
    public GameObject[] menuElements;
    public GameObject[] importantElements;
    public CanvasGroup canvasGroup;
    public bool isActive { get; set; }
}