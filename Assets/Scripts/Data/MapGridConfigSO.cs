using UnityEngine;

namespace DontWaterMyBurrow.Data
{
    [CreateAssetMenu(fileName = "MapGridConfig", menuName = "ScriptableObjects/MapGridConfig")]
    public class MapGridConfigSO : ScriptableObject
    {
        [field: Header("Tile Size")]
        [field: SerializeField] public float TileSize { get; private set; } = 1f;

        [field: Header("Boundaries")]
        [field: SerializeField] public int MinXBoundary { get; private set; } = -5;
        [field: SerializeField] public int MaxXBoundary { get; private set; } = 5;
        [field: SerializeField] public int MinYBoundary { get; private set; } = -5;
        [field: SerializeField] public int MaxYBoundary { get; private set; } = 5;

        [field: Header("Burrow")]
        [field: SerializeField] public Vector2Int BurrowPosition { get; private set; } = Vector2Int.zero;

        public float MinWorldX => MinXBoundary * TileSize;
        public float MaxWorldX => MaxXBoundary * TileSize;
        public float MinWorldY => MinYBoundary * TileSize;
        public float MaxWorldY => MaxYBoundary * TileSize;

        public Vector3 GridToWorld(Vector2Int gridPosition)
        {
            return new Vector3(
                gridPosition.x * TileSize,
                gridPosition.y * TileSize,
                0
            );
        }

        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / TileSize),
                Mathf.FloorToInt(worldPosition.y / TileSize)
            );
        }

        public bool IsWithinBounds(Vector2Int position)
        {
            return position.x >= MinXBoundary && position.x <= MaxXBoundary &&
                   position.y >= MinYBoundary && position.y <= MaxYBoundary;
        }

        public float ClampWorldX(float worldX, float halfWidth = 0f)
        {
            var minX = MinWorldX + halfWidth;
            var maxX = MaxWorldX - halfWidth;

            if (minX <= maxX)
            {
                return Mathf.Clamp(worldX, minX, maxX);
            }

            return (MinWorldX + MaxWorldX) * 0.5f;
        }

        public float ClampWorldY(float worldY, float halfHeight = 0f)
        {
            var minY = MinWorldY + halfHeight;
            var maxY = MaxWorldY - halfHeight;

            if (minY <= maxY)
            {
                return Mathf.Clamp(worldY, minY, maxY);
            }

            return (MinWorldY + MaxWorldY) * 0.5f;
        }
    }
}