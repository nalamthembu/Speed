using TMPro;
using UnityEngine;
using ThirdPersonFramework.UserInterface;
using ThirdPersonFramework;

public class TitleScreen : BaseUI
{
    [SerializeField] TMP_Text m_TitleScreenText; //'Press Any Key' Text
    [SerializeField][Range(1, 10)] float m_AlphaOccsillationSpeed = 1;
    [SerializeField][Range(0, 1)] float m_MinAlpha, m_Intensity = 1;

    private bool m_StartedGame = false;

    protected override void Start()
    {
        Show();

        if (!PlayerController.Instance)
        {
            Debug.LogError("There is no player controller in the scene!");
        }
    }


    protected override void Update()
    {
        if (m_StartedGame)
            return;

        m_TitleScreenText.alpha = Mathf.Clamp(m_Intensity * Mathf.Sin(Time.time * m_AlphaOccsillationSpeed), m_MinAlpha, 1);

        WaitForInput();
    }

    private void WaitForInput()
    {
        if (PlayerController.Instance && PlayerController.Instance.AnyKeyPressed)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LoadLevel(1);

                m_StartedGame = true;

                Hide();

                return;
            }
        }
    }
}