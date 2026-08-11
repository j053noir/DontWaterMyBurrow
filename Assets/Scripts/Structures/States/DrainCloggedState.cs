using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Water.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Structures.State
{
    public class DrainCloggedState : IState
    {
        private readonly DrainController _drain;

        public DrainCloggedState(DrainController drain)
        {
            _drain = drain;
        }

        public void Enter()
        {
            if (_drain.DebugMode) Debug.Log("Enter from Drain Clogged State");
            EventBus.Publish(new RemoveWaterDrainEvent(_drain.Position));

            // Add event to alert user to clean the drain
        }

        public void Exit()
        {
            if (_drain.DebugMode) Debug.Log("Exit from Drain Clogged State");

            // Remove event to alert user to clean the drain
        }

        public void Update()
        {
            if (_drain.DebugMode) Debug.Log("Update from Drain Clogged State");
        }
    }
}