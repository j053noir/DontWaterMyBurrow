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
            Debug.Log("Enter from Main Menu State");
            Time.timeScale = 0f;

            // TODO: Play main menu music in loop

            EventBus.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }

        public void Exit()
        {
            Debug.Log("Exit from Main Menu State");
            Time.timeScale = 1f;

            // TODO: Stop main menu music
        }
    }
}
