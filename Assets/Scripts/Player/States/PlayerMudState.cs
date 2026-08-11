using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Player.States
{
    public class PlayerMudState : IState
    {
        private readonly PlayerController _player;
        private readonly float _moveSpeed = 2.5f;

        public PlayerMudState(PlayerController player, float moveSpeed)
        {
            _player = player;
            _moveSpeed = moveSpeed;
        }

        public void Enter()
        {
            if (_player.DebugMode) Debug.Log("Enter from Player Mud State");

            _player.SetMoveSpeed(_moveSpeed);
        }

        public void Exit()
        {
            if (_player.DebugMode) Debug.Log("Exit from Player Mud State");
        }
    }
}