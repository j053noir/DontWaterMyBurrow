using UnityEngine;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Building.Events
{
    public class BuildValidationRequestEvent
    {
        public readonly Vector2Int BuildPosition;
        public readonly StructureDataSO StructureData;
        public bool IsValid;
        public BuildValidationRequestEvent(Vector2Int buildPosition, StructureDataSO structureSO)
        {
            BuildPosition = buildPosition;
            StructureData = structureSO;
            IsValid = true;
        }

        public void Invalidate()
        {
            IsValid = false;
        }
    }
}