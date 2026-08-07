using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct DamDestroyedEvent
    {
        public readonly Vector2Int Position;
        public readonly GameObject Instance;

        public DamDestroyedEvent(Vector2Int position, GameObject instance)
        {
            Position = position;
            Instance = instance;
        }
    }
}