using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    static readonly string path = Application.persistentDataPath + "/RACING_GAME.save";

    public static bool TrySave(Vehicle vehicle)
    {
        try
        {
            FileStream stream = new(path, FileMode.Create);

            BinaryFormatter formatter = new();

            VehicleBodyKitManager kitManager = vehicle.GetComponent<VehicleBodyKitManager>();

            PlayerData data = new(kitManager.bodyKit.vehicleName, kitManager.kitIndiceSettings, kitManager.paintJobSettings);

            Debug.Log(data.vehicleName);

            formatter.Serialize(stream, data);

            stream.Close();

            Debug.Log("Saved Successfully");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            return false;
        }
    }

    public static PlayerData Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();

            FileStream stream = new(path, FileMode.Open);

            PlayerData data = (PlayerData)formatter.Deserialize(stream);

            stream.Close();

            return data;
        }

        Debug.LogError("Failed to load save file from " + path);

        return null;
    }

    public static bool TryLoad(out PlayerData playerData)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();

            FileStream stream = new(path, FileMode.Open);

            playerData = (PlayerData)formatter.Deserialize(stream);

            stream.Close();

            return true;
        }

        Debug.LogError("Failed to load save file from " + path);

        playerData = null;

        return false;
    }
}