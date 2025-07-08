using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character
{
    public partial class StateMachine : Node
    {
        protected Dictionary<string, IState> _states = new Dictionary<string, IState>();
        private IState _currentState;
        public string CurrentStateName { get; private set; }

        public void AddState(string name, IState state)
        {
            _states[name] = state;
            if (state is Node stateNode && stateNode.GetParent() == null)
            {
                AddChild(stateNode);
            }
        }

        public void SetInitialState(string name)
        {
            if (_states.TryGetValue(name, out IState state))
            {
                CurrentStateName = name;
                _currentState = state;
                _currentState.Enter();
            }
        }

        public void TransitionTo(string name, object data = null)
        {
            if (_currentState != null && CurrentStateName == name) return;

            if (_states.TryGetValue(name, out IState newState))
            {
                _currentState?.Exit();
                GD.Print($"Transitioning from {_currentState?.GetType().Name} to {newState.GetType().Name}");
                CurrentStateName = name;
                _currentState = newState;
                _currentState.Enter(data);
            }
            else
            {
                GD.PushWarning($"State '{name}' not found in state machine.");
            }
        }

        public void Update(double delta)
        {
            _currentState?.Update(delta);
        }
    }
}
