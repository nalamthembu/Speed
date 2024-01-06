using UnityEngine;
using static CharacterStates;

public class CharacterStateMachine : MonoBehaviour
{
    public DrivingForward m_DrivingStateFwd;
    public DrivingBackward m_DrivingStateBwd;
    public DrivingIdle m_DrivingStateIdle;
    public BaseState m_CurrentState;

    private Animator m_Animator;

    public Animator Animator { get { return m_Animator; } }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_DrivingStateFwd = new();
        m_DrivingStateBwd = new();
        m_DrivingStateIdle = new();

        SwitchState(m_DrivingStateIdle);
    }

    public void SwitchState(BaseState newState)
    {
        if (m_CurrentState !=null)
            m_CurrentState.ExitState(this);
        m_CurrentState = newState;
        m_CurrentState.EnterState(this);
    }

    public void Update()
    {
        if (m_CurrentState is not null)
        {
            m_CurrentState.UpdateState(this);
            m_CurrentState.CheckState(this);
        }
    }
}