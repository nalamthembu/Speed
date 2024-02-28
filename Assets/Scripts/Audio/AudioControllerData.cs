using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName ="AudioControllerData", menuName = "Game/Sound/Audio Controller")]
public class AudioControllerData : ScriptableObject
{
    public AudioState[] audioStates;

    public bool TryGetAudioState(string audioStateName, out AudioState outState)
    {
        outState = null;

        for (int i = 0; i < audioStates.Length; i++)
        {
            if (audioStates[i].name == audioStateName)
            {
                outState = audioStates[i];
                return true;
            }
        }

        Debug.LogWarning("Could not find specified audio state : " + audioStateName);

        return false;
    }
}

[System.Serializable]
public class AudioState
{
    public string name;
    public AudioMixerSnapshot snapshot;
}