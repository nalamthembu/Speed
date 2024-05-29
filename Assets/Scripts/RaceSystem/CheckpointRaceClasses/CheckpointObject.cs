using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointObject : MonoBehaviour
{
    [SerializeField] float m_AdditionalTime;

    [SerializeField] GameObject m_PositionIndicator;

    public static event Action<float> OnPlayerPastCheckpoint;

    private bool m_Collected = false;

    public float GetAdditionalTime() => m_AdditionalTime;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        m_Collected = false;
        m_PositionIndicator.SetActive(true);
        gameObject.SetActive(false);

        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = m_Collected ? Color.green : Color.white;

        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5F, Vector3.one);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player tried to make U turn and re collect this checkpoint when its already collected.
        if (m_Collected)
            return;

        // Check if previous checkpoint was collected.
        int index = 0;
        foreach (var checkpoint in CheckpointRace.Instance.GetAllCheckpoints())
        {
            if (checkpoint == this)
            {
                if (index == 0)
                    continue;

                CheckpointObject prevCheckpoint = CheckpointRace.Instance.GetCheckpointObject(index - 1);

                if (prevCheckpoint != null && !prevCheckpoint.m_Collected)
                {
                    Debug.Log("Cannot collect a checkpoint further from the next.");

                    return;
                }
            }
            index++;
        }

        if (Player.Instance && Player.Instance.Vehicle && other.gameObject == Player.Instance.Vehicle.gameObject)
        {
            OnPlayerPastCheckpoint?.Invoke(GetAdditionalTime());
            m_Collected = true;
            m_PositionIndicator.SetActive(false);
        }
    }
}