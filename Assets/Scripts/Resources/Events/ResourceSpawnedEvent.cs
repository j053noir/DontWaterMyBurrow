using UnityEngine;

namespace DontWaterMyBurrow.Resources.Events
{
    public readonly struct ResourceSpawnedEvent
    {
        public readonly GameObject Instance;
        public readonly Vector2Int Position;
        public readonly ResourceType ResourceType;

        public ResourceSpawnedEvent(GameObject instance, Vector2Int position, ResourceType resourceType)
        {
            Instance = instance;
            Position = position;
            ResourceType = resourceType;
        }
    }
}