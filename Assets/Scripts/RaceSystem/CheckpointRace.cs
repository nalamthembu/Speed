using System;
using UnityEngine;

public class CheckpointRace : Race
{
    [SerializeField] float m_StartingCheckpointTime;
    [SerializeField] CheckpointObject[] m_Checkpoints;

    private float m_TimeRemaining = 0;
    private int m_CollectedCheckpoints;

    public static event Action OnOutOfTime;
    public static event Action<int, int> OnCheckpointUpdate;

    public static CheckpointRace Instance;

    [ContextMenu("Debug/Initialise Race")]
    public override void InitialiseRace()
    {
        base.InitialiseRace();

        Debug.Log("Initialising Race");

        m_TimeRemaining = m_StartingCheckpointTime;

        if (Player.Instance.Vehicle)
        {
            Player.Instance.Vehicle.transform.position = m_StartingGrid.GetStartPosition(0);
            Player.Instance.Vehicle.transform.forward = m_StartingGrid.StartGridCentre.forward;
        }

        m_StartCountdown = true;
        m_RaceInitialised = true;

        //Make sure all the checkpoints are triggers
        foreach (var checkpoint in m_Checkpoints)
        {
            if (checkpoint.TryGetComponent<Collider>(out var collider) && !collider.isTrigger)
            {
                collider.isTrigger = true;
            }
        }

        OnCheckpointUpdate?.Invoke(m_CollectedCheckpoints, m_Checkpoints.Length);

        if (CheckpointRaceUI.Instance)
            CheckpointRaceUI.Instance.Show();
    }

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    public float GetRemainingTime() => m_TimeRemaining;

    protected override void OnDisable()
    {
        base.OnDisable();
        CheckpointObject.OnPlayerPastCheckpoint -= OnCheckpointCollected;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckpointObject.OnPlayerPastCheckpoint += OnCheckpointCollected;
    }

    private void OnCheckpointCollected(float additionalTime)
    {
        m_TimeRemaining += additionalTime;
        m_CollectedCheckpoints++;
        OnCheckpointUpdate?.Invoke(m_CollectedCheckpoints, m_Checkpoints.Length);
    }

    protected override void OnMetLossCondition()
    {
        base.OnMetLossCondition();

        m_RaceFinished = true;

        OnOutOfTime?.Invoke();

        Debug.Log("Player lost the race");

        // TODO : Show loss screen

        // TODO : Slow down time?

        // TODO : Stop the car
    }

    protected override void OnMetWinConditions()
    {
        base.OnMetWinConditions();

        m_RaceFinished = true;

        Debug.Log("Player won the race");

        // TODO : Show Win Screen

        // TODO : Slow down time?

        // TODO : Let AI take over
    }

    protected override void HandleTimers()
    {
        base.HandleTimers();

        m_TimeElapsed += Time.deltaTime;

        m_TimeRemaining -= Time.deltaTime;
    }

    protected override void Update()
    {
        if (m_GamePaused)
            return;

        base.Update();

        if (m_RaceStarted && !m_RaceFinished)
        {
            //  We ran out of time.
            if (m_TimeRemaining <= 0)
            {
                OnMetLossCondition();
                return;
            }
            else
            {
                // If there is still time left and we've collected all the checkpoints.
                if (m_CollectedCheckpoints >= m_Checkpoints.Length)
                {
                    OnMetWinConditions();
                    return;
                }
            }

            HandleTimers();
        }
    }
}