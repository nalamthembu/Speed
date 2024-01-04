using TMPro;
using UnityEngine;

public class MusicMenuItem : MonoBehaviour
{
    [SerializeField] TMP_Text m_Artist, m_Title;

    public void SetMetaData(string artist, string title)
    {
        m_Artist.text = artist;
        m_Title.text = title;
    }

    public void OnClick()
    {
        MusicManager.instance.PlaySongByMetadata(m_Title.text, m_Artist.text);
        
    }
}
