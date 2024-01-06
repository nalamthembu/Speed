public interface BaseState
{
    public abstract void EnterState(CharacterStateMachine stateMachine);
    public abstract void UpdateState(CharacterStateMachine stateMachine);
    public abstract void ExitState(CharacterStateMachine stateMachine);
    public abstract void CheckState(CharacterStateMachine stateMachine);
}