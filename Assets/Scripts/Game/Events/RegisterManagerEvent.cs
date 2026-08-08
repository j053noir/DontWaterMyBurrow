using System;

namespace DontWaterMyBurrow.Game.Events
{
    public readonly struct RegisterManagerEvent
    {
        public readonly Type ManagerType;

        public RegisterManagerEvent(Type managerType)
        {
            ManagerType = managerType;
        }
    }
}