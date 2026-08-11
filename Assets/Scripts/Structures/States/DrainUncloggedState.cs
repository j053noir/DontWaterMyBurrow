using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Water.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Structures.State
{
    public class DrainUncloggedState : IState, IUpdateableState
    {
        private readonly DrainController _drain;

        public DrainUncloggedState(DrainController drain)
        {
            _drain = drain;
        }

        public void Enter()
        {
            if (_drain.DebugMode) Debug.Log("Enter from Drain Unclogged State");
            EventBus.Publish(new RegisterWaterDrainEvent(_drain.Position, _drain.DrainRadius, _drain.DrainAmount));

            // TODO: Start water drain animation
            // TODO: Play water drain SFX
        }

        public void Exit()
        {
            if (_drain.DebugMode) Debug.Log("Exit from Drain Unclogged State");
            EventBus.Publish(new RemoveWaterDrainEvent(_drain.Position));

            // TODO: Stop water drain animation
            // TODO: Stop water drain SFX
        }

        public void Update()
        {
            Debug.Log("Update from Drain Unclogged State");
        }
    }
}