using DontWaterMyBurrow.Core.Interfaces;

namespace DontWaterMyBurrow.Core
{
    public class StateMachine
    {
        private IState _currentState;

        public IState CurrentState => _currentState;

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
            if (_currentState is IUpdateableState updateableState)
            {
                updateableState.Update();
            }
        }

        public void FixedUpdate()
        {
            if (_currentState is IFixedUpdateableState fixedUpdateableState)
            {
                fixedUpdateableState.FixedUpdate();
            }
        }
    }
}