using System.Collections.Generic;
using UnityEngine;

namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct WaterGridUpdateEvent
    {
        public readonly IReadOnlyDictionary<Vector2Int, float> WaterGrid;

        public WaterGridUpdateEvent(IReadOnlyDictionary<Vector2Int, float> waterGrid)
        {
            WaterGrid = waterGrid;
        }
    }
}