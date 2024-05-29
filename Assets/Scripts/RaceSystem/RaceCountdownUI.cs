using UnityEngine;
using ThirdPersonFramework.UserInterface;
using TMPro;


public class RaceCountdownUI : BaseGameMenu
{
    [SerializeField] TMP_Text m_CountdownText;

    public static RaceCountdownUI Instance;

    BaseRace m_CurrentRace;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }

        Hide();

        m_CurrentRace = FindAnyObjectByType<BaseRace>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BaseRace.OnRaceInitialised -= OnRaceInit;
        BaseRace.OnRaceStarted -= OnRaceStarted;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BaseRace.OnRaceInitialised += OnRaceInit;
        BaseRace.OnRaceStarted += OnRaceStarted;
    }

    private void OnRaceStarted() => StartCoroutine(HideAfterDelay(3));
    private void OnRaceInit() => Show();

    protected override void OnGamePaused()
    {
        base.OnGamePaused();

        Hide();
    }

    protected override void OnGameResumed()
    {
        base.OnGameResumed();

        if (m_CurrentRace && !m_CurrentRace.RaceStarted)
            Show();
    }

    public void SetCountdownNumber(float countdownNumber)
    {
        string final_string;

        if (countdownNumber <= 0)
            final_string = "GO!";
        else
            final_string = countdownNumber.ToString();

        m_CountdownText.text = final_string;

        Hide(true);
    }

    protected override void Update()
    {
        base.Update();

        if (!m_CurrentRace.RaceStarted)
        {
            if (m_CurrentRace && IsHidden() || Mathf.Ceil(m_CanvasGroup.alpha) <= 1)
                Show();
        }
    }
}