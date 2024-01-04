using UnityEngine;

public class CameraViewManager : MonoBehaviour
{
    [SerializeField] CameraViewData cameraViewData;

    [SerializeField] Camera garageCamera;

    [SerializeField] Vector3[] cameraMatrix = new Vector3[3]; //Position, Rotation, FOV(x) only 

    public bool DEBUG_MODE;

    private string DEBUG_CURRENT_CAM_VIEW;

    Vector3 camPosVel;
    float fovVel;

    private void Awake()
    {
        if (garageCamera is null)
        {
            Debug.LogError("**DEBUG** : Garage Camera is NULL");
        }
    }

    private void Start()
    {
        if (cameraViewData.CameraViews.Length > 0)
        {
            GoToCameraView(cameraViewData.CameraViews[0].ID);
        }
    }

    public void GoToCameraView(string viewID)
    {
        for (int i = 0; i < cameraViewData.CameraViews.Length; i++)
        {
            if (cameraViewData.CameraViews[i].ID.Equals(viewID))
            {
                cameraMatrix = new Vector3[]
                {
                    cameraViewData.CameraViews[i].position,
                    cameraViewData.CameraViews[i].rotation,
                    Vector3.one * cameraViewData.CameraViews[i].fieldOfView
                };

                DEBUG_CURRENT_CAM_VIEW = viewID;

                return;
            }
        }
    }

    private void LateUpdate()
    {
        if (DEBUG_MODE)
            GoToCameraView(DEBUG_CURRENT_CAM_VIEW);

        Vector3 desiredPosition = cameraMatrix[0];

        if (Physics.Linecast(transform.position, -transform.up, out RaycastHit hitBottom))
        {
            desiredPosition = hitBottom.point + Vector3.up * 4.0F;
        }

        if (Physics.Linecast(transform.position, -transform.forward, out RaycastHit hitBack))
        {
            desiredPosition = hitBack.point + Vector3.up * 4.0F;
        }

        garageCamera.transform.position = Vector3.SmoothDamp(garageCamera.transform.position, desiredPosition, ref camPosVel, 0.5F);
        garageCamera.transform.rotation = Quaternion.Lerp(garageCamera.transform.rotation, Quaternion.Euler(cameraMatrix[1]), Time.deltaTime * 7.5F);
        garageCamera.fieldOfView = Mathf.SmoothDamp(garageCamera.fieldOfView, cameraMatrix[2].x, ref fovVel, .96F);
    }

    public void OnDrawGizmos()
    {
        if (cameraViewData.CameraViews.Length > 0)
        {
            Gizmos.color = Color.cyan;

            foreach (CameraView view in cameraViewData.CameraViews)
            {
                Gizmos.DrawWireCube(view.position, Vector3.one * 0.25f);
            }
        }
    }
}

