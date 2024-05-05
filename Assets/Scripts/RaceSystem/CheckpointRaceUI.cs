using System.Collections;
using TMPro;
using UnityEngine;


/*
 * [PURPOSE]
 * This handles the system that shows 
 * the player how much time they have left after
 * they collect a checkpoint.
 */
[System.Serializable]
public class CheckpointCollectFeedbackUI
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] TMP_Text m_RemainingTimeText;
    [SerializeField] TMP_Text m_AdditionalTimeText;
    [SerializeField] float m_VisibleTime = 5; // HOW LONG should this be on the screen for?

    // TRIGGERED BY AN EVENT
    public IEnumerator GiveFeedbackToPlayer(float additional, float remaining)
    {
        m_AdditionalTimeText.text = additional.GetFloatStopWatchFormat();

        m_CanvasGroup.alpha = 1;

        float timer = 0;

        while (timer < m_VisibleTime)
        {
            m_RemainingTimeText.text = remaining.GetFloatStopWatchFormat();

            timer += Time.deltaTime;

            yield return null;
        }

        m_CanvasGroup.alpha = 0;
    }
}

/*
 * [PURPOSE]
 * This handles all the UI pertaining to 
 * Checkpoint races.
 */

public class CheckpointRaceUI : BaseRaceUI
{
    [SerializeField] TMP_Text m_CheckpointCountText;
    [SerializeField] CheckpointCollectFeedbackUI m_CollectFeedbackUI;

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
            m_CollectFeedbackUI.GiveFeedbackToPlayer(m_AdditionalTime, CheckpointRace.Instance.GetRemainingTime());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckpointRace.OnCheckpointUpdate += SetCheckpointCount;
        CheckpointObject.OnPlayerPastCheckpoint += SetAdditionalTime;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CheckpointRace.OnCheckpointUpdate -= SetCheckpointCount;
    }
    private void SetAdditionalTime(float additionalTime) => m_AdditionalTime = additionalTime;

}