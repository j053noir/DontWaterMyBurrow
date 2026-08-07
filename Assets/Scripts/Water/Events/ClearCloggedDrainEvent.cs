using UnityEngine;

namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct ClearCloggedDrainEvent
    {
        public readonly Vector2Int position;

        public ClearCloggedDrainEvent(Vector2Int position)
        {
            this.position = position;
        }
    }
}