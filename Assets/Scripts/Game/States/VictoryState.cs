using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class VictoryState : IState
    {
        public void Enter()
        {
            Debug.Log("Enter from Victory State");
            Time.timeScale = 0f;

            EventBus.Publish(new GameStateChangedEvent(GameState.Victory));

            // TODO: Win music
        }

        public void Exit()
        {
            Debug.Log("Exit from Victory State");
            Time.timeScale = 1f;

            // TODO: Stop win music
        }
    }
}
