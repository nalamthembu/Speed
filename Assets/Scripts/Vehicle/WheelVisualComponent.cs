using UnityEngine;
using VehiclePhysics;

public class WheelVisualComponent : MonoBehaviour
{
    [SerializeField] TyreScriptable m_TyreData;
    [SerializeField] RimScriptable m_RimData;
    [SerializeField] Transform m_WheelMeshParent;
    [SerializeField] WheelPosition m_WheelPosition;
    [SerializeField] WheelSide m_WheelSide;
    VPWheelCollider m_WheelCollider;

    public RimScriptable RimData { get { return m_RimData; } set { m_RimData = value; } }

    private GameObject m_SpawnedTyre;
    private GameObject m_SpawnedRim;
    private GameObject m_EntireWheel; //The Parent of the two.

    public GameObject SpawnedWheel { get { return m_SpawnedTyre; } }

    private void Awake()
    {
        SpawnWheelAndRim();
    }

    [ContextMenu("Spawn Wheel (DEBUG)")]
    public void SpawnWheelAndRim()
    {
        Vector3 wheelDirection = Vector3.zero;

        if (m_SpawnedTyre)
            Destroy(m_SpawnedTyre);

        switch (m_WheelSide)
        {
            case WheelSide.Left:

                switch (m_WheelPosition)
                {
                    case WheelPosition.Back:
                        m_EntireWheel = new("WheelBL");
                        break;

                    case WheelPosition.Front:
                        m_EntireWheel = new("WheelFL");
                        break;
                }

                wheelDirection = Vector3.up * 180;

                break;


            case WheelSide.Right:

                switch (m_WheelPosition)
                {
                    case WheelPosition.Back:
                        m_EntireWheel = new("WheelBR");
                        break;

                    case WheelPosition.Front:
                        m_EntireWheel = new("WheelFR");
                        break;
                }

                wheelDirection = Vector3.up * 0;

                break;
        }

        m_EntireWheel.transform.parent = m_WheelMeshParent;
        m_SpawnedTyre = Instantiate(m_TyreData.mesh, m_EntireWheel.transform.position, Quaternion.Euler(wheelDirection), m_EntireWheel.transform);
        m_SpawnedRim = Instantiate(m_RimData.mesh, m_SpawnedTyre.transform.position, m_SpawnedTyre.transform.rotation, m_SpawnedTyre.transform);

        if (m_WheelCollider == null)
        {
            m_WheelCollider = GetComponent<VPWheelCollider>();
            m_WheelCollider.wheelTransform = m_EntireWheel.transform;
        }

        ResizeWheelCollider();
    }
    private void ResizeWheelCollider()
    {
        //Change : Of course this object wouldn't have a mesh renderer on it!
        //You need to get it from the tyre *Mesh* itself
        if (m_TyreData.mesh.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            Bounds b = meshRenderer.bounds;

            if (m_WheelCollider == null)
            {
                Debug.LogError("Wheel Collider is null!");
                return;
            }

            m_WheelCollider.radius = b.size.z / 2;
        }
        else
        {
            Debug.LogError("Could not find MeshRenderer");
        }
    }
}

public enum WheelSide
{
    Left,
    Right
};

public enum WheelPosition
{
    Front,
    Back
};