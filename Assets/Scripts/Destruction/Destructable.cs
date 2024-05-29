using UnityEngine;
using ThirdPersonFramework;

public class Destructable : Entity
{
    [SerializeField] float m_ImpulseThreshold = 1;

    [SerializeField] CollisionSoundData m_CollisionSoundData;

    private AudioSource m_Source;

    protected override void Awake()
    {
        base.Awake();

        InitAudio();
    }

    void InitAudio()
    {
        m_Source = gameObject.AddComponent<AudioSource>();

        m_Source.loop = false;

        m_Source.volume = 1.0f;

        m_Source.minDistance = 10;

        m_Source.pitch = 1.0f;

        m_Source.playOnAwake = false;

        m_Source.spatialBlend = 1;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.impulse.magnitude >= m_ImpulseThreshold)
        {
            // Play Collision Sound

            if (m_Source != null)
            {
                m_Source.clip = m_CollisionSoundData.GetHitSound();
                m_Source.pitch = new RandomPitch(1, 1.25f).GetRandomPitch();

                if (m_Source.outputAudioMixerGroup != SoundManager.Instance.GetMixer(SoundType.SFX).group)
                {
                    m_Source.outputAudioMixerGroup = SoundManager.Instance.GetMixer(SoundType.SFX).group;
                }

                m_Source.Play();
            }
        }
    }
}
