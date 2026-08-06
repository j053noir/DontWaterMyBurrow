using UnityEngine;

public readonly struct WaterDrainEvent
{
    public readonly Vector2Int position;
    public readonly float drainAmount;
    public readonly float drainRadius;

    public WaterDrainEvent(Vector2Int position, float drainAmount, float drainRadius)
    {
        this.position = position;
        this.drainAmount = drainAmount;
        this.drainRadius = drainRadius;
    }
}