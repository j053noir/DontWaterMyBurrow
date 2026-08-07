using UnityEngine;

public readonly struct RemoveWaterDrainEvent
{
    public readonly Vector2Int Position;

    public RemoveWaterDrainEvent(Vector2Int position)
    {
        Position = position;
    }
}