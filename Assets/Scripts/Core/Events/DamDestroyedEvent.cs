using UnityEngine;

public readonly struct DamDestroyedEvent
{
    public readonly Vector2Int[] OccupiedCells;
    public readonly GameObject damInstance;

    public DamDestroyedEvent(Vector2Int[] occupiedCells, GameObject logInstance)
    {
        OccupiedCells = occupiedCells;
        damInstance = logInstance;
    }
}