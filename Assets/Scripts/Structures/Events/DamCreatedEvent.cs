using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct DamCreatedEvent
    {
        public readonly Vector2Int[] OccupiedCells;
        public readonly GameObject Instance;

        public DamCreatedEvent(Vector2Int[] occupiedCells, GameObject instance)
        {
            OccupiedCells = occupiedCells;
            Instance = instance;
        }
    }
}