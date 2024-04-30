using System;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    //Game Mode Variables
    [Header("Game Mode Variables")]
    [SerializeField] protected GameObject m_PlayerPrefab;

    //Different Stages of the game
    public virtual void PrepareForGameStart() { OnGameIsAboutToStart?.Invoke(); }
    public virtual void StartGame() { OnGameHasStarted?.Invoke(); }
    public virtual bool PlayerHasMetWinConditions(bool DidPlayerWin) 
    { 
        if (DidPlayerWin) OnPlayerHasWonGame?.Invoke(); 
        else OnPlayerHasLostGame?.Invoke(); 
        return DidPlayerWin; 
    }

    public virtual void EndGame(EndGameResult endGameResult) { OnGameHasEnded?.Invoke(); }

    //Lifetime Methods
    protected virtual void Awake() { }
    protected virtual void Update() { }
    protected virtual void Start() { }

    //Events
    public static event Action OnPlayerHasWonGame;
    public static event Action OnPlayerHasLostGame;
    public static event Action OnGameIsAboutToStart;
    public static event Action OnGameHasStarted;
    public static event Action OnGameHasEnded;
}

public enum EndGameResult
{
    PlayerWon,
    PlayerLost,
    PlayerQuit
};