using TMPro;
using UnityEngine;

/*
 * [PURPOSE]
 * This handles all the UI pertaining to 
 * Checkpoint races.
 */

public class CheckpointRaceUI : BaseRaceUI
{
    [SerializeField] TMP_Text m_CheckpointCountText;
    [SerializeField] TMP_Text m_RemainingTimeText;

    public static CheckpointRaceUI Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    float m_AdditionalTime;

    public void SetCheckpointCount(int collected, int total)
    {
        m_CheckpointCountText.text = $"{collected}/{total}";

        // Check if theres any change
        if (collected > 0)
            StartCoroutine(CheckpointCollectFeedbackUI.Instance.GiveFeedbackToPlayer(m_AdditionalTime));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckpointRace.OnCheckpointUpdate += SetCheckpointCount;
        CheckpointObject.OnPlayerPastCheckpoint += SetAdditionalTime;
    }

    protected override void OnGamePaused()
    {
        base.OnGamePaused();
        Hide();
    }

    protected override void OnGameResumed()
    {
        base.OnGameResumed();
        Show(true);
    }

    

    protected override void OnDisable()
    {
        base.OnDisable();
        CheckpointRace.OnCheckpointUpdate -= SetCheckpointCount;
        CheckpointObject.OnPlayerPastCheckpoint -= SetAdditionalTime;
    }
    private void SetAdditionalTime(float additionalTime) => m_AdditionalTime = additionalTime;

    protected override void Update()
    {
        base.Update();

        if (m_GamePaused)
            return;

        if (CheckpointRace.Instance && CheckpointRace.Instance.RaceStarted && !CheckpointRace.Instance.RaceEnded)
        {
            float remainingTime = CheckpointRace.Instance.GetRemainingTime();

            if (remainingTime <= 10 && remainingTime > 5)
                m_RemainingTimeText.color = Color.yellow;
            else if (remainingTime <= 5)
                m_RemainingTimeText.color = Color.red;

            m_RemainingTimeText.text = remainingTime.GetFloatStopWatchFormat();
        }
    }

    protected override void OnPlayerWinRace()
    {
        base.OnPlayerWinRace();

        Hide();
    }

    protected override void OnPlayerLostRace()
    {
        base.OnPlayerLostRace();

        Hide();
    }
}