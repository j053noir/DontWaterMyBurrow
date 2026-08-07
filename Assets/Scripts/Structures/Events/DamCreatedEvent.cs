using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct DamCreatedEvent
    {
        public readonly Vector2Int Position;
        public readonly GameObject Instance;

        public DamCreatedEvent(Vector2Int position, GameObject instance)
        {
            Position = position;
            Instance = instance;
        }
    }
}