using UnityEngine;

public readonly struct StructureDestroyedEvent
{
    public readonly Vector2Int Position;
    public readonly GameObject StructureInstance;

    public StructureDestroyedEvent(Vector2Int position, GameObject structureInstance)
    {
        Position = position;
        StructureInstance = structureInstance;
    }
}