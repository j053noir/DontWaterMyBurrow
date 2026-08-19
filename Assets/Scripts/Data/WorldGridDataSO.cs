using System.Collections.Generic;
using UnityEngine;

namespace DontWaterMyBurrow.Data
{
    public enum GridCellType
    {
        Empty,
        Structure,
        Dam,
        Resource,
        Water,
        Mud,
    }

    public struct GridObject
    {
        public GameObject Instance;
        public GridCellType Type;

        public GridObject(GameObject instance, GridCellType type)
        {
            Instance = instance;
            Type = type;
        }
    }

    [CreateAssetMenu(fileName = "WorldGridData", menuName = "ScriptableObjects/WorldGridData")]
    public class WorldGridDataSO : ScriptableObject
    {
        private readonly Dictionary<Vector2Int, GridObject> _occupiedCells = new();

        public GridObject? GetCell(Vector2Int position)
        {
            if (_occupiedCells.ContainsKey(position))
            {
                return _occupiedCells[position];
            }

            return null;
        }

        public GridCellType GetCellType(Vector2Int position)
        {
            if (_occupiedCells.ContainsKey(position))
            {
                return _occupiedCells[position].Type;
            }

            return GridCellType.Empty;
        }

        public void SetCell(Vector2Int cell, GridObject gridObject)
        {
            if (!IsCellOccupied(cell))
                _occupiedCells[cell] = gridObject;
        }

        public void RemoveCell(Vector2Int cell)
        {
            if (IsCellOccupied(cell))
            {
                // TODO: Return instance to pool
                if (_occupiedCells[cell].Instance != null) Destroy(_occupiedCells[cell].Instance);
                _occupiedCells.Remove(cell);
            }
        }

        public void Reset()
        {
            foreach (var item in _occupiedCells)
            {
                if (item.Value.Instance != null) Destroy(item.Value.Instance);
            }

            _occupiedCells.Clear();
        }

        public bool IsCellOccupied(Vector2Int position)
        {
            return _occupiedCells.ContainsKey(position);
        }

        public bool IsCellDam(Vector2Int position)
        {
            return _occupiedCells.ContainsKey(position) && _occupiedCells[position].Type == GridCellType.Dam;
        }

        public bool IsCellStructure(Vector2Int position)
        {
            return _occupiedCells.ContainsKey(position) && _occupiedCells[position].Type == GridCellType.Structure;
        }

        public bool IsWalkable(Vector2Int position)
        {
            if (!_occupiedCells.TryGetValue(position, out var gridObject))
            {
                return true;
            }

            return gridObject.Type == GridCellType.Empty
                || gridObject.Type == GridCellType.Mud
                || gridObject.Type == GridCellType.Water
                || gridObject.Type == GridCellType.Resource;
        }

        public bool IsFloodable(Vector2Int position)
        {
            if (!_occupiedCells.TryGetValue(position, out var gridObject))
            {
                return true;
            }

            return gridObject.Type == GridCellType.Empty
                || gridObject.Type == GridCellType.Mud
                || gridObject.Type == GridCellType.Water
                || gridObject.Type == GridCellType.Resource;
        }
    }
}