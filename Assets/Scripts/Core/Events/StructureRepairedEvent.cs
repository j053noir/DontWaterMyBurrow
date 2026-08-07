using UnityEngine;

public readonly struct StructureRepairedEvent
{
    public readonly StructureType StructureType;
    public readonly GameObject Structure;

    public StructureRepairedEvent(StructureType type, GameObject structure)
    {
        StructureType = type;
        Structure = structure;
    }
}