using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct PumpCloggedStateChangedEvent
    {
        public readonly GameObject Pump;
        public readonly bool IsClogged;

        public PumpCloggedStateChangedEvent(GameObject pump, bool isClogged)
        {
            Pump = pump;
            IsClogged = isClogged;
        }
    }
}