using System;
using UnityEngine;
using VehiclePhysics;

public class Player : MonoBehaviour
{
    //I ONLY WANT ONE INSTANCE OF THE PLAYER IN ANY SCENE.
    public static Player Instance;

    [SerializeField] GameObject m_PlayerCharacterPrefab;

    [SerializeField] GameObject m_CameraRigPrefab;

    Vehicle m_Vehicle;

    public Vehicle Vehicle { get { return m_Vehicle; } }

    public static event Action OnPlayerInitialised;

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Player Was Destroyed!");

        Instance = null;
    }

    [ContextMenu("Debug/Initialise Player")]
    public void InitialisePlayer()
    {
        Vehicle vehicle;

        if (SaveSystem.TryLoad(out PlayerData data))
        {
            if (data == null) //Could not Load Game
            {
                vehicle =
                    VehicleManager.Instance.SpawnVehicle
                    (
                        VehicleManager.Instance.vehicleLib.vehicles[0].name,
                        new Vector3[]
                        {
                            transform.localPosition,
                            transform.localEulerAngles
                        }
                    );

                Initialise(vehicle);
            }
            else //Loading was successful.
            {
                vehicle =
                    VehicleManager.Instance.SpawnVehicle
                    (
                        data.vehicleName,
                        new Vector3[]
                        {
                            transform.localPosition,
                            transform.localEulerAngles
                        }
                    );

                if (vehicle == null)
                {
                    Debug.LogError("**VEHICLE IS NULL**");
                    return;
                }

                Initialise(vehicle);

                //SETUPKIT
                var kit = vehicle.GetComponent<VehicleBodyKitManager>();
                kit.kitIndiceSettings.bonnet = data.kitIndices[0];
                kit.kitIndiceSettings.fender = data.kitIndices[1];
                kit.kitIndiceSettings.frontBumper = data.kitIndices[2];
                kit.kitIndiceSettings.rearBumper = data.kitIndices[3];
                kit.kitIndiceSettings.rollCage = data.kitIndices[4];
                kit.kitIndiceSettings.roofScoop = data.kitIndices[5];
                kit.kitIndiceSettings.sideSkirt = data.kitIndices[6];
                kit.kitIndiceSettings.spoiler = data.kitIndices[7];
                kit.kitIndiceSettings.rims = data.kitIndices[8];
                kit.paintJobSettings.paintJobIndex = data.paintJobIndex;

                kit.InitialiseBodyKit();
            }
        }

        // DESTORY Temporary Camera
        Destroy(Camera.main.gameObject);

        // Spawn Camera
        Instantiate(m_CameraRigPrefab, Vehicle.transform.position - Vehicle.transform.forward * 3, Quaternion.identity);

        // Setup Race UI
        RacingHUD.Instance.SetMaxRev(Vehicle.Engine.MaxRPM);

        CameraController.Instance.SetCameraFocus(Vehicle.GetComponent<PlayerVehicleInput>().camera_Focus);

        OnPlayerInitialised?.Invoke();
    }

    private void Initialise(Vehicle vehicle)
    {
        m_Vehicle = vehicle;
        m_Vehicle.transform.parent = transform;
        m_Vehicle.transform.SetLocalPositionAndRotation(transform.localPosition, transform.localRotation);
        m_Vehicle.GetComponent<PlayerVehicleInput>().playerControlEnabled = true;
    }

    private void Update()
    {
        if (m_Vehicle != null)
        {
            if (RacingHUD.Instance)
            {
                RacingHUD.Instance.SetRev(m_Vehicle.Engine.RPM);
                RacingHUD.Instance.SetSpeedometer(m_Vehicle.SpeedKMH);
                RacingHUD.Instance.SetGearIndicator((m_Vehicle.Transmission.CurrentGear + 1).ToString());
            }
        }
    }
}