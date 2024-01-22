using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] CameraSettings[] camSettings;

    [SerializeField] IdleCameraSettings idleSettings;

    [SerializeField] float rotationTime = 0.25F;

    [SerializeField] float handHeldSmoothing, handHeldRange, handHeldSmoothTime;

    [SerializeField] LayerMask obstructionLayer;

    private int camIndex;

    private float idleTimer = 0;

    private Vector3 handHeldVelocity;

    private const float MAX_IDLE_TIME = 10.0F;

    private Transform target;

    public static CameraController Instance;

    new private Camera camera;

    public bool IsUsingIdleCamera { get; private set; }

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        InitialiseCamera();
    }

    private void OnDestroy()
    {
        Debug.Log("Gameplay Camera Destroyed!");
        Instance = null;
    }

    private void InitialiseCamera()
    {
        camera = GetComponentInChildren<Camera>();
    }

    Vector3 velocity;

    private void LateUpdate()
    {
        IsUsingIdleCamera = ShouldUseIdleCamera();

        if (IsUsingIdleCamera)
        {
            return;
        }

        Vector3 targetPos = target.position;

        float t = Player.instance.Vehicle.SpeedKMH / camSettings[camIndex].maxFOVSpeed;

        camera.fieldOfView = Mathf.Lerp(camSettings[camIndex].minFOV, camSettings[camIndex].maxFOV, t);

        Vector3 normalPosition = (targetPos - transform.forward * -camSettings[camIndex].cameraPosition.z + transform.up * camSettings[camIndex].cameraPosition.y);

        Vector3 highSpeedPosition = (targetPos - transform.forward * -camSettings[camIndex].maxedFOVPosition.z + transform.up * camSettings[camIndex].maxedFOVPosition.y);

        Vector3 desiredCamPos = Vector3.Lerp(normalPosition, highSpeedPosition, t);

        transform.position = desiredCamPos;

        transform.forward = Vector3.SmoothDamp(transform.forward, target.forward, ref velocity, rotationTime);
        DoHandHeldEffect(Player.instance.Vehicle.SpeedKMH / camSettings[camIndex].maxHandHeldSpeed, camSettings[camIndex].handHeldSmoothingTime);
    }

    public bool ShouldUseIdleCamera()
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
            transform.position = Vector3.Lerp(transform.position, target.position + idleSettings.GetCurrentCameraPreset().position, Time.deltaTime * 2);

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, idleSettings.GetCurrentCameraPreset().rotation, Time.deltaTime * 2);

            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, idleSettings.GetCurrentCameraPreset().fieldOfView, Time.deltaTime * 2);

            ShouldDoHandHeldEffect(true, handHeldSmoothing);

            idleSettings.Update();

            return true;
        }

        ShouldDoHandHeldEffect(false);

        return false;
    }

    private void DoHandHeldEffect(float percent, float smoothing = 5)
    {
        Vector3 handHeldPosition = new()
        {
            x = Mathf.Sin(Time.time * handHeldSmoothing) * Random.Range(-handHeldRange, handHeldRange),
            y = Mathf.Cos(Time.time * handHeldSmoothing) * Random.Range(-handHeldRange, handHeldRange),
            z = Random.Range(-handHeldRange, handHeldRange)
        };

        handHeldPosition *= Mathf.Lerp(0, 1, Mathf.Clamp01(percent));

        camera.transform.localPosition = Vector3.SmoothDamp(camera.transform.localPosition, handHeldPosition, ref handHeldVelocity, smoothing);
    }

    private void ShouldDoHandHeldEffect(bool handHeldEffectEnabled, float smoothing = 5)
    {

        if (handHeldEffectEnabled == true)
        { 
            Vector3 handHeldPosition = new()
            {
                x = Mathf.Sin(Time.time * handHeldSmoothing) * Random.Range(-handHeldRange, handHeldRange),
                y = Mathf.Cos(Time.time * handHeldSmoothing) * Random.Range(-handHeldRange, handHeldRange),
                z = Random.Range(-handHeldRange, handHeldRange)
            };

            camera.transform.localPosition = Vector3.SmoothDamp(camera.transform.localPosition, handHeldPosition, ref handHeldVelocity, smoothing);

            return;
        }

        camera.transform.localPosition = Vector3.zero;
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

    [Range(0, 100)] public float handHeldSmoothingTime;

    [Range(10, 250)] public float maxFOVSpeed, maxHandHeldSpeed;
}

[System.Serializable]
public struct IdleCameraSettings
{
    [SerializeField] IdleCameraPreset[] m_IdleCameraPresets;

    private IdleCameraPreset m_CurrentIdleCamPreset;

    [Range(1, 10)] public float cameraChangeFrequencyInSeconds;

    private float currentTime;

    int currentIndex;

    public IdleCameraPreset GetCurrentCameraPreset() => m_CurrentIdleCamPreset;

    public void SetTargetCameraPosition(int index)
    {
        m_CurrentIdleCamPreset = m_IdleCameraPresets[index];
    }

    public void Update()
    {
        if (m_IdleCameraPresets.Length > 0)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= cameraChangeFrequencyInSeconds)
            {
                currentIndex++;

                if (currentIndex >= m_IdleCameraPresets.Length)
                    currentIndex = 0;

                SetTargetCameraPosition(currentIndex);

                currentTime = 0;
            }
        }
    }
}

[System.Serializable]
public struct IdleCameraPreset
{
    public Vector3 position;

    public Vector3 rotation;

    public float fieldOfView;
}