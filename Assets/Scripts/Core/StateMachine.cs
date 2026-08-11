using DontWaterMyBurrow.Core.Interfaces;

namespace DontWaterMyBurrow.Core
{
    public class StateMachine
    {
        private IState _currentState;

        public void ChangeState(IState newState)
        {
            if (_currentState == newState) return;

            // Exit current state
            _currentState?.Exit();

            // Change to new state
            _currentState = newState;

            // Execute new state
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void FixedUpdate()
        {
            _currentState?.FixedUpdate();
        }
    }
}