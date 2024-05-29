using UnityEngine;

[CreateAssetMenu(fileName = "CSD_", menuName = "Game/Audio/Collision Sound Data")]
public class CollisionSoundData : ScriptableObject
{
    [SerializeField] string m_CollisionName;
    [SerializeField] AudioClip[] m_Hit;
    [SerializeField] AudioClip[] m_Scrape;

    public AudioClip GetHitSound()
    {
        if (m_Hit.Length <= 0)
        {
            Debug.LogError($"There are no hit sounds assigned to {m_CollisionName}!");
            return null;
        }

        return m_Hit[Random.Range(0, m_Hit.Length)];
    }

    public AudioClip GetScrapeSound()
    {
        if (m_Scrape.Length <= 0)
        {
            Debug.LogError($"There are no hit sounds assigned to {m_CollisionName}!");
            return null;
        }

        return m_Scrape[Random.Range(0, m_Scrape.Length)];
    }
}