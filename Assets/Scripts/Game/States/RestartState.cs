using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class RestartState : IState
    {
        private readonly GameManager _gameManager;

        public RestartState(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Enter()
        {
            if (_gameManager.DebugMode) Debug.Log("Enter from Restart State");
            Time.timeScale = 0f;

            _gameManager.ResetGame();
            EventBus.Publish(new GameStateChangedEvent(GameState.Restart));
        }

        public void Exit()
        {
            if (_gameManager.DebugMode) Debug.Log("Exit from Restart State");
            Time.timeScale = 1f;
        }
    }
}
