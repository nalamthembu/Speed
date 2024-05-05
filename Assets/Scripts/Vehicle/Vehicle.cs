using System;
using System.Collections.Generic;
using ThirdPersonFramework;
using UnityEngine;
using Object = UnityEngine.Object;

[
    RequireComponent
    (
        typeof(VehicleInput),
        typeof(VehicleEngine),
        typeof(VehicleTransmission)
    )
]

[
    RequireComponent
    (
        typeof(Rigidbody),
        typeof(VehicleVisuals)
    )
]

public class Vehicle : Entity
{
    //TO-DO : ABS ON WHEELS.

    public Axis[] axes;

    [SerializeField] Seat[] seats;

    private Rigidbody rigidBody;

    private VehicleInput input;

    private VehicleEngine engine;

    private VehicleTransmission transmission;

    private Axis steeringAxis;
    readonly private List<Axis> poweredAxis = new();
    readonly private List<Wheel> poweredWheels = new();
    readonly private List<Wheel> allWheels = new();
    readonly private List<Wheel> steeringWheels = new();

    public Axis SteeringAxis { get { return steeringAxis; } }
    public List<Axis> PoweredAxis { get { return poweredAxis; } }
    public List<Wheel> AllWheels { get { return allWheels; } }
    public List<Wheel> SteeringWheels { get { return steeringWheels; } }
    public bool ABSActive { get; private set; }

    public bool IsAIRacer { get; private set; }

    public float SpeedKMH { get { return rigidBody.velocity.magnitude * 3.5F; } }
    public VehicleEngine Engine { get { return engine; } }
    public VehicleTransmission Transmission { get { return transmission; } }

    Vector3 m_VelocityBeforePause;
    Vector3 m_AngularVelocityBeforePause;

    public bool IsControlledByPlayer()
    {
        if (!TryGetComponent<PlayerVehicleInput>(out var input))
            return false;

        return input.playerControlEnabled;
    }

    //ORIENTATION RESET
    private const float TIME_BEFORE_AUTORESET = 3;
    private float currentResetTimer = 0;

    public void SetSimulatePhysics(bool value) => rigidBody.isKinematic = !value; //this is flipped because IsKinematic false means that it isKinematic (respects physics).

    protected override void Awake()
    {
        base.Awake();

        input = GetComponent<VehicleInput>();

        engine = GetComponent<VehicleEngine>();

        transmission = GetComponent<VehicleTransmission>();

        rigidBody = GetComponent<Rigidbody>();

        rigidBody.mass = Mathf.Round(rigidBody.mass < 800 ? 800 : rigidBody.mass);

        for (int i = 0; i < axes.Length; i++)
        {
            if (axes[i].isPowered)
            {
                poweredAxis.Add(axes[i]);
            }

            for (int j = 0; j < axes[i].wheels.Length; j++)
            {
                allWheels.Add(axes[i].wheels[j]);

                if (axes[i].isSteering)
                {
                    if (!steeringAxis.Equals(axes[i]))
                        steeringAxis = axes[i];

                    steeringWheels.Add(axes[i].wheels[j]);
                }

                if (axes[i].isPowered)
                {
                    poweredWheels.Add(axes[i].wheels[j]);
                }
            }
        }

        SetMaxSteerAngle();
    }


    public void InitialiseSeatCharacter(GameObject characterPrefab, SeatType seatType)
    {
        for (int i = 0; i < seats.Length; i++)
        {
            if (seatType == seats[i].GetSeatType())
                seats[i].InitialiseSeat(characterPrefab);
        }
    }

    private void SetMaxSteerAngle()
    {
        for (int i = 0; i < axes.Length; i++)
        {
            if (!axes[i].isSteering)
                continue;

            for (int j = 0; j < axes[i].wheels.Length; j++)
            {

                axes[i].wheels[0].SetMaxSteeringAngle(Mathf.Abs(
                    Mathf.Rad2Deg *
                    Mathf.Atan(2.55F / (axes[i].steerRadius + (1.5f / 2)))
                    * 1));

                axes[i].wheels[1].SetMaxSteeringAngle(Mathf.Abs(
                    Mathf.Rad2Deg *
                    Mathf.Atan(2.55F / (axes[i].steerRadius - (1.5f / 2)))
                    * 1));
            }
        }
    }

    protected override void FixedUpdate()
    {
        ControlVariableDrag();
        ControlVariableWheelStiffness();
        ControlSteering();
        ControlABS();

        if (IsFlippedOver())
        {
            currentResetTimer += Time.deltaTime;

            if (currentResetTimer >= TIME_BEFORE_AUTORESET)
            {
                transform.localPosition += Vector3.up;

                transform.localEulerAngles *= 0;

                currentResetTimer = 0;
            }
        }
    }

