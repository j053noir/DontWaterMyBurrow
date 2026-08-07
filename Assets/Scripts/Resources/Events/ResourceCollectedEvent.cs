using UnityEngine;

namespace DontWaterMyBurrow.Resources.Events
{
    public readonly struct ResourceCollectedEvent
    {
        public readonly Vector2Int Position;
        public readonly ResourceType ResourceType;
        public readonly int Quantity;

        public ResourceCollectedEvent(Vector2Int position, ResourceType resourceType, int quantity = 1)
        {
            Position = position;
            ResourceType = resourceType;
            Quantity = quantity;
        }
    }
}