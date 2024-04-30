using System.Collections;
using UnityEngine;
using VehiclePhysics;

public class CarAudio : MonoBehaviour
{
    private RealisticEngineSound m_EngineSoundComponent;

    private Vehicle m_AttachedVehicle;

    private VPStandardInput m_VPStandardInput;

    [SerializeField] bool simulated = true;

    [SerializeField] float m_StartPeakRPM = 3000.0F;

    [SerializeField] float m_StartPeakReachTime = 0.25F;

    private bool m_HasCompletedStart;

    private void Awake()
    {
        m_EngineSoundComponent = GetComponentInChildren<RealisticEngineSound>();
        m_AttachedVehicle = GetComponent<Vehicle>();
        m_VPStandardInput = GetComponent<VPStandardInput>();

        if (m_AttachedVehicle != null && m_EngineSoundComponent != null)
        {
            m_EngineSoundComponent.maxRPMLimit = m_AttachedVehicle.PeakEngineRPM - 250;
            m_EngineSoundComponent.mainCamera = Camera.main;
        }
    }

    public void SetSimulated(bool simulated) => this.simulated = simulated;

    private void OnEnable() => Vehicle.OnVehicleStarted += OnVehicleStarted;

    [ContextMenu("Test Start")]
    private void TestStart() => StartCoroutine(DoStartSound());

    private void OnVehicleStarted(Vehicle vehicle)
    {
        if (m_AttachedVehicle != null && vehicle == m_AttachedVehicle)
            StartCoroutine(DoStartSound());
    }

    private IEnumerator DoStartSound()
    {
        m_HasCompletedStart = false;

        float currentRpm = 0;

        float refFloat = 0;

        if (currentRpm < m_StartPeakRPM)
        {
            currentRpm = Mathf.SmoothDamp(currentRpm, m_StartPeakRPM, ref refFloat, m_StartPeakReachTime);

            if (m_EngineSoundComponent != null)
            {
                m_EngineSoundComponent.engineCurrentRPM = currentRpm;
            }

            yield return new WaitForEndOfFrame();
        }

        m_HasCompletedStart = true;
    }

    private void Update()
    {
        m_EngineSoundComponent.transform.gameObject.SetActive(simulated);

        if (simulated)
        {
            m_EngineSoundComponent.engineCurrentRPM = m_AttachedVehicle.EngineRPM;
            m_EngineSoundComponent.gasPedalPressing = m_VPStandardInput.externalThrottle > 0;
            //res.isReversing = transmission.IsInReverse;
        }
    }
}