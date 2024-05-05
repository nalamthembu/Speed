using ThirdPersonFramework.UserInterface;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

enum PARTICLE_STATE
{
    ON,
    OFF
}

enum SOUND_STATE
{
    PLAY,
    STOP
}

public class WheelFX : MonoBehaviour
{
    #region VARS_SOUND
    [Header("Road Noise")]

    [SerializeField] string wheelRoadNoiseID = "WheelFX_RoadNoise";
    [SerializeField] float minRPMB4RoadNoise = 100.0F;

    private float currentMaxRoadNoisePitch = 1.25F;

    [SerializeField] GlobalSkidData m_SkidData;

    [Header("Skid Visual Settings")]
    [SerializeField] Material m_SkidMaterial;
    [SerializeField] GameObject m_SkidParticlePrefab;

    private ParticleSystem m_SkidParticleSystem;

    private float m_CurrentFrictionValue;
    private float m_SkidTime = 0;

    private WheelCollider m_WheelCollider;

    private AudioSource m_SkidSource;

    private AudioSource m_RoadNoiseSource;

    [SerializeField] float[] randomMaxPitch = { 1, 1.5f };

    #endregion

    #region VARS_VISUALS
    [SerializeField] float m_MarkWidth = 0.2f;

    int m_Skidding;
    Vector3[] m_LastPos = new Vector3[2];
    WheelHit m_WheelHit;

    #endregion

    bool m_GamePaused;

    private void Awake()
    {
        m_SkidSource = gameObject.AddComponent<AudioSource>();

        m_RoadNoiseSource = gameObject.AddComponent<AudioSource>();

        currentMaxRoadNoisePitch = randomMaxPitch[Random.Range(0, randomMaxPitch.Length)];

        m_RoadNoiseSource.maxDistance = 10.0F;

        m_RoadNoiseSource.minDistance = 2.0F;

        m_RoadNoiseSource.spread = 360.0F;

        m_RoadNoiseSource.spatialBlend = 1;


        m_SkidSource.minDistance = 10.0F;

        m_SkidSource.spread = 360.0F;

        m_SkidSource.maxDistance = 25.0F/*m*/;

        InitialiseWheelEffects();
    }

    void Start()
    {
        m_WheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        if (m_GamePaused)
            return;

        if (!m_WheelCollider.GetGroundHit(out m_WheelHit) || GameManager.Instance.IsInMenu)
        {
            if (m_SkidSource.isPlaying)
                StopAllSounds();

            SkidParticles(PARTICLE_STATE.OFF);

            return;
        }

        if (IsSkidding())
        {
            if (m_SkidTime < 1)
                m_SkidTime += Time.deltaTime;

            DrawSkidMesh();

            if (m_CurrentFrictionValue >= 0.25F || m_SkidTime >= 1.0f)
                SkidParticles(PARTICLE_STATE.ON);
        }
        else
        {
            m_Skidding = 0;

            m_SkidTime = 0;

            SkidParticles(PARTICLE_STATE.OFF);
        }

        SkidSound();

        HandleWheelNoise();
    }

    void HandleWheelNoise()
    {
        if (IsSkidding() || !m_WheelCollider.isGrounded)
        {
            if (m_RoadNoiseSource.isPlaying)
                m_RoadNoiseSource.Stop();

            currentMaxRoadNoisePitch = randomMaxPitch[Random.Range(0, randomMaxPitch.Length)];

            return;
        }
        else
        {
            if (!m_RoadNoiseSource.isPlaying)
                SoundManager.Instance.PlaySound(wheelRoadNoiseID, m_RoadNoiseSource, true);

            m_RoadNoiseSource.pitch = Mathf.Lerp(1, currentMaxRoadNoisePitch, (Mathf.Abs(m_WheelCollider.rpm) / minRPMB4RoadNoise));
            m_RoadNoiseSource.volume = Mathf.SmoothStep(0, 0.35F, Mathf.Floor(Mathf.Abs(m_WheelCollider.rpm)) / minRPMB4RoadNoise);
        }
    }

