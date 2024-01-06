using UnityEngine;

public abstract class GameState
{
    public abstract void EnterState(GameStateMachine stateMachine);
    public abstract void UpdateState(GameStateMachine stateMachine);
    public abstract void CheckForStateChange(GameStateMachine stateMachine);
    public abstract void ExitState(GameStateMachine stateMachine);
}

public class GameStatePaused : GameState
{
    public override void CheckForStateChange(GameStateMachine stateMachine)
    {
        if (!GameManager.Instance.IsPaused)
        {
            if (GameManager.Instance.IsInRace)
            {
                stateMachine.SwitchState(stateMachine.gameState_Race);
            }

            if (GameManager.Instance.IsInRace == false)
            {
                stateMachine.SwitchState(stateMachine.gameState_Menu);
                //If we're not in a race theres no need to keep track of all the rigidbodies in the scene.
                if (!GameManager.Instance.IsInRace)
                    stateMachine.bodies.Clear();
            }
        }
    }

    public override void EnterState(GameStateMachine stateMachine)
    {
        //Make all rigidbodies Kinematic.
        stateMachine.StopAllRigidbodies();

        //Make sure menus can read input.
        if (FEManager.instance.IsReadingInput == false)
        {
            FEManager.instance.IsReadingInput = true;
        }
    }

    public override void ExitState(GameStateMachine stateMachine)
    {
        //Make all rigidbodies nonKinematic.
        if (stateMachine.bodies.Count > 0)
            stateMachine.ResumeAllRigidbodies();
    }

    public override void UpdateState(GameStateMachine stateMachine)
    {
        //Check if the game is unpaused.
        CheckForStateChange(stateMachine);
    }
}

public class GameStateInMenu : GameState
{
    public override void CheckForStateChange(GameStateMachine stateMachine)
    {
        if (GameManager.Instance.IsInRace)
        {
            stateMachine.SwitchState(stateMachine.gameState_Race);
        }

        return;
    }

    public override void EnterState(GameStateMachine stateMachine)
    {
        //Freeze All Rigidbodies
        stateMachine.StopAllRigidbodies();

        //Change music style to menu ( garage ) music.
        if (FEManager.instance.IsReadingInput == false)
        {
            FEManager.instance.IsReadingInput = true;
        }
    }

    public override void ExitState(GameStateMachine stateMachine)
    {
        //Change music style to racing music.
        if (FEManager.instance.IsReadingInput == true)
        {
            FEManager.instance.IsReadingInput = false;
        }
    }

    public override void UpdateState(GameStateMachine stateMachine)
    {
        CheckForStateChange(stateMachine);
    }
}

public class GameStateInRace : GameState
{
    public override void CheckForStateChange(GameStateMachine stateMachine)
    {
        if (!GameManager.Instance.IsPaused)
        {
            if (!GameManager.Instance.IsInRace)
            {
                stateMachine.SwitchState(stateMachine.gameState_Menu);
            }
        }
        else
        {
            stateMachine.SwitchState(stateMachine.gameState_Paused);
        }
    }

    public override void EnterState(GameStateMachine stateMachine)
    {
        //Make sure there is a player vehicle in the scene.
        if (Player.instance != null)
        {
            if (Player.instance.Vehicle == null)
            {
                Player.instance.InitialisePlayer();
            }
        }
        else
            Debug.LogError("There is no Player Object in the scene!");

        //Get all rigidbodies
        stateMachine.GetAllRigidbodies();

    }

    public override void ExitState(GameStateMachine stateMachine)
    {
        stateMachine.ResumeAllRigidbodies();

        //If we're not in a race theres no need to keep track of all the rigidbodies in the scene.
        if (!GameManager.Instance.IsInRace)
            stateMachine.bodies.Clear();

        return;
    }

    public override void UpdateState(GameStateMachine stateMachine)
    {
        CheckForStateChange(stateMachine);
    }
}
