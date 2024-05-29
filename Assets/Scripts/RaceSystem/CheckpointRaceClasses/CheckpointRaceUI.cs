using System.Collections;
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
    [SerializeField] TMP_Text m_AdditionalTimeText;
    [SerializeField] float m_AdditionalTextDuration = 3;

    public static CheckpointRaceUI Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }

        m_AdditionalTimeText.gameObject.SetActive(false);
    }

    public void SetCheckpointCount(int collected, int total)
    {
        m_CheckpointCountText.text = $"{collected}/{total}";
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckpointRace.OnCheckpointUpdate += SetCheckpointCount;
        CheckpointObject.OnPlayerPastCheckpoint += OnCheckpointCollected;
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
        CheckpointObject.OnPlayerPastCheckpoint -= OnCheckpointCollected;
    }

    void OnCheckpointCollected(float addedTime) => StartCoroutine(CheckpointCollectedCoroutine(addedTime));

    IEnumerator CheckpointCollectedCoroutine(float addedTime)
    {
        m_AdditionalTimeText.gameObject.SetActive(true);

        m_AdditionalTimeText.text = $"+{addedTime.GetFloatStopWatchFormat()}";

        yield return new WaitForSeconds(m_AdditionalTextDuration);

        m_AdditionalTimeText.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (CheckpointRace.Instance.RaceEnded)
        {
            if (!IsHidden())
                Hide(true);
            return;
        }

        if (m_GamePaused)
            return;

        if (CheckpointRace.Instance && CheckpointRace.Instance.RaceStarted && !CheckpointRace.Instance.RaceEnded)
        {
            float remainingTime = CheckpointRace.Instance.GetRemainingTime();

            if (remainingTime < 10)
            {
                if (remainingTime < 5)
                    m_RemainingTimeText.color = Color.red;
                else
                {
                    m_RemainingTimeText.color = Color.yellow;
                }
            }
            else
                m_RemainingTimeText.color = Color.white;

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