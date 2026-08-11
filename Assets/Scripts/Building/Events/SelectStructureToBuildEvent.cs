using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Building.Events
{
    public readonly struct SelectStructureToBuildEvent
    {
        public readonly StructureDataSO StructureData;

        public SelectStructureToBuildEvent(StructureDataSO structureData)
        {
            StructureData = structureData;
        }
    }
}