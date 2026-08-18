using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Resources.Events
{
    public readonly struct OutOfResourcesEvent
    {
        public readonly StructureDataSO StructureData;

        public OutOfResourcesEvent(StructureDataSO structureData)
        {
            StructureData = structureData;
        }
    }
}
