namespace Game.States.Interfaces
{
    public interface IBallStateMachine
    {
        void Enter<TState>() where TState : class, IBallState;
        void AddState<TState>(IBallState state) where TState : IBallState;
        bool CompareStateWithActive<TState>() where TState : class, IBallState;
    }
}