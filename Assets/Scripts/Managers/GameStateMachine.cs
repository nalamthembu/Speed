using System.Collections.Generic;
using UnityEngine;
using System;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance;

    [SerializeField] GameObject m_PlayerPrefab;
    [SerializeField] GameObject m_GameplayCameraPrefab;

    public GameStateInMenu gameState_Menu = new();
    public GameStateInRace gameState_Race = new();
    public GameStatePaused gameState_Paused = new();
    public GameState gameState_Current;

    public GameObject PlayerPrefab { get { return m_PlayerPrefab; } }
    public GameObject GameplayCameraPrefab { get { return m_GameplayCameraPrefab; } }

    private GameStateEnum m_GameStateEnum;

    public event Action OnIsRacing, OnIsInMenu, OnIsPaused;

    //Every rigidbody in the scene is kept here to "pause" when the game is in the paused state.
    public List<Body> bodies;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
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

            bodies.Add(new(b[i], b[i].gameObject.name));
        }
    }

    public void StopAllRigidbodies()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            if (bodies[i] == null && bodies[i].TryPause() == false)
            {
                Debug.LogWarning("Rigidbody was null, removing from list.");
                bodies.Remove(bodies[i]);
            }
        }
    }
    
    public void ResumeAllRigidbodies()
    {
        for (int i = 0; i < bodies.Count; i++)
            if (bodies[i] == null && !bodies[i].TryResume())
                bodies.Remove(bodies[i]);
    }
    #endregion

}

[Serializable]
public class Body
{
    [SerializeField] string Name; //This is so it shows up in the editor.
    private Rigidbody rigidbody;
    private Vector3 previousVelocity;

    public Body(Rigidbody rigidbody, string Name = "Body")
    {
        this.Name = Name;
        this.rigidbody = rigidbody;
    }

    public bool TryPause()
    {
        if (rigidbody is null)
            return false;

        previousVelocity = rigidbody.velocity;
        rigidbody.isKinematic = true;

        Debug.Log(previousVelocity);

        return true;
    }

    public bool TryResume()
    {
        if (rigidbody is null)
            return false;

        rigidbody.isKinematic = false;
        rigidbody.velocity = previousVelocity;

        Debug.Log(rigidbody.velocity + " unp");

        return true;

    }
}

public enum GameStateEnum
{
    IsRacing,
    IsInMenu,
    IsPaused,
}