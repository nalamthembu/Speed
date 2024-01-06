using UnityEngine;

public class Character : MonoBehaviour
{
    Vehicle m_Vehicle;

    public void SetVehicle(Vehicle vehicle)
    {
        m_Vehicle = vehicle;
    }
}