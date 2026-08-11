using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Player.States
{
    public class PlayerDisabledState : IState
    {
        private readonly PlayerController _player;

        public PlayerDisabledState(PlayerController player)
        {
            _player = player;
        }

        public void Enter()
        {
            Debug.Log("Enter from Player Disabled State");
            _player.SetMoveSpeed(0f);
        }

        public void Exit()
        {
            Debug.Log("Exit from Player Disabled State");
        }
    }
}