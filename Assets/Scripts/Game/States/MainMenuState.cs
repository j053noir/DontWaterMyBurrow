using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game.States
{
    public class MainMenuState : IState
    {
        public void Enter()
        {
            // Enter MainMenuState
            Time.timeScale = 0f;

            // TODO: Play main menu music in loop

            EventBus.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }

        public void Exit()
        {
            Time.timeScale = 1f;

            // TODO: Stop main menu music
        }
    }
}
