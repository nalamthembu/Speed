using System;
using ThirdPersonFramework.UserInterface;
using UnityEngine;

/*
 * [PURPOSE]
 * This script is the base class
 * of every race script, it handles
 * the basic needs of every race, including
 * starting position, path caching and 
 * countdowns.
 */

public class Race : MonoBehaviour
{
    [SerializeField] protected StartingGrid m_StartingGrid;

    [SerializeField] float m_CountdownTime = 3;

    protected bool m_RaceInitialised = false;
    protected bool m_RaceStarted = false;
    protected float m_CountdownTimer;
    protected bool m_RaceFinished = false;
    protected float m_TimeElapsed = 0;
    protected bool m_StartCountdown;
    protected bool m_GamePaused;

    public float GetTimeElasped() => m_TimeElapsed;

    protected virtual void OnDrawGizmos()
    {
        if (m_StartingGrid != null && m_StartingGrid.DebugMode)
            m_StartingGrid.OnDrawGizmos();
    }

    protected virtual void OnDisable()
    {
        PauseMenu.OnPauseMenuOpened -= OnGamePaused;
        PauseMenu.OnPauseMenuClosed -= OnGameResumed;
    }
    protected virtual void OnEnable()
    {
        PauseMenu.OnPauseMenuOpened += OnGamePaused;
        PauseMenu.OnPauseMenuClosed += OnGameResumed;
    }

    private void OnGameResumed() => m_GamePaused = false;
    private void OnGamePaused() => m_GamePaused = true;

    public virtual void InitialiseRace() => m_CountdownTimer = m_CountdownTime;
    

    protected virtual void Update()
    {
        if (m_RaceInitialised && !m_RaceStarted && m_StartCountdown)
        {
            m_CountdownTimer -= Time.deltaTime;

            if (m_CountdownTimer <= 0)
                m_RaceStarted = true;
        }
    }

    protected virtual void Awake() { }
    protected virtual void OnMetWinConditions() { }
    protected virtual void OnMetLossCondition() { }
    protected virtual void RestartRace() { }
    protected virtual void HandleTimers() { }
}

/*
 * [PURPOSE]
 * This handles the starting position
 * of all the racers.
 */

[System.Serializable]
public class StartingGrid
{
    [SerializeField] Transform m_StartingGridCentreTransform;
    [SerializeField] Vector3[] m_StartingPositions;
    [SerializeField] bool m_DebugMode;

    public bool DebugMode { get { return m_DebugMode; } }

    public Transform StartGridCentre => m_StartingGridCentreTransform;

    public int NumberOfRacers => m_StartingPositions.Length;

    public Vector3 GetStartPosition(int position) => m_StartingPositions[position];

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (m_StartingPositions != null && m_StartingPositions.Length >0 && m_DebugMode)
        {
            foreach(var position in m_StartingPositions)
            {
                Gizmos.DrawWireCube(position, new(1, 0.5F, 1));
            }
        }
    }
 }