    void SkidSound()
    {
        if (m_SkidData != null && m_SkidData.GlobalSkidSettings != null)
        {
            if (m_CurrentFrictionValue >= m_SkidData.GlobalSkidSettings.MinSkidThreshold)
            {
                //Is Skidding
                if (!m_SkidSource.isPlaying)
                {
                    if (SoundManager.Instance)
                        SoundManager.Instance.PlaySound(m_SkidData.GlobalSkidSettings.wheelSkidID, m_SkidSource, true);
                }
                else
                {
                    //If the source is playing...
                    float randomMaxPitch = Random.Range(m_SkidData.GlobalSkidSettings.MaxSkidVolume - 0.25f, m_SkidData.GlobalSkidSettings.MaxSkidVolume + 0.25F);
                    m_SkidSource.pitch = Mathf.Lerp(1, randomMaxPitch, m_CurrentFrictionValue / m_SkidData.GlobalSkidSettings.MaxSkidThreshold);
                    if (m_CurrentFrictionValue < m_SkidData.GlobalSkidSettings.OscillationThreshold)
                    {
                        float targetPitch = Mathf.Lerp(1, m_SkidData.GlobalSkidSettings.MaxSkidPitch, m_CurrentFrictionValue / m_SkidData.GlobalSkidSettings.MaxSkidThreshold);
                        float targetVolume = Mathf.Lerp(m_SkidData.GlobalSkidSettings.MinSkidVolume, m_SkidData.GlobalSkidSettings.MinSkidVolume, m_CurrentFrictionValue / m_SkidData.GlobalSkidSettings.MaxSkidThreshold);
                        m_SkidSource.pitch = Mathf.SmoothDamp(m_SkidSource.pitch, targetPitch, ref m_SkidData.GlobalSkidSettings.SinPitchVelocity, 1);
                        m_SkidSource.volume = Mathf.SmoothDamp(m_SkidSource.volume, targetVolume, ref m_SkidData.GlobalSkidSettings.SinVolumeVelocity, 0.25F);
                    }
                    else
                    {
                        float targetOscPitch = 1 + Mathf.Sin(Time.time * m_SkidData.GlobalSkidSettings.PitchFrequency) * m_SkidData.GlobalSkidSettings.PitchAmplitude;
                        float targetOscVol = m_SkidData.GlobalSkidSettings.MinSkidVolume + Mathf.Sin(Time.time * m_SkidData.GlobalSkidSettings.VolumeFrequency) * m_SkidData.GlobalSkidSettings.VolumeAmplitude;
                        m_SkidSource.pitch = Mathf.SmoothDamp(m_SkidSource.pitch, targetOscPitch, ref m_SkidData.GlobalSkidSettings.SinPitchVelocity, m_SkidData.GlobalSkidSettings.SmoothTimePitchOsc);
                        m_SkidSource.volume = Mathf.SmoothDamp(m_SkidSource.volume, targetOscVol, ref m_SkidData.GlobalSkidSettings.SinVolumeVelocity, m_SkidData.GlobalSkidSettings.SmoothTimeVolumeOsc);
                    }
                }
            }
            else
            {
                m_SkidSource.Stop();
            }
        }
    }

    bool IsSkidding()
    {
        m_CurrentFrictionValue = Mathf.Abs((m_WheelHit.forwardSlip + m_WheelHit.sidewaysSlip) / 2);

        return m_CurrentFrictionValue > m_SkidData.GlobalSkidSettings.MinSkidThreshold;
    }

    void StopAllSounds()
    {
        m_SkidSource.Stop();
        m_RoadNoiseSource.Stop();
    }

    void SkidParticles(PARTICLE_STATE particleState)
    {
        switch (particleState)
        {
            case PARTICLE_STATE.ON:
                m_SkidParticleSystem.Play();
                return;

            case PARTICLE_STATE.OFF:
                m_SkidParticleSystem.Stop();
                return;
        }
    }

    void InitialiseWheelEffects()
    {
        GameObject particleGameObject = Instantiate(m_SkidParticlePrefab, transform);

        particleGameObject.transform.forward = -transform.forward;

        m_SkidParticleSystem = particleGameObject.GetComponent<ParticleSystem>();
    }

    void DrawSkidMesh()
    {
        m_WheelCollider.GetGroundHit(out m_WheelHit);

        GameObject mark = new("Mark");
        MeshFilter filter = mark.AddComponent<MeshFilter>();
        MeshRenderer renderer = mark.AddComponent<MeshRenderer>();

        renderer.shadowCastingMode = ShadowCastingMode.Off;

        Mesh markMesh = new();
        var vertices = new Vector3[4];
        int[] triangles;

        float heightAboveGround = 0.06f;

        Quaternion rotation = Quaternion.Euler(transform.eulerAngles);

        if (m_Skidding == 0)
        {
            vertices[0] = m_WheelHit.point + rotation * new Vector3(m_MarkWidth, heightAboveGround, 0);
            vertices[1] = m_WheelHit.point + rotation * new Vector3(-m_MarkWidth, heightAboveGround, 0);
            vertices[2] = m_WheelHit.point + rotation * new Vector3(-m_MarkWidth, heightAboveGround, 0);
            vertices[3] = m_WheelHit.point + rotation * new Vector3(m_MarkWidth, heightAboveGround, 0);
            m_LastPos[0] = vertices[2];
            m_LastPos[1] = vertices[3];
            m_Skidding = 1;
        }
        else
        {
            vertices[1] = m_LastPos[0];
            vertices[0] = m_LastPos[1];
            vertices[2] = m_WheelHit.point + rotation * new Vector3(-m_MarkWidth, heightAboveGround, 0);
            vertices[3] = m_WheelHit.point + rotation * new Vector3(m_MarkWidth, heightAboveGround, 0);
            m_LastPos[0] = vertices[2];
            m_LastPos[1] = vertices[3];
        }

        triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        markMesh.vertices = vertices;
        markMesh.triangles = triangles;
        markMesh.RecalculateNormals();
        Vector2[] uvm = new Vector2[4];

        uvm[0] = new Vector2(1, 0);
        uvm[1] = new Vector2(0, 0);
        uvm[2] = new Vector2(0, 1);
        uvm[3] = new Vector2(1, 1);

        markMesh.uv = uvm;
        filter.mesh = markMesh;
        renderer.material = m_SkidMaterial;
        mark.transform.SetParent(GameManager.Instance.SKIDMARK_PARENTOBJ.transform);
        Destroy(mark, GameManager.SKIDMARK_LIFETIME);
    }

    private void OnDisable()
    {
        PauseMenu.OnPauseMenuOpened -= OnGamePaused;
        PauseMenu.OnPauseMenuClosed -= OnGameResumed;
    }

    private void OnEnable()
    {
        PauseMenu.OnPauseMenuOpened += OnGamePaused;
        PauseMenu.OnPauseMenuClosed += OnGameResumed;
    }

    void OnGamePaused()
    {
        m_GamePaused = true;
        StopAllSounds();
        m_SkidParticleSystem.Pause();
    }
    void OnGameResumed() => m_GamePaused = false;
}
