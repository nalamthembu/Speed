using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, Sound> soundDictionary;

    public SoundLibrary library;

    public static SoundManager Instance;

    private AudioSource m_FrontendSource;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitialiseSoundManager();
    }

    private void InitialiseSoundManager()
    {
        soundDictionary = new();

        foreach (Sound s in library.sounds)
        {
            soundDictionary.Add(s.name, s);
        }

        m_FrontendSource = gameObject.AddComponent<AudioSource>();
        m_FrontendSource.loop = false;
        m_FrontendSource.spatialBlend = 0;
        m_FrontendSource.outputAudioMixerGroup = library.GetMixer(SoundType.Frontend).group;
        m_FrontendSource.playOnAwake = false;
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            m_FrontendSource.clip = sound.GetRandomClip();

            if (MixerExists(sound.soundType, out Mixer mixerFE))
                m_FrontendSource.outputAudioMixerGroup = mixerFE.group;
        }
        m_FrontendSource.Play();
    }

    public void PlaySound(string soundName, AudioSource source, bool loop = false, RandomPitch? randomPitch = default, float minDist = 5)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            switch (sound.soundType)
            {
                case SoundType.Frontend:

                    source.clip = sound.GetRandomClip();

                    if (MixerExists(sound.soundType, out Mixer mixerFE))
                    {
                        source.outputAudioMixerGroup = mixerFE.group;
                    }

                    source.spatialBlend = 0;

                    source.loop = loop;

                    break;

                case SoundType.SFX:

                    source.clip = sound.GetRandomClip();

                    if (MixerExists(sound.soundType, out Mixer mixerSFX))
                    {
                        source.outputAudioMixerGroup = mixerSFX.group;
                    }

                    source.spatialBlend = 1;

                    source.loop = loop;

                    source.minDistance = minDist;

                    source.pitch = randomPitch != null ? randomPitch.Value.GetRandomPitch() : 1;

                    break;
            }

            source.Play();
        }
        else
        {
            Debug.LogError("Could not find sound of ID : " + soundName);
        }
    }

    public void PlaySound(string soundName, Vector3 position, bool loop = false, RandomPitch? randomPitch = null, float minDist = 50.0F)
    {

        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            switch (sound.soundType)
            {
                case SoundType.Frontend:

                    Debug.LogWarning("You tried to play a frontend sound in 3-D Space");

                    break;

                case SoundType.SFX:

                    AudioSource source = PoolManager.Instance.GetAudioSource();

                    source.transform.position = position;

                    source.clip = sound.GetRandomClip();

                    if (MixerExists(sound.soundType, out Mixer mixer))
                    {
                        source.outputAudioMixerGroup = mixer.group;
                    }

                    source.minDistance = minDist;

                    source.spatialBlend = 1;

                    source.pitch = randomPitch.HasValue ? randomPitch.Value.GetRandomPitch() : 1;

                    source.dopplerLevel = 0; // no doppler...that sounds weird.

                    source.loop = loop;

                    source.Play();

                    PoolManager.Instance.ReturnAudioSource(source, source.clip.length + 1);

                    break;
            }
        }
    }

    public Sound GetSound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            return sound;
        }

        Debug.LogError("Could not find sound " + soundName);

        return null;
    }

    public Mixer GetMixer(SoundType mixerType)
    {
        /*Does*/
        MixerExists(mixerType, out Mixer mixerToReturn);//?

        return mixerToReturn;
    }

    private bool MixerExists(SoundType mixerType, out Mixer mixer)
    {
        for (int i = 0; i < library.mixers.Length; i++)
        {
            if (library.mixers[i].mixerType == mixerType)
            {
                mixer = library.mixers[i];

                return true;
            }
        }

        Debug.LogError("Could not find mixer of type : " + mixerType);

        mixer = new Mixer();

        return false;
    }
}

[System.Serializable]
public struct RandomPitch
{
    public float min;
    public float max;

    public RandomPitch(float min = 1, float max = 1)
    {
        this.min = min;
        this.max = max;
    }

    public float GetRandomPitch() => Random.Range(min, max);
}
