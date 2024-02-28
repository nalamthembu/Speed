using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSkidData", menuName = "Game/Vehicle/Global Skid Data")]
public class GlobalSkidData : ScriptableObject
{
    public SkidSettings GlobalSkidSettings;
}

[System.Serializable]
public class SkidSettings
{
    [Header("Skidding")]

    public string wheelSkidID = "WheelFX_Skid"; //Sound ID
    [Range(0.001f, 3)] public float MinSkidThreshold = .2f;
    [Range(0.001f, 3)] public float MaxSkidThreshold = 1;
    [Range(0.001f, 3)] public float OscillationThreshold = 0.45f;

    [Header("Sin Wave - Volume")]
    [SerializeField] public float VolumeAmplitude = 0.25F;
    [SerializeField] public float VolumeFrequency = 65F;
    [SerializeField] public float SmoothTimeVolumeOsc = 0.01F;
    [HideInInspector] public float SinVolumeVelocity;

    [Header("Sin Wave - Pitch")]
    [SerializeField] public float PitchAmplitude = 0.15F;
    [SerializeField] public float PitchFrequency = 1;
    [SerializeField] public float SmoothTimePitchOsc = 0.1F;
    [HideInInspector] public float SinPitchVelocity;

    [Header("Skid Audio Settings")]
    public float MinSkidVolume = 0.5F;
    public float MaxSkidVolume = 1;
    public float MaxSkidPitch = 1.5F;
}