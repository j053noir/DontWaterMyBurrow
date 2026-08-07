using UnityEngine;

namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct RemoveWaterDrainEvent
    {
        public readonly Vector2Int Position;

        public RemoveWaterDrainEvent(Vector2Int position)
        {
            Position = position;
        }
    }
}