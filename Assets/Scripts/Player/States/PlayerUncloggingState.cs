using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;
using DontWaterMyBurrow.Structures;

namespace DontWaterMyBurrow.Player.States
{
    public class PlayerUncloggingState : IState, IUpdateableState
    {
        private readonly PlayerController _player;
        private readonly IState _previousState;
        private float _uncloggingTimer;
        private readonly float _uncloggingDuration;
        public readonly GameObject _objectToUnclog;

        public PlayerUncloggingState(PlayerController player, IState previousState, float uncloggingDuration, GameObject objectToUnclog)
        {
            _player = player;
            _previousState = previousState;
            _uncloggingDuration = uncloggingDuration;
            _objectToUnclog = objectToUnclog;
        }

        public void Enter()
        {
            if (_player.DebugMode) Debug.Log("Enter from Player Unclogging State");

            _uncloggingTimer = _uncloggingDuration;
            _player.SetMoveSpeed(0f);

            // TODO: Publish event to start unclogging (animation, sfx, particles)
        }

        public void Exit()
        {
            if (_player.DebugMode) Debug.Log("Exit from Player Unclogging State");

            // TODO: Publish event to stop unclogging (animation, sfx, particles)
        }

        public void Update()
        {
            _uncloggingTimer -= Time.deltaTime;

            if (_uncloggingTimer <= 0)
            {
                if (_objectToUnclog.TryGetComponent<WaterPumpController>(out var waterPump))
                {
                    waterPump.CleanPump();
                }
                else if (_objectToUnclog.TryGetComponent<DrainController>(out var drain))
                {
                    drain.CleanDrain();
                }

                _player.StateMachine.ChangeState(_previousState);
            }
        }
    }
}