using System;
using UnityEngine;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct RepairStructureRequestEvent
    {
        public readonly Vector2Int Position;
        public readonly StructureDataSO StructureSO;
        public readonly Action<bool> Callback;

        public RepairStructureRequestEvent(Vector2Int position, StructureDataSO structureSO, Action<bool> callback)
        {
            Position = position;
            StructureSO = structureSO;
            Callback = callback;
        }
    }
}