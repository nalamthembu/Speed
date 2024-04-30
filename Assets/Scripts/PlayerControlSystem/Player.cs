using System;
using Unity.VisualScripting;
using UnityEngine;
using VehiclePhysics;

public class Player : MonoBehaviour
{
    //I ONLY WANT ONE INSTANCE OF THE PLAYER IN ANY SCENE.
    public static Player Instance;

    [SerializeField] GameObject m_PlayerCharacterPrefab;

    [SerializeField] GameObject m_GameplayCameraPrefab;

    [SerializeField] Vehicle vehicle;

    public Vehicle Vehicle { get { return vehicle; } }

    public static event Action OnPlayerInitialised;

    private bool m_Initialized;

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //Player Camera
        Instantiate(m_GameplayCameraPrefab);
    }

    private void OnDestroy()
    {
        Debug.Log("Player Was Destroyed!");

        Instance = null;
    }


    [ContextMenu("Debug : Initialise Player")]
    public void InitialisePlayer()
    {
        if (VehicleManager.instance == null)
        {
            Debug.LogError("There is no Vehicle Manager in this scene!");
            return;
        }

        Vehicle vehicle = null;

        if (SaveSystem.TryLoad(out PlayerData data))
        {
            if (data is null) //Could not Load Game
            {
                vehicle =
                    VehicleManager.instance.SpawnVehicle
                    (
                        VehicleManager.instance.vehicleLib.vehicles[0].name,
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
                    VehicleManager.instance.SpawnVehicle
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
        if (vehicle != null)
        {
            CameraController.Instance.SetCameraFocus(vehicle.CameraFocus);
            OnPlayerInitialised?.Invoke();
            m_Initialized = true;

            if (RacingHUD.Instance)
                RacingHUD.Instance.SetMaxRev(vehicle.PeakEngineRPM);
        }
    }

    private void Initialise(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.vehicle.transform.parent = transform;
        this.vehicle.transform.SetLocalPositionAndRotation(transform.localPosition, transform.localRotation);
        this.vehicle.GetComponent<VPStandardInput>().enabled = true;
    }

    private void Update()
    {
        if (RacingHUD.Instance && vehicle)
        {
            RacingHUD.Instance.SetRev(vehicle.EngineRPM);
            RacingHUD.Instance.SetSpeedometer(vehicle.SpeedKMH);
        }
    }
}