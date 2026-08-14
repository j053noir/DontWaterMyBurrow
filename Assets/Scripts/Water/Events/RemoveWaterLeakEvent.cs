using UnityEngine;

namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct RemoveWaterLeakEvent
    {
        public readonly Vector2Int Position { get; }

        public RemoveWaterLeakEvent(Vector2Int position)
        {
            Position = position;
        }
    }
}