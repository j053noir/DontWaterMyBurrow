using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Player.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Player.States
{
    public class PlayerBuildState : IState
    {
        private readonly PlayerController _player;
        private readonly Vector2Int _targetCell;
        public IState PreviousState;

        public PlayerBuildState(PlayerController player, Vector2Int targetCell, IState previousState)
        {
            _player = player;
            _targetCell = targetCell;
            PreviousState = previousState;
        }

        public void Enter()
        {
            if (_player.DebugMode) Debug.Log("Enter from Player Build State");

            EventBus.Publish(new PlayerOpenedBuilMenuEvent(_targetCell));
        }

        public void Exit()
        {
            if (_player.DebugMode) Debug.Log("Exit from Player Build State");
            EventBus.Publish(new PlayerClosedBuildMenuEvent());
        }
    }
}