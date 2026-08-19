using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct DamDestroyedEvent
    {
        public readonly Vector2Int[] OccupiedCells;
        public readonly GameObject Instance;

        public DamDestroyedEvent(Vector2Int[] occupiedCells, GameObject instance)
        {
            OccupiedCells = occupiedCells;
            Instance = instance;
        }
    }
}