using System.Collections.Generic;
using UnityEngine;

namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct WaterFlowUpdatedEvent
    {
        public readonly Dictionary<Vector2Int, Vector2Int> CellsFlow;

        public WaterFlowUpdatedEvent(Dictionary<Vector2Int, Vector2Int> cellsFlow)
        {
            CellsFlow = cellsFlow;
        }
    }
}