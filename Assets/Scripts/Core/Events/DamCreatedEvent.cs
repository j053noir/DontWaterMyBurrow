using UnityEngine;

public readonly struct DamCreatedEvent
{
    public readonly Vector2Int[] OccupiedCells;
    public readonly GameObject DamInstance;

    public DamCreatedEvent(Vector2Int[] occupiedCells, GameObject logInstance)
    {
        OccupiedCells = occupiedCells;
        DamInstance = logInstance;
    }
}