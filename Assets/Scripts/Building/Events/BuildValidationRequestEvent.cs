using System;
using UnityEngine;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Building.Events
{
    public readonly struct BuildValidationRequestEvent
    {
        public readonly Vector2Int BuildPosition;
        public readonly StructureDataSO StructureData;
        public readonly Action<bool> Callback;

        public BuildValidationRequestEvent(Vector2Int buildPosition, StructureDataSO structureSO, Action<bool> callback)
        {
            BuildPosition = buildPosition;
            StructureData = structureSO;
            Callback = callback;
        }
    }
}