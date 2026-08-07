using UnityEngine;

public readonly struct RegisterWaterDrainEvent
{
    public readonly Vector2Int Position;
    public readonly int DrainRadius;
    public readonly int DrainAmount;

    public RegisterWaterDrainEvent(Vector2Int position, int drainRadius, int drainAmount = 1)
    {
        Position = position;
        DrainRadius = drainRadius;
        DrainAmount = drainAmount;
    }
}