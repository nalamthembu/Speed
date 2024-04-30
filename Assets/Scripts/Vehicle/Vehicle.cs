using System;
using System.Collections;
using UnityEngine;
using VehiclePhysics;
using Object = UnityEngine.Object;
using ThirdPersonFramework;

public class Vehicle : Entity
{
    [SerializeField] Seat[] m_Seats;

    [SerializeField] WheelVisualComponent[] m_WheelVisualComponents;

    [SerializeField] Transform m_CameraFocus;

    [SerializeField] float m_StartTime = 0.5F;

    [SerializeField] float m_MaxDrag = 1;

    public VPVehicleController Controller { get; private set; }
    public VPStandardInput VPStandardInput { get; private set; }
    public float SpeedKMH { get; private set; }
    public float EngineRPM { get; private set; }
    public float PeakEngineRPM { get; private set; }
    public float CurrentGear { get; private set; }
    public Transform CameraFocus { get { return m_CameraFocus; } }
    public WheelVisualComponent[] wheelVisualComponents { get { return m_WheelVisualComponents; } }

    private VehicleBase m_VehicleBase;

    private Rigidbody m_RigidBody;

    public static event Action<Vehicle> OnVehicleStarted;

    private Vector3 m_VelocityBeforePausing;

    protected override void Awake()
    {
        Controller = GetComponent<VPVehicleController>();
        VPStandardInput = GetComponent<VPStandardInput>();
        m_RigidBody = GetComponent<Rigidbody>();
        if (Controller != null)
            m_VehicleBase = Controller;

        PeakEngineRPM = Controller.engine.peakRpm;
    }

    protected override void FixedUpdate()
    {
        if (m_RigidBody != null)
        {
            SpeedKMH = m_RigidBody.velocity.magnitude * 3.6F;
            m_RigidBody.drag = Mathf.Lerp(m_MaxDrag, 0, Mathf.Clamp(SpeedKMH / 15, 0, 1));
        }

        if (m_VehicleBase != null)
        {
            int[] vehicleData = m_VehicleBase.data.Get(Channel.Vehicle);
            CurrentGear = vehicleData[VehicleData.GearboxGear];
            EngineRPM = Controller.EnergyProvider.sensorRpm;
        }
    }

    public void SetSimulatePhysics(bool simulatePhysics) => m_RigidBody.isKinematic = !simulatePhysics;

    //TODO : Handle Characters in Vehicle

    public void StartVehicle() => StartCoroutine(StartVehicleEnumerator());

    private IEnumerator StartVehicleEnumerator()
    {
        float timer = 0;

        while (timer < m_StartTime)
        {
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        //Start the car

        Controller.enabled = true;
        VPStandardInput.enabled = transform.parent != null && transform.parent.GetComponent<Player>();

        OnVehicleStarted?.Invoke(this);
    }

    protected override void OnGamePaused()
    {
        base.OnGamePaused();

        Debug.Log("Game Pause Event!");

        m_VelocityBeforePausing = m_RigidBody.velocity;

        SetSimulatePhysics(false);
    }

    protected override void OnGameResume()
    {
        base.OnGameResume();

        Debug.Log("Game Resume Event!");

        SetSimulatePhysics(true);

        m_RigidBody.velocity = m_VelocityBeforePausing;
    }
}

[System.Serializable]
public struct Seat
{
    [SerializeField] SeatType m_SeatType;

    [SerializeField] GameObject m_CharacterPrefab; //The character we spawn into the seat.

    [SerializeField] Transform m_SeatTransform;

    public readonly SeatType GetSeatType() => m_SeatType;

    public readonly void InitialiseSeat(GameObject characterPrefab = null)
    {
        if (characterPrefab == null)
            Object.Instantiate(m_CharacterPrefab, m_SeatTransform.position, m_SeatTransform.rotation, m_SeatTransform);
        else
            Object.Instantiate(characterPrefab, m_SeatTransform.position, m_SeatTransform.rotation, m_SeatTransform);
    }
}

public enum SeatType
{
    RearLeft,
    RearRight,
    Passenger,
    Driver
}