using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Player.States
{
    public class PlayerNormalState : IState
    {
        private readonly PlayerController _player;
        private readonly float _moveSpeed = 5f;

        public PlayerNormalState(PlayerController player, float moveSpeed)
        {
            _player = player;
            _moveSpeed = moveSpeed;
        }

        public void Enter()
        {
            if (_player.DebugMode) Debug.Log("Enter from Player Normal State");

            _player.SetMoveSpeed(_moveSpeed);
        }

        public void Exit()
        {
            if (_player.DebugMode) Debug.Log("Exit from Player Normal State");
        }
    }
}