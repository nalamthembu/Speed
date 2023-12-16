using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Game/Sound/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    public Sound[] sounds;

    public Mixer[] mixers;
}


[System.Serializable]
public struct Mixer
{
    public string name;

    public AudioMixerGroup group;

    public SoundType mixerType;
}