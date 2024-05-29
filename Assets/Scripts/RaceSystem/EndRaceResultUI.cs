using ThirdPersonFramework.UserInterface;
using UnityEngine;
using TMPro;
using System;

/*
 * [PURPOSE]
 * This handles the screen that comes up
 * at the end of a race and displays the 
 * appropriate output (WIN|LOSS)
 */

public class EndRaceResultUI : BaseUI
{
    [SerializeField] TMP_Text m_ResultText;
    [SerializeField] TMP_Text m_TimeElapsedText; 
    [SerializeField] TMP_Text m_TimeRemainingText; // Pretty much only used by Checkpoint Races

    public static event Action OnResultScreenShown;
    public static event Action OnResultScreenCleared;

    BaseRace m_CurrentRace;

    protected override void Awake()
    {
        base.Awake();

        m_CurrentRace = FindAnyObjectByType<BaseRace>();

        Hide();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BaseRace.OnPlayerWonRace -= OnPlayerWonRace;
        BaseRace.OnPlayerLostRace -= OnPlayerLostRace;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        BaseRace.OnPlayerWonRace += OnPlayerWonRace;
        BaseRace.OnPlayerLostRace += OnPlayerLostRace;
    }

    protected override void OnGamePaused() => Hide();

    protected override void OnGameResumed()
    {
        base.OnGameResumed();

        if (m_CurrentRace && m_CurrentRace.RaceEnded)
            Show();
    }

    private void OnPlayerLostRace()
    {
        m_ResultText.text = "You Lose!";

        m_ResultText.color = Color.red;

        m_TimeElapsedText.text = m_CurrentRace.GetTimeElasped().GetFloatStopWatchFormat();

        CheckpointRace checkpointRace = (CheckpointRace)m_CurrentRace;
        m_TimeRemainingText.gameObject.SetActive(checkpointRace != null);

        if (checkpointRace != null)
            m_TimeRemainingText.text = checkpointRace.GetRemainingTime().GetFloatStopWatchFormat();

        Show();
    }

    public override void Hide(bool quick = false)
    {
        base.Hide(quick);

        OnResultScreenCleared?.Invoke();
    }

    public override void Show(bool quick = false)
    {
        base.Show(quick);

        OnResultScreenShown?.Invoke();
    }

    private void OnPlayerWonRace()
    {
        m_ResultText.text = "You Win!";

        m_ResultText.color = Color.green;

        m_TimeElapsedText.text = m_CurrentRace.GetTimeElasped().GetFloatStopWatchFormat();

        CheckpointRace checkpointRace = (CheckpointRace)m_CurrentRace;
        m_TimeRemainingText.gameObject.SetActive(checkpointRace != null);

        if (checkpointRace != null)
            m_TimeRemainingText.text = checkpointRace.GetRemainingTime().GetFloatStopWatchFormat();

        Show();
    }
}