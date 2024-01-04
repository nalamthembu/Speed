using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayerMenu : MonoBehaviour
{
    [SerializeField] Transform m_MusicItemTemplate;

    [SerializeField] NowPlayingMenuItem m_NowPlayingItem;

    private void Start()
    {
        m_MusicItemTemplate.gameObject.SetActive(false);

        for (int i = 0; i < MusicManager.instance.GetTotalMusicCount(); i++)
        {
            Transform item = Instantiate(m_MusicItemTemplate, m_MusicItemTemplate.parent);

            MusicMenuItem musicItem = item.GetComponent<MusicMenuItem>();

            Song song = MusicManager.instance.GetAllSongs()[i];

            musicItem.SetMetaData(song.artist, song.title);

            item.gameObject.SetActive(true);

        }

        Song s = MusicManager.instance.GetNowPlaying();

        m_NowPlayingItem.InitItem(s.artist, s.title);
    }

    private void OnEnable() => m_NowPlayingItem.OnEnable();

    private void OnDisable() => m_NowPlayingItem.OnDisable();

    public NowPlayingMenuItem GetNowPlayingItem() => m_NowPlayingItem;

    private void Update()
    {
        if (m_NowPlayingItem != null)
            m_NowPlayingItem.Update();
    }

    public void SetSeek(Slider slider)
    {
        m_NowPlayingItem.SetSeek(slider.value);
    }
}

[System.Serializable]
public class NowPlayingMenuItem
{
    [SerializeField] TMP_Text m_Artist, m_Title, m_SeekTime, m_Duration;

    private float m_fSeekTime, m_fDuration;

    [SerializeField] Slider m_SeekSlider;

    public void InitItem(string artist, string title)
    {
        m_Artist.text = artist;
        m_Title.text = title;
        m_Duration.text = m_fDuration.GetFloatMinSecFormat();
        m_SeekSlider.maxValue = m_fDuration;
    }

    public void OnEnable() => MusicManager.instance.OnSongChanged += OnSongChanged;
    public void OnDisable() => MusicManager.instance.OnSongChanged -= OnSongChanged;

    public void SetSeek(float value) => m_fSeekTime = value;
    
    private void OnSongChanged()
    {
        Song s = MusicManager.instance.GetNowPlaying();
        m_fDuration = s.clip.length;
        InitItem(s.artist, s.title);
    }

    public void Update()
    {
        if (MusicManager.instance != null)
        {
            m_fSeekTime = MusicManager.instance.GetSeekTime();
            m_SeekSlider.value = m_fSeekTime;
            m_SeekTime.text = m_fSeekTime.GetFloatMinSecFormat();
        }
    }
}