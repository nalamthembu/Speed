using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private GameObject m_AudioSourcePrefab;
    [SerializeField] private int m_InitialPoolSize = 10;

    private Queue<AudioSource> m_AudioSourcePool = new Queue<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < m_InitialPoolSize; i++)
        {
            AudioSource audioSource = Instantiate(m_AudioSourcePrefab, transform).GetComponent<AudioSource>();
            audioSource.gameObject.SetActive(false);
            m_AudioSourcePool.Enqueue(audioSource);
        }
    }

    public AudioSource GetAudioSource()
    {
        if (m_AudioSourcePool.Count == 0)
        {
            AudioSource newAudioSource = Instantiate(m_AudioSourcePrefab, transform).GetComponent<AudioSource>();
            return newAudioSource;
        }

        AudioSource source = m_AudioSourcePool.Dequeue();

        source.gameObject.SetActive(true);

        return source;
    }

    private IEnumerator DelayedDeactivate(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.gameObject.SetActive(false);
        m_AudioSourcePool.Enqueue(audioSource);
    }

    public void ReturnAudioSource(AudioSource audioSource, float delay = 0f)
    {
        StartCoroutine(DelayedDeactivate(audioSource, delay));
    }
}
