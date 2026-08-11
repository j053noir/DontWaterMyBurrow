using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class PauseState : IState
    {
        public void Enter()
        {
            Debug.Log("Enter from Pause State");
            Time.timeScale = 0f;

            EventBus.Publish(new GameStateChangedEvent(GameState.Pause));
        }

        public void Exit()
        {
            Debug.Log("Exit from Pause State");
            Time.timeScale = 1f;
        }
    }
}
