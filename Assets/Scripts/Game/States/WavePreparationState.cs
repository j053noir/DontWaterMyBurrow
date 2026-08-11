using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class WavePreparationState : IState
    {
        private readonly GameManager _gameManager;

        public WavePreparationState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Enter()
        {
            if (_gameManager.DebugMode) Debug.Log("Enter from Wave Preparation State");
            Time.timeScale = 1f;

            // TODO: Play preparation music

            EventBus.Publish(new GameStateChangedEvent(GameState.WavePreparation));
        }

        public void Exit()
        {
            if (_gameManager.DebugMode) Debug.Log("Exit from Wave Preparation State");

            // TODO: Stop preparation music
        }
    }
}
