using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RaceSystem;

//Frontend Manager
public class FEManager : MonoBehaviour
{
    public static FEManager instance;

    public Leaderboard FE_Leaderboard;

    public NotificationSystem FE_Notifications;

    public Transitions FE_Transitions;

    public GameObject FE_QuitGamePrompt;

    [SerializeField] FEInput frontEndInput;

    public bool IsReadingInput;

    public FEInput GetFrontEndInput() => frontEndInput;

    private void Awake()
    {
        if (instance is null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
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
    }

    private void Update()
    {
        if (IsReadingInput)
            frontEndInput.ReadInput();
    }

    //FRONT END METHODS

    public void ShowExitGamePrompt() => FE_QuitGamePrompt.SetActive(true);

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
    public bool PauseButton { get; private set; }
    public bool SelectButton { get; private set; }
    public bool IsPressingAnyKey { get; private set; }
    public Vector2 DirectionalInput { get; private set; }

    public void ReadInput()
    {
        IsPressingAnyKey = Input.anyKeyDown;
        PauseButton = Input.GetKeyDown(KeyCode.Escape);
        SelectButton = Input.GetKeyDown(KeyCode.Return);
        DirectionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
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