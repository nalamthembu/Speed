using UnityEngine;

[CreateAssetMenu(fileName = "CameraViewData", menuName = "Game/Garage/Camera View Data")]
public class CameraViewData : ScriptableObject
{
    public CameraView[] CameraViews;
}

[System.Serializable]
public struct CameraView
{
    public string ID;
    public Vector3 position;
    public Vector3 rotation;
    public float fieldOfView;
}