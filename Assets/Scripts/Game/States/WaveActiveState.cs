using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class WaveActiveState : IState
    {
        public void Enter()
        {
            Debug.Log("Enter from Wave Active State");
            Time.timeScale = 1f;

            // TODO: Play wave music

            EventBus.Publish(new GameStateChangedEvent(GameState.WaveActive));
        }

        public void Exit()
        {
            Debug.Log("Exit from Wave Active State");

            // TODO: Stop wave music
        }
    }
}
