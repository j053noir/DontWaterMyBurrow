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

        /// <summary>
        /// Minimum X boundary in world coordinates.
        /// </summary>
        public float MinWorldX => MinXBoundary * TileSize;

        /// <summary>
        /// Maximum X boundary in world coordinates.
        /// </summary>
        public float MaxWorldX => MaxXBoundary * TileSize;

        /// <summary>
        /// Minimum Y boundary in world coordinates.
        /// </summary>
        public float MinWorldY => MinYBoundary * TileSize;

        /// <summary>
        /// Maximum Y boundary in world coordinates.
        /// </summary>
        public float MaxWorldY => MaxYBoundary * TileSize;

        /// <summary>
        /// Converts integer grid coordinates to the world-space center position of the tile.
        /// </summary>
        /// <param name="gridPosition">The integer grid coordinate (x, y).</param>
        /// <returns>The center point of the tile in world space.</returns>
        public Vector3 GridToWorld(Vector3 position)
        {
            return new Vector3(
                position.x * TileSize,
                position.y * TileSize,
                0f
            );
        }

        /// <summary>
        /// Converts integer grid coordinates to the world-space center position of the tile.
        /// </summary>
        /// <param name="gridPosition">The integer grid coordinate (x, y).</param>
        /// <returns>The center point of the tile in world space.</returns>
        public Vector3 GridToWorld(Vector2Int gridPosition)
        {
            return new Vector3(
                (gridPosition.x + 0.5f) * TileSize,
                (gridPosition.y + 0.5f) * TileSize,
                0f
            );
        }

        /// <summary>
        /// Converts a continuous world-space position to its corresponding integer grid coordinate.
        /// </summary>
        /// <param name="worldPosition">The world-space position.</param>
        /// <returns>The integer grid coordinate containing the world position.</returns>
        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / TileSize),
                Mathf.FloorToInt(worldPosition.y / TileSize)
            );
        }

        /// <summary>
        /// Determines whether a grid coordinate lies within the configured map boundaries.
        /// </summary>
        /// <param name="position">The grid coordinate to test.</param>
        /// <returns>True if the coordinate is within boundaries; otherwise, false.</returns>
        public bool IsWithinBounds(Vector2Int position)
        {
            return position.x >= MinXBoundary && position.x <= MaxXBoundary &&
                   position.y >= MinYBoundary && position.y <= MaxYBoundary;
        }

        /// <summary>
        /// Clamps a continuous horizontal world coordinate within the map boundaries, optionally considering the object's half-width.
        /// </summary>
        /// <param name="worldX">The raw X position in world space.</param>
        /// <param name="halfWidth">Optional half-width of the object to prevent border overflow.</param>
        /// <returns>The clamped X coordinate in world space.</returns>
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

        /// <summary>
        /// Clamps a continuous vertical world coordinate within the map boundaries, optionally considering the object's half-height.
        /// </summary>
        /// <param name="worldY">The raw Y position in world space.</param>
        /// <param name="halfHeight">Optional half-height of the object to prevent border overflow.</param>
        /// <returns>The clamped Y coordinate in world space.</returns>
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

        /// <summary>
        /// Snaps any rotation to the nearest 90-degree orthogonal quadrant (0°, 90°, 180°, 270°).
        /// </summary>
        /// <param name="rotation">The original rotation quaternion.</param>
        /// <returns>An orthogonal rotation aligned with the grid.</returns>
        public Quaternion SnapToGridRotation(Quaternion rotation)
        {
            float angle = rotation.eulerAngles.z;
            float snappedAngle = Mathf.Round(angle / 90f) * 90f;
            return Quaternion.Euler(0f, 0f, snappedAngle);
        }

        /// <summary>
        /// Converts a cardinal direction vector into its corresponding 90-degree orthogonal grid rotation.
        /// </summary>
        /// <param name="direction">The cardinal 2D direction (right, up, left, down).</param>
        /// <returns>The orthogonal rotation quaternion aligned with the specified direction.</returns>
        public Quaternion DirectionToGridRotation(Vector2Int direction)
        {
            if (direction == Vector2Int.right) return Quaternion.Euler(0f, 0f, 0f);
            if (direction == Vector2Int.up) return Quaternion.Euler(0f, 0f, 90f);
            if (direction == Vector2Int.left) return Quaternion.Euler(0f, 0f, 180f);
            if (direction == Vector2Int.down) return Quaternion.Euler(0f, 0f, 270f);

            return Quaternion.identity;
        }
    }
}