using UnityEngine;

public readonly struct WaterDrainEvent
{
    public readonly Vector2Int Position;
    public readonly float DrainAmount;
    public readonly int DrainRadius;

    public WaterDrainEvent(Vector2Int position, float drainAmount, int drainRadius)
    {
        Position = position;
        DrainAmount = drainAmount;
        DrainRadius = drainRadius;
    }
}