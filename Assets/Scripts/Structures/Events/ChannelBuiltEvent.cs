using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct ChannelBuiltEvent
    {
        public readonly Vector2Int Position;
        public readonly Vector2Int Direction;

        public ChannelBuiltEvent(Vector2Int position, Vector2Int direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}