using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine instance;

    public GameStateInMenu gameState_Menu = new();
    public GameStateInRace gameState_Race = new();
    public GameStatePaused gameState_Paused = new();
    public GameState gameState_Current;

    private GameStateEnum m_GameStateEnum;

    public event Action OnIsRacing, OnIsInMenu, OnIsPaused;

    //Every rigidbody in the scene is kept here to "pause" when the game is in the paused state.
    public List<Body> bodies;
    public List<Vector3> velocities;

    private void Awake()
    {
        if (instance is null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SwitchState(gameState_Menu);
    }

    private void Update()
    {
        if (gameState_Current != null)
            gameState_Current.UpdateState(this);
    }

    public void SwitchState(GameState newState)
    {
        print("GameState : " + newState);

        if (gameState_Current != null)
            gameState_Current.ExitState(this);

        gameState_Current = newState;

        gameState_Current.EnterState(this);

        switch (gameState_Current)
        {
            case GameStateInMenu:
                m_GameStateEnum = GameStateEnum.IsInMenu;
                OnIsInMenu?.Invoke();
                break;

            case GameStatePaused:
                m_GameStateEnum = GameStateEnum.IsPaused;
                OnIsPaused?.Invoke();
                break;

            case GameStateInRace:
                m_GameStateEnum = GameStateEnum.IsRacing;

                if (Player.instance != null)
                    Player.instance.InitialisePlayer();

                OnIsRacing?.Invoke();
                break;
        }
    }

    public GameStateEnum GetCurrentGameState() => m_GameStateEnum;

    #region RIGIDBODY_MANAGEMENT
    public void GetAllRigidbodies()
    {
        Rigidbody[] b = FindObjectsOfType<Rigidbody>();

        bodies = new();

        velocities = new();

        for (int i = 0; i < b.Length; i++)
        {
            //If the rigidbody was previously kinematic don't change that when we unpause.
            if (b[i].isKinematic)
                bodies.Add(new Body(b[i], b[i].isKinematic));
            else
                bodies.Add(new Body(b[i], false));

            velocities.Add(b[i].velocity);
        }
    }

    public void StopAllRigidbodies()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            //If the rigidbody was previously kinematic don't change that when we unpause.
            if (bodies[i].wasPreviouslyKinematic)
                continue;

            velocities[i] = bodies[i].rigidbody.velocity;

            bodies[i].rigidbody.isKinematic = true;
        }
    }

    public void ResumeAllRigidbodies()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            //If the rigidbody was previously kinematic don't change that when we unpause.
            if (bodies[i].wasPreviouslyKinematic)
                continue;

            bodies[i].rigidbody.isKinematic = false;
            bodies[i].rigidbody.velocity = velocities[i];
        }
    }
    #endregion

}

[System.Serializable]
public struct Body
{
    public Rigidbody rigidbody;
    public bool wasPreviouslyKinematic;

    public Body(Rigidbody rigidbody, bool wasPreviouslyKinematic, Vector3 previousVelocity = default)
    {
        this.rigidbody = rigidbody;
        this.wasPreviouslyKinematic = wasPreviouslyKinematic;
    }
}

public enum GameStateEnum
{
    IsRacing,
    IsInMenu,
    IsPaused,
}