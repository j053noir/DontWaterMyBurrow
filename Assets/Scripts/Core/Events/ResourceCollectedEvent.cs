using UnityEngine;

public readonly struct ResourceCollectedEvent
{
    public readonly Vector2Int Position;
    public readonly ResourceType ResourceType;
    public readonly int Quantity;

    public ResourceCollectedEvent(Vector2Int position, ResourceType resourceType, int quantity)
    {
        Position = position;
        ResourceType = resourceType;
        Quantity = quantity;
    }
}