using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] LoadingScreen m_LoadingScreen;

    public const int GARAGE_LEVEL = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public int GetCurrentLevel() => SceneManager.GetActiveScene().buildIndex;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (MusicManager.instance != null)
        {
            //I think its weird to just go into another scene with the same song.
            MusicManager.instance.PlayRandomSong();
            MusicManager.instance.FadeInCurrentSong();
        }
    }

    public void LoadLevel(int index)
    {
        StartCoroutine(LoadLevel_Coroutine(index));
    }

    IEnumerator LoadLevel_Coroutine(int index)
    {
        //SETUP_LOADING_SCREEN
        m_LoadingScreen.SetEnabled(true);

        while (m_LoadingScreen.GetCanvasGroupAlpha() < 1)
        {
            float currAlpha = m_LoadingScreen.GetCanvasGroupAlpha();

            currAlpha += Time.deltaTime / 2;

            m_LoadingScreen.SetCanvasGroupAlpha(currAlpha);

            if (currAlpha + 0.01F >= 1)
            {
                currAlpha = 1;

                m_LoadingScreen.SetCanvasGroupAlpha(currAlpha);

                break;
            }

            yield return new WaitForEndOfFrame();
        }

        MusicManager.instance.FadeOutCurrentSong();

        while (MusicManager.instance.IsFadingOut)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);

        //END OF LOADING SCREEN SETUP


        //START LOADING GAME.
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            m_LoadingScreen.SetProgress(progress);
            yield return null;
        }

        print("**DEBUG : CURRENT LEVEL : " + SceneManager.GetActiveScene().name);

        //TAKE DOWN LOADING SCREEN.
        while (m_LoadingScreen.GetCanvasGroupAlpha() > 0)
        {
            float currAlpha = m_LoadingScreen.GetCanvasGroupAlpha();

            currAlpha -= Time.deltaTime;

            m_LoadingScreen.SetCanvasGroupAlpha(currAlpha);

            if (currAlpha - 0.01F <= 0)
            {
                currAlpha = 0;

                m_LoadingScreen.SetCanvasGroupAlpha(currAlpha);

                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);

        m_LoadingScreen.SetEnabled(false);

        MusicManager.instance.FadeInCurrentSong();
    }
}


[System.Serializable]
public class LoadingScreen
{
    [SerializeField] GameObject m_LoadingScreenObject;

    [SerializeField] CanvasGroup m_CanvasGroup;

    [SerializeField] Slider m_ProgressBar;

    public void SetCanvasGroupAlpha(float value) => m_CanvasGroup.alpha = value;

    public float GetCanvasGroupAlpha() => m_CanvasGroup.alpha;

    public void SetProgress(float amount) => m_ProgressBar.value = amount;

    public float GetProgress() => m_ProgressBar.value;

    public void SetEnabled(bool value) => m_LoadingScreenObject.SetActive(value);
}