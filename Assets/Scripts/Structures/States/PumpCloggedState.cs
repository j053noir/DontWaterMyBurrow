using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Structures.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Structures.State
{
    public class PumpCloggedState : IState
    {
        private readonly WaterPumpController _waterPump;

        public PumpCloggedState(WaterPumpController waterPump)
        {
            _waterPump = waterPump;
        }

        public void Enter()
        {
            if (_waterPump.DebugMode) Debug.Log("Enter from Pump Clogged State");
            EventBus.Publish(new PumpCloggedStateChangedEvent(_waterPump.gameObject, true));

            // TODO: Stop water pump particles
        }

        public void Exit()
        {
            if (_waterPump.DebugMode) Debug.Log("Exit from Pump Clogged State");

            // TODO: Start water pump particles
        }
    }
}