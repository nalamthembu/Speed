using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    [SerializeField] MusicScriptable musicScriptable;

    [SerializeField] MusicManagerUI musicUI;

    private int nowPlaying = -1;

    private AudioSource source;

    public bool DEBUG_MODE = false;

    public static MusicManager instance;

    private Song NowPlaying;

    public event Action OnSongChanged;

    public bool IsFadingOut { get; set; }

    public bool IsFadingIn { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        source = gameObject.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = musicScriptable.musicMixerGroup;

        source.bypassReverbZones = true;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayRandomSong();
    }

    public Song GetNowPlaying() => NowPlaying;

    public int GetTotalMusicCount() => musicScriptable.music.Length;

    public Song[] GetAllSongs() => musicScriptable.music;

    public float GetSeekTime() => source.time;

    public void SetSeekTime(Slider slider)
    {
        slider.value = Mathf.Clamp(slider.value, 0, source.clip.length);

        source.time = slider.value;
    }

    public void PlaySongByMetadata(string title, string artist)
    {
        foreach (Song song in musicScriptable.music)
        {
            if (song.artist == artist && song.title == title)
            {
                PlaySong(song);

                return;
            }
        }

        Debug.LogWarning("Could not find requested song " + title + " by " + artist);
    }

    public void PlayRandomSong()
    {
        nowPlaying = Random.Range(0, musicScriptable.music.Length - 1);

        int MAX_ITERATIONS = 100;

        if (GameManager.Instance.IsInRace)
        {
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                switch (musicScriptable.music[nowPlaying].type)
                {
                    case MUSIC_TYPE.MUSIC_BOTH:
                    case MUSIC_TYPE.MUSIC_RACING:

                        Song song = musicScriptable.music[nowPlaying];

                        PlaySong(song);

                        return;

                    default:

                        nowPlaying = Random.Range(0, musicScriptable.music.Length - 1);

                        break;
                }
            }

            Debug.LogWarning("Could not find a Race song and have run out of iterations");
        }
        else
        {
            Song song = musicScriptable.music[nowPlaying];

            PlaySong(song);
        }
    }

    public void FadeOutCurrentSong() => StartCoroutine(FadeOutCurrentSong_Coroutine());

    public void FadeInCurrentSong() => StartCoroutine(FadeInCurrentSong_Coroutine());

    private IEnumerator FadeInCurrentSong_Coroutine()
    {
        while (source.volume < 1)
        {
            IsFadingIn = true;

            source.volume += Time.deltaTime;

            if (source.volume + 0.01F >= 1)
            {
                source.volume = 1;

                IsFadingIn = false;

                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator FadeOutCurrentSong_Coroutine()
    {
        while(source.volume > 0)
        {
            IsFadingOut = true;

            source.volume -= Time.deltaTime / 4;

            if (source.volume - 0.01F <= 0)
            {
                source.volume = 0;

                IsFadingOut = false;

                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        if (DEBUG_MODE)
        {
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                NextSong();
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                PreviousSong();
            }
        }

        if (!source.isPlaying)
        {
            NextSong();
        }

        musicUI.Update();
    }

    public void NextSong()
    {
        nowPlaying++;

        if (nowPlaying > musicScriptable.music.Length - 1)
            nowPlaying = 0;

        while (musicScriptable.music[nowPlaying].type == MUSIC_TYPE.MUSIC_GARAGE && GameManager.Instance.IsInRace)
        {
            nowPlaying++;

            if (nowPlaying > musicScriptable.music.Length - 1)
                nowPlaying = 0;
        }

        Song song = musicScriptable.music[nowPlaying];

        PlaySong(song);
    }
    public void PreviousSong()
    {
        nowPlaying--;

        if (nowPlaying < 0)
            nowPlaying = musicScriptable.music.Length - 1;

        while (musicScriptable.music[nowPlaying].type == MUSIC_TYPE.MUSIC_GARAGE && GameManager.Instance.IsInRace)
        {
            nowPlaying--;

            if (nowPlaying < 0)
                nowPlaying = musicScriptable.music.Length - 1;
        }

        Song song = musicScriptable.music[nowPlaying];

        PlaySong(song);
    }

    //AUX_METHOD
    private void PlaySong(Song song)
    {
        source.clip = song.clip;

        musicUI.ShowInfo(song.artist, song.title);

        if (GameManager.Instance.IsInRace)
        {
            source.time = song.startTimeDuringRace;
        }

        source.Play();

        NowPlaying = song;

        OnSongChanged?.Invoke();
    }
}

[System.Serializable]
public class MusicManagerUI
{
    [SerializeField] GameObject m_Canvas;

    [SerializeField] CanvasGroup m_InfoPanel;

    [SerializeField] TMP_Text m_InfoText;

    [SerializeField][Range(1, 10)] float m_VisibilityTime = 5;

    float m_CurrentTimer = 0;

    bool m_bShowInfo = false;

    public void ShowInfo(string artist, string title)
    {
        m_InfoText.text = artist + "\n" + title;

        m_Canvas.SetActive(true);

        m_bShowInfo = true;
    }

    public void Update()
    {
        if (m_bShowInfo)
        {
            if (m_InfoPanel.alpha == 1)
            {
                m_CurrentTimer += Time.deltaTime;

                if (m_CurrentTimer >= m_VisibilityTime)
                {
                    m_Canvas.SetActive(false);

                    m_CurrentTimer = 0;

                    m_InfoPanel.alpha = 0;
                }
            }
            else
            {
                m_InfoPanel.alpha = Mathf.Lerp(m_InfoPanel.alpha, 1, Time.deltaTime);

                if (m_InfoPanel.alpha + 0.0125F >= 1)
                {
                    m_InfoPanel.alpha = 1;

                    m_Canvas.SetActive(false);
                }
            }
        }
    }
}