    protected override void OnGamePaused()
    {
        m_VelocityBeforePause = rigidBody.velocity;
        m_AngularVelocityBeforePause = rigidBody.angularVelocity;
        SetSimulatePhysics(false);
    }

    protected override void OnGameResume()
    {
        SetSimulatePhysics(true);
        rigidBody.velocity = m_VelocityBeforePause;
        rigidBody.angularVelocity = m_AngularVelocityBeforePause;
    }
    
    public bool IsFlippedOver() => Vector3.Dot(transform.up, Vector3.up) <= -0.50F;

    private void ControlVariableDrag()
    {
        rigidBody.drag = (Mathf.Floor(input.Throttle) == 0 || transmission.IsChangingGear) ?
            Mathf.Lerp(rigidBody.drag, 0.2F, Time.deltaTime) :
            Mathf.Lerp(rigidBody.drag, 0.005F, Time.deltaTime);
    }

    private void ControlVariableWheelStiffness()
    {
        float speedT = SpeedKMH / 170.0F;

        float sF_Powered, fF_Powered;

        for (int i = 0; i < poweredWheels.Count; i++)
        {
            sF_Powered = Mathf.Lerp(0.05F, 3.0F, speedT);
            fF_Powered = Mathf.Lerp(0.1f, 3.0F, speedT);

            if (IsAIRacer)
            {
                sF_Powered = Mathf.Lerp(0.05f, 3.0f, speedT + 0.25F);
                fF_Powered = Mathf.Lerp(0.1f, 3.0f, speedT + 0.25F);
            }

            poweredWheels[i].SetWheelStiffness(sF_Powered, fF_Powered);
        }
    }

    public float GetPoweredWheelSlip()
    {
        float totalSlip = 0;

        for (int i = 0; i < poweredAxis.Count; i++)
        {
            foreach (Wheel wheel in poweredAxis[i].wheels)
            {
                totalSlip += (wheel.WheelSlip.forward + wheel.WheelSlip.sideways) / poweredAxis[i].wheels.Length;
            }
        }

        totalSlip /= poweredAxis.Count;

        return totalSlip;
    }

    private void ControlABS()
    {
        float maxWheelSlippage = 0.95F;

        for (int i = 0; i < allWheels.Count; i++)
        {
            ABSActive = Mathf.Abs(allWheels[i].WheelSlip.forward) > maxWheelSlippage && input.Brake > 0;

            if (ABSActive)
            {
                allWheels[i].SetBrakeTorque(0);
            }
        }
    }

    private void ControlSteering()
    {
        for (int i = 0; i < axes.Length; i++)
        {
            if (!axes[i].isSteering)
                continue;

            for (int j = 0; j < axes[i].wheels.Length; j++)
            {
                if (input.Steering > 0)
                {
                    axes[i].wheels[0].SetSteerAngle(
                        Mathf.Rad2Deg *
                        Mathf.Atan(2.55F / (axes[i].steerRadius + (1.5f / 2)))
                        * input.Steering);

                    axes[i].wheels[1].SetSteerAngle(
                        Mathf.Rad2Deg *
                        Mathf.Atan(2.55F / (axes[i].steerRadius - (1.5f / 2)))
                        * input.Steering);
                }
                else if (input.Steering < 0)
                {
                    axes[i].wheels[0].SetSteerAngle(
                        Mathf.Rad2Deg *
                        Mathf.Atan(2.55F / (axes[i].steerRadius - (1.5f / 2)))
                        * input.Steering);

                    axes[i].wheels[1].SetSteerAngle(
                        Mathf.Rad2Deg *
                        Mathf.Atan(2.55F / (axes[i].steerRadius + (1.5f / 2)))
                        * input.Steering);
                }
            }
        }
    }

    public bool IsGrounded()
    {
        foreach (Axis axis in poweredAxis)
        {
            foreach (Wheel w in axis.wheels)
            {
                //IF ANY OF THE WHEELS ARE NOT TOUCHING THE GROUND
                if (!w.IsGrounded)
                    return false;
            }
        }

        //THIS ASSUMES ALL THE WHEELS ARE TOUCHING THE GROUND.
        return true;
    }
}

[System.Serializable]
public struct Axis
{
    public Wheel[] wheels;
    public bool isPowered;
    public bool isSteering;
    public float steerRadius;

    public override readonly bool Equals(object obj)
    {
        return obj is Axis axis &&
               EqualityComparer<Wheel[]>.Default.Equals(wheels, axis.wheels) &&
               isPowered == axis.isPowered &&
               isSteering == axis.isSteering &&
               steerRadius == axis.steerRadius;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(wheels, isPowered, isSteering, steerRadius);
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