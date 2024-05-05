using TMPro;
using UnityEngine;
using System.Collections;
using ThirdPersonFramework.UserInterface;
/*
 * [PURPOSE]
 * This handles the system that shows 
 * the player how much time they have left after
 * they collect a checkpoint.
 */

[System.Serializable]
public class CheckpointCollectFeedbackUI : BaseUI
{
    [SerializeField] TMP_Text m_RemainingTimeText;
    [SerializeField] TMP_Text m_AdditionalTimeText;
    [SerializeField] float m_VisibleTime = 5; // HOW LONG should this be on the screen for?

    public static CheckpointCollectFeedbackUI Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
    }

    // TRIGGERED BY AN EVENT
    public IEnumerator GiveFeedbackToPlayer(float additional)
    {
        m_AdditionalTimeText.text = additional.GetFloatStopWatchFormat();

        Show();

        float timer = 0;

        while (timer < m_VisibleTime)
        {
            m_RemainingTimeText.text = CheckpointRace.Instance.GetRemainingTime().GetFloatStopWatchFormat();

            timer += Time.deltaTime;

            if (timer <= m_VisibleTime - 1)
            {
                Hide();
                break;
            }

            yield return null;
        }
    }
}