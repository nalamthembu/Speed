using TMPro;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
    [SerializeField] CameraView[] cameraViews;

    [SerializeField] Camera garageCamera;

    [SerializeField] Vector3[] cameraMatrix = new Vector3[3]; //Position, Rotation, FOV(x) only 

    [SerializeField] Transform vehicleSpawnPoint;

    [SerializeField] TMP_Text vehicleName_SelectionScreen;

    private string DEBUG_CURRENT_CAM_VIEW;

    //TO-DO : SAVE DISPLAY VEHICLE TO FILE FOR PLAYER TO SPAWN IN.

    Vehicle displayVehicle;

    public static GarageManager instance;

    public bool DEBUG_MODE;
    public Transform VehicleSpawnPoint { get { return vehicleSpawnPoint; } }

    public void SetDisplayVehicle(string vehicleName)
    {
        if (displayVehicle != null)
            Destroy(displayVehicle.gameObject);

        displayVehicle =
        VehicleManager.instance.SpawnVehicle
        (
            vehicleName,
            new Vector3[]
            { vehicleSpawnPoint.position,
                    vehicleSpawnPoint.eulerAngles
            }
         );

        if (vehicleName_SelectionScreen is null)
            Debug.LogError("Selection screen is not available");

        vehicleName_SelectionScreen.text = vehicleName;
    }

    public void ChangePartOnDisplayVehicle(string partNameAndChangeDirection)
    {
        print("entering change part method");

        string part = string.Empty;

        int direction = -1;

        for (int i = 0; i < partNameAndChangeDirection.Length; i++)
        {
            if (i > partNameAndChangeDirection.IndexOf('_') - 1)
            {
                print(partNameAndChangeDirection[i]);

                if (int.TryParse(partNameAndChangeDirection[i].ToString(), out int result))
                {
                    direction = result;

                    break;
                }
            }
            else
            {
                part += partNameAndChangeDirection[i];
            }
        }

        part.ToLower();

        print(part);

        VehicleBodyKitManager kit = displayVehicle.GetComponent<VehicleBodyKitManager>();

        switch (part)
        {
            case "bonnet":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.bonnet > 0)
                        kit.kitIndiceSettings.bonnet--;
                    else
                        kit.kitIndiceSettings.bonnet = kit.bodyKit.bonnets.Length - 1;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.bonnet < kit.bodyKit.bonnets.Length - 1)
                        kit.kitIndiceSettings.bonnet++;
                    else
                        kit.kitIndiceSettings.bonnet = 0;
                }

                break;

            case "paint":

                if (direction.Equals(0))
                {
                    if (kit.paintJobSettings.paintJobIndex > 0)
                        kit.paintJobSettings.paintJobIndex--;
                    else
                        kit.paintJobSettings.paintJobIndex = 37; //HARD_CODE_IT_FOR_NOW_BECAUSE_IK_HOW_MANY_PAINT_JOBS_THERE_ARE
                }

                if (direction.Equals(1))
                {
                    if (kit.paintJobSettings.paintJobIndex < 37)
                        kit.paintJobSettings.paintJobIndex++;
                    else
                        kit.paintJobSettings.paintJobIndex = 0;
                }

                break;

            case "fbumper":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.frontBumper > 0)
                        kit.kitIndiceSettings.frontBumper--;
                    else
                        kit.kitIndiceSettings.frontBumper = kit.bodyKit.frontBumpers.Length - 1;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.frontBumper < kit.bodyKit.frontBumpers.Length - 1)
                        kit.kitIndiceSettings.frontBumper++;
                    else
                        kit.kitIndiceSettings.frontBumper = 0;
                }

                break;

            case "rbumper":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.rearBumper > 0)
                        kit.kitIndiceSettings.rearBumper--;
                    else
                        kit.kitIndiceSettings.rearBumper = kit.bodyKit.rearBumpers.Length - 1;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.rearBumper < kit.bodyKit.rearBumpers.Length - 1)
                        kit.kitIndiceSettings.rearBumper++;
                    else
                        kit.kitIndiceSettings.rearBumper = 0;
                }

                break;

            case "skirt":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.sideSkirt > 0)
                        kit.kitIndiceSettings.sideSkirt--;
                    else
                        kit.kitIndiceSettings.sideSkirt = kit.bodyKit.sideSkirts.Length - 1;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.sideSkirt < kit.bodyKit.sideSkirts.Length - 1)
                        kit.kitIndiceSettings.sideSkirt++;
                    else
                        kit.kitIndiceSettings.sideSkirt = 0;
                }

                break;

            case "spoiler":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.spoiler > 0)
                        kit.kitIndiceSettings.spoiler--;
                    else
                        kit.kitIndiceSettings.spoiler = kit.bodyKit.spoilers.Length - 1;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.spoiler < kit.bodyKit.spoilers.Length - 1)
                        kit.kitIndiceSettings.spoiler++;
                    else
                        kit.kitIndiceSettings.spoiler = 0;
                }

                break;

            case "fender":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.fender > 0)
                        kit.kitIndiceSettings.fender--;
                    else
                        kit.kitIndiceSettings.fender = kit.bodyKit.fenders.Length - 1;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.fender < kit.bodyKit.fenders.Length - 1)
                        kit.kitIndiceSettings.fender++;
                    else
                        kit.kitIndiceSettings.fender = 0;
                }

                break;

            //TO-DO : ADD CHANGE - RIMS

            case "rim":

                if (direction.Equals(0))
                {
                    if (kit.kitIndiceSettings.rims > 0)
                        kit.kitIndiceSettings.rims--;
                    else
                        kit.kitIndiceSettings.rims = kit.bodyKit.rims.Length;
                }

                if (direction.Equals(1))
                {
                    if (kit.kitIndiceSettings.rims < kit.bodyKit.rims.Length - 1)
                        kit.kitIndiceSettings.rims++;
                    else
                        kit.kitIndiceSettings.rims = 0;
                }

                break;

                //TO-DO : ADD CHANGE - ROLLCAGE


        }

        //TO-DO : SAVE ALL CHANGES
        SaveVehicle();

        kit.InitialiseBodyKit();
    }

    private void Awake()
    {
        if (instance is null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (garageCamera is null)
        {
            Debug.LogError("**DEBUG** : Garage Camera is NULL");
        }
    }

    private void Start()
    {
        string randomVehicleName = VehicleManager.instance.vehicleLib.vehicles[Random.Range(0, VehicleManager.instance.vehicleLib.vehicles.Length)].name;

        vehicleName_SelectionScreen.text = randomVehicleName;

        GoToCameraView(cameraViews[0].name);

        if (SaveSystem.TryLoad(out PlayerData data))
        {
            if (data is null)
                print("could not load game");
            else
            {
                displayVehicle =
                    VehicleManager.instance.SpawnVehicle
                    (
                        data.vehicleName,
                        new Vector3[]
                        {
                            vehicleSpawnPoint.position,
                            vehicleSpawnPoint.eulerAngles
                        }
                    );

                if (displayVehicle is null)
                {
                    Debug.LogError("**VEHICLE IS NULL**");
                    return;
                }

                //SETUPKIT
                var kit = displayVehicle.GetComponent<VehicleBodyKitManager>();
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

            print("vehicle loaded successfully");
        }
    }

    public void GoToCameraView(string viewName)
    {
        for (int i = 0; i < viewName.Length; i++)
        {
            if (cameraViews[i].name.Equals(viewName))
            {
                cameraMatrix = new Vector3[]
                {
                    cameraViews[i].position,
                    cameraViews[i].rotation,
                    Vector3.one * cameraViews[i].fieldOfView
                };

                DEBUG_CURRENT_CAM_VIEW = viewName;

                return;
            }
        }
    }

    Vector3 camPosVel;
    float fovVel;

    private void LateUpdate()
    {
        if (DEBUG_MODE)
            GoToCameraView(DEBUG_CURRENT_CAM_VIEW);

        garageCamera.transform.position = Vector3.SmoothDamp(garageCamera.transform.position, cameraMatrix[0], ref camPosVel, 0.5F);
        garageCamera.transform.rotation = Quaternion.Lerp(garageCamera.transform.rotation, Quaternion.Euler(cameraMatrix[1]), Time.deltaTime * 7.5F);
        garageCamera.fieldOfView = Mathf.SmoothDamp(garageCamera.fieldOfView, cameraMatrix[2].x, ref fovVel, .96F);
    }

    public void SaveVehicle()
    {
        SaveSystem.TrySave(displayVehicle);
    }

    //DEBUG
    private void OnDrawGizmos()
    {
        if (cameraViews.Length > 0)
        {
            Gizmos.color = Color.cyan;

            foreach (CameraView view in cameraViews)
            {
                Gizmos.DrawWireCube(view.position, Vector3.one * 0.25f);
            }
        }
    }
}

[System.Serializable]
public struct CameraView
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public float fieldOfView;
}