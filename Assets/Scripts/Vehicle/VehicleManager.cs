using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager Instance = null;

    public VehicleSpawnerScriptable vehicleLib;

    Dictionary<string, GameObject> vehicleDictionary;

    int vehicleCount;
    int selectedVehicle;

    //r- realtime vehicles (spawned).
    List<Vehicle> rVehicles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitVehicleManager();
    }

    private void InitVehicleManager()
    {
        vehicleDictionary = new();
        rVehicles = new();

        for (int i = 0; i < vehicleLib.vehicles.Length; i++)
        {
            vehicleDictionary.Add(vehicleLib.vehicles[i].name, vehicleLib.vehicles[i].vehicle);
        }

        vehicleCount = vehicleLib.vehicles.Length;
    }

    [ContextMenu("Spawn Random Vehicle (DEBUG)")]
    public void SpawnRandomVehicle()
    {
        string vName = vehicleLib.vehicles[Random.Range(0, vehicleLib.vehicles.Length)].name;
        Vector3 pos = new(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        SpawnVehicle(vName, pos, pos * 0);
    }


    public void SpawnVehicle(string name, Vector3 position, Vector3 rotation)
    {
        if (vehicleDictionary.TryGetValue(name, out GameObject vGO))
        {
            GameObject rVehicle = Instantiate(vGO, position, Quaternion.Euler(rotation));

            Vehicle v = rVehicle.GetComponent<Vehicle>();

            rVehicles.Add(v);
        }
    }
    public Vehicle SpawnVehicle(string name, Vector3[] pos_rot)
    {
        if (vehicleDictionary.TryGetValue(name, out GameObject vGO))
        {
            GameObject rVehicle = Instantiate(vGO, pos_rot[0], Quaternion.Euler(pos_rot[1]));

            Vehicle v = rVehicle.GetComponent<Vehicle>();

            rVehicles.Add(v);

            return v;
        }

        return null;
    }

    public int GetVehicleCount() => vehicleCount;

    public void SetSelectedVehicle(int selectedVehicle)
    {
        if (selectedVehicle > vehicleCount - 1)
            selectedVehicle = 0;

        if (selectedVehicle < 0)
            selectedVehicle = vehicleCount - 1;

        this.selectedVehicle = selectedVehicle;
    }

    public int GetSelectedVehicle() => selectedVehicle;

    public bool TryGetSelectedVehicleName(int index, out string vehicleName)
    {
        for (int i = 0; i < vehicleCount; i++)
        {
            if (i == selectedVehicle)
            {
                vehicleName = vehicleLib.vehicles[i].name;

                return true;
            }
        }

        Debug.LogError("Could not find vehicle index : " + index);

        vehicleName = string.Empty;

        return false;
    }

    public void SpawnVehicleWithDriverType(string name, Vector3 position, Vector3 rotation)
    {
        SpawnVehicle
        (
            name,
            new Vector3[]
            {
                position,
                rotation
            }
         );

    }
}