using UnityEngine;
using VehiclePhysics;

public class VehicleVisuals : MonoBehaviour
{

    [Header("Light Settings")]
    [SerializeField] float minTailIntensity;
    [SerializeField] float maxTailIntensity;
    [SerializeField] float headLightsDim, headLightsBright;

    private VPStandardInput input;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        input = GetComponent<VPStandardInput>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        if (input != null)
        {
            HandleHeadLights();
            HandleTailLights();
        }
        else
        {
            Debug.LogError("Vehicle Input is NULL!");
            enabled = false;
        }
    }

    private void HandleTailLights()
    {
        if (input != null && meshRenderer != null)
        {
            meshRenderer.material.SetFloat("_Braking", Mathf.Lerp(minTailIntensity, maxTailIntensity, input.externalBrake * 10));
        }
    }

    private void HandleHeadLights()
    {
        //TODO : HANDLE HEAD LIGHTS
    }
}
