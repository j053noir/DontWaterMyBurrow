using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Water.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Structures.State
{
    public class PumpOperationalState : IState, IUpdateableState
    {
        private readonly WaterPumpController _waterPump;

        public PumpOperationalState(WaterPumpController waterPump)
        {
            _waterPump = waterPump;
        }

        public void Enter()
        {
            Debug.Log("Enter from Pump Operational State");
            EventBus.Publish(new PumpCloggedStateChangedEvent(_waterPump.gameObject, false));

            // TODO: Show drain particles
        }

        public void Exit()
        {
            Debug.Log("Exit from Pump Operational State");

            // TODO: Hide drain particles
        }

        public void Update()
        {
            Debug.Log("Update from Pump Operational State");

            var vector = _waterPump.gameObject.transform.position;
            var position = new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
            EventBus.Publish(new WaterDrainEvent(position, _waterPump.DrainRate * Time.deltaTime, _waterPump.DrainRadius));
        }
    }
}