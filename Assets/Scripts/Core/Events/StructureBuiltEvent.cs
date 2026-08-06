using UnityEngine;

public readonly struct StructureBuiltEvent
{
    public readonly StructureType StructureType;
    public readonly Vector2Int Position;
    public readonly GameObject StructureInstance;

    public StructureBuiltEvent(StructureType structureType, Vector2Int position, GameObject structureInstance)
    {
        StructureType = structureType;
        Position = position;
        StructureInstance = structureInstance;
    }
}