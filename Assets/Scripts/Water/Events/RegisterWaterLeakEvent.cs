using UnityEngine;

namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct RegisterWaterLeakEvent
    {
        public readonly Vector2Int Position { get; }

        public RegisterWaterLeakEvent(Vector2Int position)
        {
            Position = position;
        }
    }
}