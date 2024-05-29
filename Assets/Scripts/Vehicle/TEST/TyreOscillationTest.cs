using UnityEngine;

public class TyreOscillationTest : MonoBehaviour
{
    [Header("Test Params")]
    [SerializeField][Range(0, 10)] float m_CurrentSkid;
    [SerializeField] float m_SkidThreshold;
    [SerializeField][Min(0.1F)] float m_MaxSkidThreshold = 2;
    [SerializeField][Min(0.1F)] float m_OscillationThreshold = 3;
    [SerializeField] AudioClip[] m_SkidSounds;
    [SerializeField] float m_Load; //Slow Car means Higher Load.

    [Header("Sin Wave - Volume")]
    [SerializeField] float m_VolumeAmplitude;
    [SerializeField] float m_VolumeFrequency;
    [SerializeField] float m_SmoothTimeVolumeOsc = 0.05F;
    float m_SinVolumeVelocity;

    [Header("Sin Wave - Pitch")]
    [SerializeField] float m_PitchAmplitude;
    [SerializeField] float m_PitchFrequency;
    [SerializeField] float m_SmoothTimePitchOsc = 0.05F;
    float m_SinPitchVelocity;

    [Header("Audio Settings")]
    [SerializeField] float m_MinVolume = 0;
    [SerializeField] float m_MaxVolume = 1;
    [SerializeField] float m_MaxPitch = 2;

    AudioSource m_Source;

    private void Awake()
    {
        m_Source = gameObject.AddComponent<AudioSource>();
        m_Source.loop = true;
        m_Source.playOnAwake = false;
        if (SoundManager.Instance != null)
            m_Source.outputAudioMixerGroup = SoundManager.Instance.GetMixer(SoundType.SFX).group;
        else
            Debug.LogError("There is no sound manager in the scene!");
        m_Source.volume = 1.0f;
        m_Source.spatialBlend = 1.0f;
    }

    private void Update()
    {
        if (m_CurrentSkid >= m_SkidThreshold)
        {
            //Is Skidding

            if (!m_Source.isPlaying)
            {
                m_Source.clip = m_SkidSounds[Random.Range(0, m_SkidSounds.Length)];
                m_Source.Play();
            }
            else
            {
                //If the source is playing...
                if (m_CurrentSkid < m_OscillationThreshold)
                {
                    float targetPitch = Mathf.Lerp(1, m_MaxPitch, m_CurrentSkid / m_MaxSkidThreshold);
                    float targetVolume = Mathf.Lerp(m_MinVolume, m_MaxVolume, m_CurrentSkid / m_MaxSkidThreshold);
                    m_Source.pitch = Mathf.SmoothDamp(m_Source.pitch, targetPitch, ref m_SinPitchVelocity, 1);
                    m_Source.volume = Mathf.SmoothDamp(m_Source.volume, targetVolume, ref m_SinVolumeVelocity, 0.25F);
                }
                else if (m_CurrentSkid >= m_OscillationThreshold && m_Load >= 0.65F)
                {
                    float targetOscPitch = 1 + Mathf.Sin(Time.time * m_PitchFrequency) * m_PitchAmplitude;
                    float targetOscVol = m_MinVolume + Mathf.Sin(Time.time * m_VolumeFrequency) * m_VolumeAmplitude;
                    m_Source.pitch = Mathf.SmoothDamp(m_Source.pitch, targetOscPitch, ref m_SinPitchVelocity, m_SmoothTimePitchOsc);
                    m_Source.volume = Mathf.SmoothDamp(m_Source.volume, targetOscVol * m_Load, ref m_SinVolumeVelocity, m_SmoothTimeVolumeOsc);
                }
            }
        }
        else
        {
            m_Source.Stop();
        }
    }
}