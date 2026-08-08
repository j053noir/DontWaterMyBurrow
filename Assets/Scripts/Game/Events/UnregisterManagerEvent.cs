using System;

namespace DontWaterMyBurrow.Game.Events
{
    public struct UnregisterManagerEvent
    {
        public Type ManagerType { get; }

        public UnregisterManagerEvent(Type managerType)
        {
            ManagerType = managerType;
        }
    }
}