using System;

namespace DontWaterMyBurrow.Game.Events
{
    public readonly struct ManagerReadyEvent
    {
        public readonly Type ManagerType;


        public ManagerReadyEvent(Type managerType)
        {
            ManagerType = managerType;
        }
    }
}