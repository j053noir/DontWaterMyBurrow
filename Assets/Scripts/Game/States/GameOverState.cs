using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class GameOverState : IState
    {
        public void Enter()
        {
            Debug.Log("Enter from Game Over State");
            Time.timeScale = 0f;

            EventBus.Publish(new GameStateChangedEvent(GameState.GameOver));
        }

        public void Exit()
        {
            Debug.Log("Exit from Game Over State");
            Time.timeScale = 1f;
        }
    }
}
