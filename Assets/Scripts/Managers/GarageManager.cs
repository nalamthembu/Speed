using TMPro;
using UnityEngine;

public class GarageManager : MonoBehaviour
{

    [SerializeField] Transform m_VehicleSpawnPoint;

    [SerializeField] TMP_Text vehicleName_SelectionScreen;

    //TO-DO : SAVE DISPLAY VEHICLE TO FILE FOR PLAYER TO SPAWN IN.

    Vehicle displayVehicle;

    public static GarageManager instance;

    public void SetDisplayVehicle(string vehicleName)
    {
        if (displayVehicle != null)
            Destroy(displayVehicle.gameObject);

        displayVehicle =
        VehicleManager.instance.SpawnVehicle
        (
            vehicleName,
            new Vector3[]
            { m_VehicleSpawnPoint.position,
                    m_VehicleSpawnPoint.eulerAngles
            }
         );

        if (vehicleName_SelectionScreen is null)
            Debug.LogError("Selection screen is not available");

        vehicleName_SelectionScreen.text = vehicleName;

        displayVehicle.SetSimulatePhysics(false);

        displayVehicle.KillEngine();

        displayVehicle.transform.parent = m_VehicleSpawnPoint;
    }

    //0 references on this method means its being called by a OnClickEvent()

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
    }

    private void Start()
    {
        if (SaveSystem.TryLoad(out PlayerData data))
        {
            displayVehicle =
                VehicleManager.instance.SpawnVehicle
                (
                    data.vehicleName,
                    new Vector3[]
                    {
                            m_VehicleSpawnPoint.position,
                            m_VehicleSpawnPoint.eulerAngles
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

        if (data is null)
        {
            print("could not load game, loading random vehicle");

            string randomVehicleName = VehicleManager.instance.vehicleLib.vehicles[Random.Range(0, VehicleManager.instance.vehicleLib.vehicles.Length)].name;

            vehicleName_SelectionScreen.text = randomVehicleName;

            displayVehicle =
                VehicleManager.instance.SpawnVehicle
                (
                    randomVehicleName,
                    new Vector3[]
                    {
                            m_VehicleSpawnPoint.position,
                            m_VehicleSpawnPoint.eulerAngles
                    }
                );

            var kit = displayVehicle.GetComponent<VehicleBodyKitManager>();
            kit.InitialiseBodyKit();

            displayVehicle.SetSimulatePhysics(false);
            displayVehicle.transform.parent = m_VehicleSpawnPoint;
            displayVehicle.transform.localPosition = displayVehicle.transform.localEulerAngles *= 0;

            //Save that random ass vehicle.
            SaveVehicle();
        }

        print("vehicle loaded successfully");

        displayVehicle.SetSimulatePhysics(false);
        displayVehicle.transform.parent = m_VehicleSpawnPoint;
        displayVehicle.transform.localPosition = displayVehicle.transform.localEulerAngles *= 0;

        displayVehicle.KillEngine();

    }

    //LEFT RIGHT OPTIONS IN VEHICLE SELECT IN GARAGE.
    public void SpawnNextVehicleInGarage_LEFT()
    {
        VehicleManager.instance.SetSelectedVehicle(VehicleManager.instance.GetSelectedVehicle() - 1);
        if (VehicleManager.instance.TryGetSelectedVehicleName(VehicleManager.instance.GetSelectedVehicle(), out string vehicleName))
            SetDisplayVehicle(vehicleName);
    }

    public void SpawnNextVehicleInGarage_RIGHT()
    {
        VehicleManager.instance.SetSelectedVehicle(VehicleManager.instance.GetSelectedVehicle() + 1);
        if (VehicleManager.instance.TryGetSelectedVehicleName(VehicleManager.instance.GetSelectedVehicle(), out string vehicleName))
            SetDisplayVehicle(vehicleName);
    }

    public void LoadRace(int raceID) => LevelManager.Instance.LoadLevel(raceID);

    public void SaveVehicle()
    {
        SaveSystem.TrySave(displayVehicle);
    }
}