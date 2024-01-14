using System;
using System.Collections.Generic;
using Game.States.Interfaces;

namespace Game.States
{
    public class BallStateMachine : IBallStateMachine
    {
        private Dictionary<Type, IBallState> _states = new();
        
        private IBallState _activeState;

        public void Enter<TState>() where TState : class, IBallState
        {
            IBallState state = ChangeState<TState>();
            state.Enter();
        }

        public void AddState<TState>(IBallState state) where TState : IBallState
        {
            _states.Add(typeof(TState), state);;
        }

        public bool CompareStateWithActive<TState>() where TState : class, IBallState
        {
            TState state = GetState<TState>();
            
            return state.Equals(_activeState);
        }

        private TState ChangeState<TState>() where TState : class, IBallState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IBallState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}