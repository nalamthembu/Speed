using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Game/Sound/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    public Sound[] sounds;

    public Mixer[] mixers;

    public Sound GetSound(string soundName)
    {
        foreach(var sound in sounds)
            if (sound.name == soundName) return sound;

        Debug.LogError($"Could not find specified sound ({soundName})");

        return null;
    }

    public Mixer GetMixer(SoundType type)
    {
        foreach(Mixer mixer in mixers)
        {
            if (mixer.mixerType == type)
                return mixer;
        }

        Debug.LogError($"Could not find specified mixer ({type})");

        return null;
    }
}


[System.Serializable]
public class Mixer
{
    public string name;

    public AudioMixerGroup group;

    public SoundType mixerType;
}