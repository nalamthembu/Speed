using UnityEngine;

//This script keeps track of inputs for the vehicle, it will be used by other scripts under the vehicle object.
public class VehicleInput : MonoBehaviour
{
    [Range(0, 1)] float throttle;
    [Range(0, 1)] float brake;
    [Range(0, 1)] float handbrake;
    [Range(-1, 1)] float steering;

    private bool isInReverse;

    private float rawThrottle; //Includes negative numbers if the player/ai decides to reverse.

    #region ACCESSORS
    public float Throttle { get { return throttle; } set { throttle = value; } }
    public float RawThrottle { get { return rawThrottle; } set { rawThrottle = value; } }
    public float Brake { get { return brake; } set { brake = value; } }
    public float Handbrake { get { return handbrake; } set { handbrake = value; } }
    public float Steering { get { return steering; } set { steering = value; } }
    public bool IsInReverse { get { return isInReverse; } set { isInReverse = value; } }
    #endregion

    private void Update() => FloorValues();

    private void FloorValues()
    {
        brake = Mathf.Floor(brake);
        Handbrake = Mathf.Floor(Handbrake);
    }
}