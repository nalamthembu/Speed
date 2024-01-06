using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance;

    public GameStateInMenu gameState_Menu = new();
    public GameStateInRace gameState_Race = new();
    public GameStatePaused gameState_Paused = new();
    public GameState gameState_Current;

    private GameStateEnum m_GameStateEnum;

    public event Action OnIsRacing, OnIsInMenu, OnIsPaused;

    //Every rigidbody in the scene is kept here to "pause" when the game is in the paused state.
    public List<Body> bodies;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
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

                //Change: this was checking if the player was *not* null before,
                //this meant every time you unpaused the game it would spawn a new player in.

                if (Player.instance == null) 
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

        for (int i = 0; i < b.Length; i++)
        {
            if (b[i].isKinematic)
                continue;

            bodies.Add(new(b[i]));
        }
    }

    public void StopAllRigidbodies()
    {
        for (int i = 0; i < bodies.Count; i++)
            bodies[i].Pause();
    }

    public void ResumeAllRigidbodies()
    {
        for (int i = 0; i < bodies.Count; i++)
            bodies[i].Resume();
    }
    #endregion

}

[System.Serializable]
public class Body
{
    readonly private Rigidbody rigidbody;
    private Vector3 previousVelocity;

    public Body(Rigidbody rigidbody)
    {
        this.rigidbody = rigidbody;
    }

    public void Pause()
    {
        previousVelocity = rigidbody.velocity;
        rigidbody.isKinematic = true;

        Debug.Log(previousVelocity);
    }

    public void Resume()
    {
        rigidbody.isKinematic = false;
        rigidbody.velocity = previousVelocity;

        Debug.Log(rigidbody.velocity + " unp");

    }
}

public enum GameStateEnum
{
    IsRacing,
    IsInMenu,
    IsPaused,
}