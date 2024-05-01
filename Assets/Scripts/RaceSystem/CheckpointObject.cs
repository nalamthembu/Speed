using System;
using UnityEngine;

public class CheckpointObject : MonoBehaviour
{
    [SerializeField] float m_AdditionalTime;

    public static event Action<float> OnPlayerPastCheckpoint;

    private bool m_Collected = false;

    public float GetAdditionalTime() => m_AdditionalTime;

    private void OnDrawGizmos()
    {
        Gizmos.color = m_Collected ? Color.green : Color.white;

        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5F, Vector3.one);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player.Instance && other.gameObject == Player.Instance.Vehicle.gameObject)
        {
            OnPlayerPastCheckpoint?.Invoke(GetAdditionalTime());
            m_Collected = true;
            gameObject.SetActive(false);
        }
    }
}