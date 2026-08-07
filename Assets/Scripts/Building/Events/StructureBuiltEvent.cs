using UnityEngine;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Building.Events
{
    public readonly struct StructureBuiltEvent
    {
        public readonly StructureType StructureType;
        public readonly Vector2Int Position;
        public readonly GameObject StructureInstance;
        public readonly StructureDataSO StructureData;

        public StructureBuiltEvent(StructureType structureType, Vector2Int position, GameObject structureInstance, StructureDataSO structureData)
        {
            StructureType = structureType;
            Position = position;
            StructureInstance = structureInstance;
            StructureData = structureData;
        }
    }
}