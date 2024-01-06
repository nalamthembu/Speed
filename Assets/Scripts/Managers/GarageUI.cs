using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GarageUI : MonoBehaviour
{
    Dictionary<string, GarageMenu> m_GarageMenuDict;

    [SerializeField] string m_InitialContext;

    private GarageMenu m_CurrentContext;

    private CameraViewManager m_CamViewManager;

    [SerializeField] GarageMenu[] m_GarageMenus;

    private AudioSource sourceUI;

    public string GetContextID()
    {
        return m_CurrentContext.ToString();
    }

    public void PlayUISound()
    {
        SoundManager.instance.PlaySound("BTNFX_CLICK", sourceUI);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }

    private void Awake()
    {
        if (m_CamViewManager is null)
        {
            m_CamViewManager = GetComponent<CameraViewManager>();
        }

        sourceUI = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        m_GarageMenuDict = new();

        for (int i = 0; i < m_GarageMenus.Length; i++)
        {
            m_GarageMenuDict.Add(m_GarageMenus[i].GetMenuID(), m_GarageMenus[i]);
        }

        if (m_InitialContext == string.Empty)
        {
            Debug.LogError("Initial Context cannot be empty!");

            return;
        }

        SetContext(m_InitialContext);
    }

    public void SetContext(string menuID)
    {
        if (m_CurrentContext != null)
        {
            StartCoroutine(m_CurrentContext.SetActive(false));
        }

        if (m_GarageMenuDict.TryGetValue(menuID, out GarageMenu menu))
        {
            m_CurrentContext = menu;
            StartCoroutine(m_CurrentContext.SetActive(true));
            m_CamViewManager.GoToCameraView(menuID);
        }
        else
        {
            Debug.LogError("Could not find menu ID : " + menuID);
        }
    }
}

[System.Serializable]
public class GarageMenu
{
    [SerializeField] string m_MenuID;
    [SerializeField] CanvasGroup m_CanvasGroup;

    public bool IsFadingOut { get; private set; }
    public bool IsActive { get; private set; }

    public string GetMenuID() => m_MenuID;

    public IEnumerator SetActive(bool Value)
    {
        IsActive = Value;

        if (IsActive) 
        {
            float velToOne = 0;

            m_CanvasGroup.gameObject.SetActive(IsActive);

            while (m_CanvasGroup.alpha < 1)
            {
                m_CanvasGroup.alpha = Mathf.SmoothDamp(m_CanvasGroup.alpha, 1, ref velToOne, 0.1F);

                if (m_CanvasGroup.alpha > 0.99F && Mathf.Ceil(m_CanvasGroup.alpha) >= 1)
                {
                    m_CanvasGroup.alpha = 1;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            float velToZero = 0;

            while (m_CanvasGroup.alpha > 0)
            {
                m_CanvasGroup.alpha = Mathf.SmoothDamp(m_CanvasGroup.alpha, 0, ref velToZero, 0.1F);

                if (m_CanvasGroup.alpha < 0.01F && Mathf.Floor(m_CanvasGroup.alpha) <= 0)
                {
                    m_CanvasGroup.alpha = 0;
                }

                yield return new WaitForEndOfFrame();
            }

            m_CanvasGroup.gameObject.SetActive(IsActive);
        }
    }
}
