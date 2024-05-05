using UnityEngine;
using ThirdPersonFramework.UserInterface;
using TMPro;

public class RaceCountdownUI : BaseGameMenu
{
    [SerializeField] TMP_Text m_CountdownText;

    private bool m_ShowCountdown;

    public static RaceCountdownUI Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetCountdownShowing(bool value) => m_ShowCountdown = value;

    public void SetCountdownText(string text)
    {
        m_CountdownText.text = text;

        Hide(true);
    }

    protected override void Update()
    {
        base.Update();

        if (m_ShowCountdown)
        {
            if (IsHidden())
                Show();
        }
        else //Hide if not in use
        {
            if (!IsHidding)
                Hide();
        }
    }
}