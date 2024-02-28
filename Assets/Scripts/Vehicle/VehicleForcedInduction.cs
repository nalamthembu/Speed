using UnityEngine;

//Forced Induction : This is the base class for all forced induction parts (Turbos, Superchargers)
[RequireComponent(typeof(AudioSource))]
public class VehicleForcedInduction : MonoBehaviour
{
    [Tooltip("How long it takes for the forced induction to get to max boost. (Superchargers should be extremely short time but not 0, that's not possible.)")]
    [SerializeField] float m_SpoolTime;
    [SerializeField] AnimationCurve m_BoostCurve;
    [SerializeField] string m_WasteGateSoundID;
    [SerializeField] string m_ForcedInductionLoop;

    private float m_CurrentBoost;
    private float m_CurrentSpoolTime;
    [SerializeField] float m_WasteGateShutTime = 0.5f;
    float m_CurrentWasteGateShutTime = 0;

    //Public
    public float GetMaxBoost() => m_BoostCurve[m_BoostCurve.length - 1].value;
    public float GetCurrentBoost() => m_CurrentBoost;

    //Flags
    bool m_IsWasteGateOpened = false;

    //Components
    protected VehicleEngine m_AttachedVehicleEngine; //the engine you want to force air into.
    protected VehicleInput m_VehicleInput; //input from vehicle.
    protected VehicleTransmission m_AttachedTransmission; //we want to know if the clutch is engaged.
    protected AudioSource m_AudioSource; //source of spool and waste gate noise.

    protected virtual void Awake()
    {
        m_VehicleInput = GetComponent<VehicleInput>();

        m_AudioSource = GetComponent<AudioSource>();

        m_AttachedVehicleEngine = GetComponent<VehicleEngine>();

        m_AttachedTransmission = GetComponent<VehicleTransmission>();


        if (m_AudioSource == null)
            m_AudioSource = gameObject.AddComponent<AudioSource>();

        //Make sure audio source is setup correctly
        m_AudioSource.loop = false;
        m_AudioSource.spatialBlend = 1;
        m_AudioSource.minDistance = 4;
        m_AudioSource.playOnAwake = false;


        if (m_AttachedTransmission == null)
        {
            Debug.LogError("There is no transmission attached to this forced induction part.");

            enabled = false;
        }


        if (m_AttachedVehicleEngine == null)
        {
            Debug.LogError("There is no engine attached to this forced induction part.");

            enabled = false;
        }

        if (m_VehicleInput == null)
        {
            Debug.LogError("There is no engine attached to this forced induction part.");

            enabled = false;
        }

        //Validate max boost time on curve, make it match the specified spool time.
        m_BoostCurve.keys[^1].time = m_SpoolTime;
    }

    protected virtual void ControlBoost()
    {
        if (m_VehicleInput != null && m_AttachedTransmission != null)
        {
            //if there is more than a 25% throttle
            if (m_VehicleInput.Throttle >= 0.25F)
            {
                //if the tranmission is currently changing gear.
                if (m_AttachedTransmission.IsChangingGear && !m_IsWasteGateOpened)
                {
                    OpenWasteGate();
                    return;
                }

                if (m_IsWasteGateOpened)
                {
                    m_CurrentWasteGateShutTime += Time.deltaTime;

                    m_CurrentBoost -= Time.deltaTime / 2;

                    m_CurrentBoost = Mathf.Clamp(m_CurrentBoost, 0, GetMaxBoost());

                    if (m_CurrentWasteGateShutTime >= m_WasteGateShutTime)
                    {
                        //close the waste gate
                        m_IsWasteGateOpened = false;
                        m_CurrentWasteGateShutTime = 0;

                        //Swap audio
                        if (SoundManager.Instance != null)
                        {
                            SoundManager.Instance.PlaySound(m_ForcedInductionLoop, m_AudioSource, true, null, 2);
                        }
                    }

                    return;
                }

                HandleBoost();
            }
            else
            {
                //if there is suddenly not throttle

                //open waste gate
                if (!m_IsWasteGateOpened)
                    OpenWasteGate();
            }
        }
    }

    protected virtual void HandleBoost()
    {
        //'force air into the engine'
        m_CurrentSpoolTime += Time.deltaTime;
        m_CurrentSpoolTime = Mathf.Clamp(m_CurrentSpoolTime, 0, m_SpoolTime);
        m_CurrentBoost = m_BoostCurve.Evaluate(m_CurrentSpoolTime);

        if (m_AttachedVehicleEngine != null)
            m_AttachedVehicleEngine.AddPower(m_CurrentBoost);

        if (m_AudioSource != null)
        {
            m_AudioSource.pitch = 1 + (m_CurrentBoost / GetMaxBoost()) * 1.15F;

            m_AudioSource.pitch = Mathf.Clamp(m_AudioSource.pitch, 1, 1.15F);
        }
    }

    protected virtual void OpenWasteGate()
    {
        //TODO : ANTI-LAG?

        m_CurrentBoost = 0;
        m_IsWasteGateOpened = true;
        m_CurrentSpoolTime = 0;

        if (m_AttachedVehicleEngine == null)
            m_AttachedVehicleEngine.AddPower(0); //reset additional power.

        //Play Sound 
        if (SoundManager.Instance)
        {
            if (m_WasteGateSoundID == string.Empty) {
                Debug.LogError("Wastegate sound ID was left blank");
                return;
            }

            SoundManager.Instance.PlaySound(m_WasteGateSoundID, m_AudioSource, false, new RandomPitch(1, 1.25F), 5);
        }
    }

    protected virtual void Update()
    {
        ControlBoost();
    }
}