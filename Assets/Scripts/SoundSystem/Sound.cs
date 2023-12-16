using UnityEngine;

[System.Serializable]
public struct Sound
{
    public string name;

    public AudioClip[] sounds;

    public SoundType soundType;

    public AudioClip GetRandomClip()
    {
        if (sounds.Length == 1)
            return sounds[0];
        else
        {
            return sounds[Random.Range(0, sounds.Length)];
        }
    }
}

public enum SoundType
{
    SFX,
    Music,
    Frontend,
    VehicleSounds
}