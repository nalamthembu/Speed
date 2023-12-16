using System;
using UnityEngine;
using RaceSystem;

public class AIDriver : Racer
{
    public Driver driver;
    public VehicleBodyKitManager kitManager;
    private VehicleInput input;
    [SerializeField] float rayLength = 1.5F;

    public float targetThrottle { get; private set; }
    public float targetBrake { get; private set; }
    public float targetHandbrake { get; private set; }
    public Vector3 targetPosition { get; private set; }
    public float remainingDistance { get; private set; }

    private float targetSpeed = 100;

    private Transform targetTransform;

    float steerDirection;

    protected override void Awake()
    {
        base.Awake();

        kitManager = GetComponent<VehicleBodyKitManager>();
        input = GetComponent<VehicleInput>();
    }

    private void Start()
    {
        InitialiseVehicle();
    }

    private void Update()
    {
        ControlVehicle();
        DetectAndAvoidObstacles();
        ProcessNavigation();
    }

    private void InitialiseVehicle()
    {
        switch (driver.driverType)
        {
            case DriverType.CAREFUL:
                kitManager.kitIndiceSettings = kitManager.RandomKit();
                kitManager.RandomisePaintJob();
                break;
            case DriverType.RECKLESS:
                kitManager.kitIndiceSettings = kitManager.RandomKit();
                kitManager.RandomisePaintJob();
                break;

            case DriverType.NPC:
                kitManager.kitIndiceSettings = new();
                break;

            case DriverType.COP:
                break;
        }

        kitManager.InitialiseBodyKit();
        kitManager.InitialisePaintJob();
    }


    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    public void SetTargetSpeed(float targetSpeed) => this.targetSpeed = targetSpeed;

