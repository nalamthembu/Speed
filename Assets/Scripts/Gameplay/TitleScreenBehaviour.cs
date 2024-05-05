using ThirdPersonFramework;
using ThirdPersonFramework.UserInterface;
using TMPro;
using UnityEngine;

public class TitleScreenBehaviour : BaseUI
{
    [SerializeField] TMP_Text m_TitleScreenText; //'Press Any Key' Text

    bool m_GameStarted = false;
    [SerializeField][Range(1, 10)] float m_AlphaOccsillationSpeed = 1;
    [SerializeField][Range(0, 1)] float m_MinAlpha, m_Intensity = 1;

    protected override void Start()
    {
        base.Start();

        Show();
    }

    protected override void Update()
    {
        if (!m_GameStarted)
        {
            m_TitleScreenText.alpha = Mathf.Clamp(m_Intensity * Mathf.Sin(Time.time * m_AlphaOccsillationSpeed), m_MinAlpha, 1);

            if (LevelManager.Instance != null && PlayerController.Instance && PlayerController.Instance.OnAnyKey)
            {
                LevelManager.Instance.LoadLevel(1);

                m_GameStarted = true;

                Hide(true);

                return;
            }
        }
    }
}