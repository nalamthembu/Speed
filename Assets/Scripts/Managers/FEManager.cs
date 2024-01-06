using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RaceSystem;

//Frontend Manager
public class FEManager : MonoBehaviour
{
    public static FEManager instance;

    [SerializeField] PauseMenu m_PauseMenu;

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

        if (GameManager.Instance.IsInRace)
            m_PauseMenu.Update();
    }

    private void OnEnable()
    {
        m_PauseMenu.OnEnable();
    }

    private void OnDisable()
    {
        m_PauseMenu.OnDisable();
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
public class PauseMenu
{
    private bool m_Enabled;

    [SerializeField] CanvasGroup m_CanvasGroup;

    private float m_AlphaVelocity;

    public void OnEnable()
    {
        GameStateMachine.Instance.OnIsPaused += OnPaused;
        GameStateMachine.Instance.OnIsRacing += OnResume;
    }

    public void OnDisable()
    {
        GameStateMachine.Instance.OnIsPaused -= OnPaused;
        GameStateMachine.Instance.OnIsRacing -= OnResume;
    }

    public void Update()
    {
        DoVisibility();
    }

    private void DoVisibility()
    {
        if (m_Enabled)
        {
            if (m_CanvasGroup.alpha <= 1)
            {
                m_CanvasGroup.alpha = Mathf.SmoothDamp(m_CanvasGroup.alpha, 1, ref m_AlphaVelocity, 0.5F);

                if (m_CanvasGroup.alpha + 0.1F >= 1)
                {
                    m_CanvasGroup.alpha = 1;

                    m_CanvasGroup.interactable = true;
                }
            }
        }
        else
        {
            if (m_CanvasGroup.alpha > 0)
            {
                m_CanvasGroup.alpha = Mathf.SmoothDamp(m_CanvasGroup.alpha, 0, ref m_AlphaVelocity, 0.5F);

                if (m_CanvasGroup.alpha - 0.1F <= 0)
                {
                    m_CanvasGroup.alpha = 0;

                    m_CanvasGroup.interactable = false;
                }
            }
        }
    }


    private void OnResume()
    {
        m_Enabled = false;
        m_CanvasGroup.gameObject.SetActive(m_Enabled);
    }

    private void OnPaused()
    {
        m_Enabled = true;
        m_CanvasGroup.gameObject.SetActive(m_Enabled);
    }
}