    private void ProcessNavigation()
    {
        if (targetTransform != null || targetPosition != null)
        {
            remainingDistance = Vector3.Distance(transform.position, targetTransform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(targetPosition, Vector3.one);
    }

    private void DetectAndAvoidObstacles()
    {
        #region ORIGIN_POINTS
        Vector3 rightsideOrigin = transform.position + transform.up * .5F + transform.right;
        Vector3 leftsideOrigin = transform.position + transform.up * .5F + -transform.right;

        Vector3 rightsideFenderOrigin = transform.position + transform.forward * 2 + transform.up * .5F + transform.right;
        Vector3 leftsideFenderOrigin = transform.position + transform.forward * 2 + transform.up * .5F + -transform.right;

        Vector3 rightBlindSpotOrigin = transform.position + transform.up * .5F + transform.forward + transform.right * 2F;
        Vector3 leftBlindSpotOrigin = transform.position + transform.up * .5F + transform.forward + -transform.right * 2F;

        Vector3 frontMidOrigin = transform.position + transform.forward * 3F + transform.up * .5F;
        Vector3 frontLeftOrigin = transform.position + transform.forward * 3F + transform.up * .5F + -transform.right;
        Vector3 frontRightOrigin = transform.position + transform.forward * 3F + transform.up * .5F + transform.right;

        Vector3 rearMidOrigin = transform.position + -transform.forward * 3F + transform.up * .5F;
        #endregion

        float newSteeringInput = 0;

        targetBrake = 0;

        #region RAYCASTS

        if (Physics.Raycast(rightsideOrigin, transform.right, out RaycastHit rightHit, rayLength))
        {
            Debug.DrawLine(rightsideOrigin, rightHit.point, Color.green);

            //How much should the AI stand its ground and not avoid the obstacle?

            float tolerance = 0.6F;

            newSteeringInput += Mathf.Clamp(rightHit.distance - tolerance, -1, 0);
        }

        if (Physics.Raycast(leftsideOrigin, -transform.right, out RaycastHit leftHit, rayLength))
        {
            Debug.DrawLine(leftsideOrigin, leftHit.point, Color.green);

            //How much should the AI stand its ground and not avoid the obstacle?

            float tolerance = 0.6F;

            newSteeringInput -= Mathf.Clamp(rightHit.distance - tolerance, -1, 0);
        }

        if (Physics.Raycast(rightsideFenderOrigin, transform.right, out RaycastHit rightFNHit, rayLength))
        {
            Debug.DrawLine(rightsideFenderOrigin, rightFNHit.point, Color.green);

            //How much is the AI willing to risk damaging the fender?
            float tolerance = 0.25F;

            targetBrake += Mathf.Clamp01(1 - rightFNHit.distance - tolerance);
            newSteeringInput += Mathf.Clamp(rightHit.distance - tolerance * 1.5F, -1, 0);
        }

        if (Physics.Raycast(leftsideFenderOrigin, -transform.right, out RaycastHit leftFNHit, rayLength))
        {
            Debug.DrawLine(leftsideFenderOrigin, leftFNHit.point, Color.green);

            //How much is the AI willing to risk damaging the fender?
            float tolerance = 0.25F;

            targetBrake += Mathf.Clamp01(1 - rightFNHit.distance - tolerance);
            newSteeringInput -= Mathf.Clamp(rightHit.distance - tolerance * 1.5F, -1, 0);
        }

        if (Physics.Raycast(rightBlindSpotOrigin, -transform.forward, out RaycastHit rightBLNDSpt, rayLength * 2))
        {
            Debug.DrawLine(rightBlindSpotOrigin, rightBLNDSpt.point, Color.red);
        }

        if (Physics.Raycast(leftBlindSpotOrigin, -transform.forward, out RaycastHit leftBLNDSpt, rayLength * 2))
        {
            Debug.DrawLine(leftBlindSpotOrigin, leftBLNDSpt.point, Color.red);
        }

        if (Physics.Raycast(frontMidOrigin, transform.forward, out RaycastHit frontMidHit, rayLength))
        {
            Debug.DrawLine(frontMidOrigin, frontMidHit.point, Color.green);
        }

        if (Physics.Raycast(rearMidOrigin, -transform.forward, out RaycastHit rearMidHit, rayLength))
        {
            Debug.DrawLine(rearMidOrigin, rearMidHit.point, Color.green);

            targetThrottle += Mathf.Clamp01(1 - rearMidHit.distance);
        }

        if (Physics.Raycast(frontLeftOrigin, transform.forward, out RaycastHit frontLeftHit, rayLength))
        {
            Debug.DrawLine(frontLeftOrigin, frontLeftHit.point, Color.green);
        }

        if (Physics.Raycast(frontLeftOrigin, transform.forward + -transform.right, out RaycastHit front45LHit, rayLength))
        {
            Debug.DrawLine(frontLeftOrigin, front45LHit.point, Color.green);
        }

        if (Physics.Raycast(frontRightOrigin, transform.forward, out RaycastHit frontRightHit, rayLength))
        {
            Debug.DrawLine(frontRightOrigin, frontRightHit.point, Color.green);
        }

        if (Physics.Raycast(frontRightOrigin, transform.forward + transform.right, out RaycastHit front45RHit, rayLength))
        {
            Debug.DrawLine(frontRightOrigin, front45RHit.point, Color.green);
        }
        #endregion

        input.Steering = Mathf.Lerp(input.Steering, newSteeringInput, Time.deltaTime * 3F);
        input.Brake = Mathf.Lerp(input.Brake, targetBrake, Time.deltaTime * 3F);
        input.RawThrottle = Mathf.Lerp(input.RawThrottle, targetThrottle, Time.deltaTime * 3F);
    }

    private void ControlVehicle()
    {
        //Steer in correct direction

        if (targetTransform != null)
        {
            Vector3 steerVector;

            steerVector = transform.InverseTransformPoint
                    (
                        new
                            (
                                targetTransform.position.x,
                                transform.position.y,
                                targetTransform.position.z
                            )
                    );

            steerDirection = steerVector.x / steerVector.magnitude;

            input.Steering = steerDirection;
        }

        switch (driver.driverType)
        {
            case DriverType.CAREFUL:

                //Slow down well before corners.
                if (vehicle.SpeedKMH > targetSpeed)
                {
                    targetBrake += 1 - (vehicle.SpeedKMH / targetSpeed);
                }

                //Release accelerator when the wheels slip, if they slip too much, Brake.

                if (vehicle.GetPoweredWheelSlip() >= 0.5F)
                {
                    targetThrottle = 0;

                    if (vehicle.GetPoweredWheelSlip() >= 0.75F)
                        targetBrake += 1 - vehicle.GetPoweredWheelSlip() / 0.75F;
                }

                //Only floor it when the wheels are straight
                //Keep a safe driving distance from other racers 

                if (targetTransform != null)
                {
                    float t = Mathf.Abs(input.Steering);

                    //Lerp from 0 - 1 based on how hard the car is being steered.

                    //Getting absolute because steering is between -1 & 1

                    if (vehicle.SpeedKMH < targetSpeed)
                        targetThrottle = Mathf.Lerp(0.25F, 1.0F, 1 - t);
                }

                break;

            case DriverType.RECKLESS:

                //Slow down a few metres away from a corner.
                //Feather the throttle if the wheels are slipping and then floor it when they're straight.
                //Get up close and personal to other racers, Brake if the distance is too close.

                break;

            case DriverType.NPC:

                //Drive at a random speed between the minSpeed and the speed limit
                //Slow down well before corners.
                //Shift early? Idk how that's gonna work.
                //Keep a safe driving distance.
                //If you detect police with their siren on, move out the way.

                break;

            case DriverType.COP:
                //Like reckless driver.
                break;
        }
    }
}

[System.Serializable]
public struct Driver
{
    public DriverType driverType;
    [Range(10, 400)]
    public float minSpeed;
    [Range(10, 400)]
    public float maxSpeed;
    public bool IsInBrakeZone;
}

public enum DriverType
{
    CAREFUL,
    RECKLESS,
    NPC,
    COP
}