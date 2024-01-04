using TMPro;
using UnityEngine;

public class TitleScreenBehaviour : MonoBehaviour
{
    [SerializeField] TMP_Text m_TitleScreenText; //'Press Any Key' Text

    bool m_GameStarted = false;
    [SerializeField][Range(1, 10)] float m_AlphaOccsillationSpeed = 1;
    [SerializeField][Range(0, 1)] float m_MinAlpha, m_Intensity = 1;

    private void Start()
    {
        if (FEManager.instance is null)
        {
            Debug.LogError("There is no Frontend Manager in this scene!");
            return;
        }
    }

    private void Update()
    {
        if (!m_GameStarted)
        {
            m_TitleScreenText.alpha = Mathf.Clamp(m_Intensity * Mathf.Sin(Time.time * m_AlphaOccsillationSpeed), m_MinAlpha, 1);

            if (FEManager.instance != null)
            {
                if (FEManager.instance.GetFrontEndInput().IsPressingAnyKey)
                {
                    if (LevelManager.Instance != null)
                    {
                        LevelManager.Instance.LoadLevel(1);

                        m_GameStarted = true;

                        //Disable whole title Screen.
                        gameObject.SetActive(false);

                        return;
                    }
                }
            }
        }
    }
}
