using RaceSystem;
using UnityEngine;

public class Player : Racer
{
    //I ONLY WANT ONE INSTANCE OF THE PLAYER IN ANY SCENE.
    public static Player instance;

    protected override void Awake()
    {
        if (instance is null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void InitialisePlayer()
    {
        Vehicle vehicle;

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

                if (vehicle is null)
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

        CameraController.Instance.SetCameraFocus(this.vehicle.GetComponent<PlayerVehicleInput>().camera_Focus);
        GameManager.Instance.InitPlayer();
    }

    public void Initialise(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.vehicle.transform.parent = transform;
        this.vehicle.transform.SetLocalPositionAndRotation(transform.localPosition, transform.localRotation);
        this.vehicle.GetComponent<PlayerVehicleInput>().playerControlEnabled = true;
    }
}