using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] CameraSettings[] camSettings;

    [SerializeField] IdleCameraSettings idleSettings;

    [SerializeField] float rotationTime = 0.25F;

    [SerializeField] float handHeldIntensity;

    private int camIndex;

    private float idleTimer = 0;

    private const float MAX_IDLE_TIME = 10.0F;

    private Transform target;

    public static CameraController instance;

    new private Camera camera;

    private void Awake()
    {
        if (instance is null)
            instance = this;
        else
            Destroy(gameObject);

        InitialiseCamera();
    }

    private void InitialiseCamera()
    {
        camera = GetComponentInChildren<Camera>();
    }

    Vector3 velocity;

    Vector3 idleCameraVelocity;

    private void Update()
    {
        if (UseIdleCamera())
        {
            return;
        }

        Vector3 targetPos = target.position;

        float t = Player.instance.Vehicle.SpeedKMH / camSettings[camIndex].maxFOVSpeed;

        camera.fieldOfView = Mathf.Lerp(camSettings[camIndex].minFOV, camSettings[camIndex].maxFOV, t);

        Vector3 normalPosition = (targetPos - transform.forward * -camSettings[camIndex].cameraPosition.z + transform.up * camSettings[camIndex].cameraPosition.y);

        Vector3 highSpeedPosition = (targetPos - transform.forward * -camSettings[camIndex].maxedFOVPosition.z + transform.up * camSettings[camIndex].maxedFOVPosition.y);

        transform.position = Vector3.Lerp(normalPosition, highSpeedPosition, t);

        transform.forward = Vector3.SmoothDamp(transform.forward, target.forward, ref velocity, rotationTime);
    }

    public bool UseIdleCamera()
    {
        float flooredVehicleSpeed = Mathf.FloorToInt(Player.instance.Vehicle.SpeedKMH);

        if (flooredVehicleSpeed <= 0)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= MAX_IDLE_TIME)
                idleTimer = MAX_IDLE_TIME;
        }
        else
            idleTimer = 0;

        if (idleTimer >= MAX_IDLE_TIME)
        {
            Vector3 idlePosition = (target.position - transform.forward * -idleSettings.cameraPosition.z + transform.up * idleSettings.cameraPosition.y + transform.right * idleSettings.cameraPosition.x);

            transform.position = Vector3.SmoothDamp(transform.position, idlePosition, ref idleCameraVelocity, 2.0F);

            transform.eulerAngles = Vector3.up * idleSettings.yAxisRotation;

            camera.fieldOfView = idleSettings.FOV;

            return true;
        }

        return false;
    }

    public void SetCameraFocus(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Camera focus set to : " + newTarget);
    }
}

[System.Serializable]
public struct CameraSettings
{
    public Vector3 cameraPosition, maxedFOVPosition;

    [Range(10, 100)] public float minFOV, maxFOV;

    [Range(10, 250)] public float maxFOVSpeed;
}

[System.Serializable]
public struct IdleCameraSettings
{
    public Vector3 cameraPosition;

    [Range(-360, 360)] public float yAxisRotation;

    [Range(10, 100)] public float FOV;

}