using UnityEngine;

public enum StructureType
{
    WoodChannel,
    SandBag,
    WaterPump
}

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct StructureAttackedEvent
    {
        public readonly StructureType StructureType;
        public readonly Vector2Int Position;
        public readonly GameObject StructureInstance;
        public readonly int DamageTaken;

        public StructureAttackedEvent(StructureType structureType, Vector2Int position, GameObject structureInstance, int damageTaken)
        {
            StructureType = structureType;
            Position = position;
            StructureInstance = structureInstance;
            DamageTaken = damageTaken;
        }
    }
}