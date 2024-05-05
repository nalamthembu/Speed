using ThirdPersonFramework.UserInterface;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
    [SerializeField] TyreScriptable tyre;
    [SerializeField] RimScriptable rim;
    [SerializeField] Transform wheelMeshParent;
    [SerializeField] WheelPosition wheelPosition;
    [SerializeField] WheelOutfacingDirection wheelSide;
    public RimScriptable Rim { get { return rim; } set { rim = value; } }

    //"r = Realtime, or Item that is spawned in during gameplay
    private WheelSlip slip;
    private GameObject rTyre;
    private GameObject rRim;

    GameObject rWheelObject = null;
    new WheelCollider collider;

    //Public accessors
    public float RPM { get { return collider.rpm; } }
    public WheelSlip WheelSlip { get { return slip; } }
    public WheelPosition Position { get { return wheelPosition; } }
    public Vector3 WorldPos { get; private set; }

    public float MaxSteeringAngle { get; private set; }

    public Quaternion WorldRot { get; private set; }

    bool m_GamePaused;

    private void Awake()
    {
        SpawnWheelAndRim();
    }

    public void SetMotorTorque(float torque) => collider.motorTorque = torque * 1.25F;
    public void SetBrakeTorque(float bTorque) => collider.brakeTorque = bTorque * bTorque;
    public void SetSteerAngle(float steerAngle) => collider.steerAngle = steerAngle;
    public bool IsGrounded
    {
        get
        {
            return collider.isGrounded;
        }
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

    void OnGamePaused() => m_GamePaused = true;
    void OnGameResumed() => m_GamePaused = false;

    public void FixedUpdate()
    {
        if (m_GamePaused)
            return;

        collider.GetGroundHit(out WheelHit hit);
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        WorldPos = pos;
        WorldRot = rot;
        rWheelObject.transform.SetPositionAndRotation(pos, rot);
        slip.forward = hit.forwardSlip;
        slip.sideways = hit.sidewaysSlip;
        slip.debugReadout = hit.forwardSlip / collider.forwardFriction.extremumSlip;
    }

    public void SetWheelStiffness(float sideWaysStiffness, float fwdStiffness)
    {
        WheelFrictionCurve sidewaysFric = collider.sidewaysFriction;
        WheelFrictionCurve fwdFric = collider.forwardFriction;

        sidewaysFric.stiffness = sideWaysStiffness;
        fwdFric.stiffness = fwdStiffness;

        collider.sidewaysFriction = sidewaysFric;
        collider.forwardFriction = fwdFric;
    }

    public void SetMaxSteeringAngle(float maxSteeringAngle)
    {
        MaxSteeringAngle = maxSteeringAngle;
    }

    public float GetAngle() => Mathf.Abs(collider.steerAngle);

    private void ResizeWheelCollider()
    {
        //Change : Of course this object wouldn't have a mesh renderer on it!
        //You need to get it from the tyre *Mesh* itself
        if (tyre.mesh.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            Bounds b = meshRenderer.bounds;

            if (collider is null)
            {
                Debug.LogError("Wheel Collider is null!");
                return;
            }
              
            collider.radius = b.size.z / 2;
        }
        else
        {
            Debug.LogError("Could not find MeshRenderer");
        }
    }

    [ContextMenu("Spawn Wheel (DEBUG)")]
    public void SpawnWheelAndRim()
    {
        if (collider is null)
            collider = GetComponent<WheelCollider>();

        Vector3 wheelDirection = Vector3.zero;

        if (rTyre)
            Destroy(rTyre);

        switch (wheelSide)
        {
            case WheelOutfacingDirection.LEFT:

                switch (wheelPosition)
                {
                    case WheelPosition.BACK:
                        rWheelObject = new("WheelBL");
                        break;

                    case WheelPosition.FRONT:
                        rWheelObject = new("WheelFL");
                        break;
                }

                wheelDirection = Vector3.up * 180;

                break;


            case WheelOutfacingDirection.RIGHT:

                switch (wheelPosition)
                {
                    case WheelPosition.BACK:
                        rWheelObject = new("WheelBR");
                        break;

                    case WheelPosition.FRONT:
                        rWheelObject = new("WheelFR");
                        break;
                }

                wheelDirection = Vector3.up * 0;

                break;
        }

        rWheelObject.transform.parent = wheelMeshParent;

        rTyre = Instantiate(tyre.mesh, rWheelObject.transform.position, Quaternion.Euler(wheelDirection), rWheelObject.transform);
        rRim = Instantiate(rim.mesh, rTyre.transform.position, rTyre.transform.rotation, rTyre.transform);
        ResizeWheelCollider();
    }
}

#region STRUCTS
[System.Serializable]
public struct WheelSlip
{
    public float forward;
    public float sideways;
    public float debugReadout;
}

public enum WheelOutfacingDirection
{
    LEFT,
    RIGHT
}

public enum WheelPosition
{
    BACK,
    FRONT
}
#endregion