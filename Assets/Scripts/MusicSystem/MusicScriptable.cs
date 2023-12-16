using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicLibrary", menuName = "Game/Music/Music Library")]
public class MusicScriptable : ScriptableObject
{
    public Song[] music;

    public AudioMixerGroup musicMixerGroup;
}

[System.Serializable]
public struct Song
{
    public string title;
    public string artist;
    public AudioClip clip;
    public float startTimeDuringRace;
    public MUSIC_TYPE type;
}

public enum MUSIC_TYPE
{
    MUSIC_GARAGE,
    MUSIC_BOTH,
    MUSIC_RACING
}