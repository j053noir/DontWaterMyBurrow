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

        public Vector2Int GridToWorld(GameObject owner, Vector2Int gridPosition)
        {
            return new Vector2Int(
                gridPosition.x + Mathf.RoundToInt(owner.transform.position.x / TileSize),
                gridPosition.y + Mathf.RoundToInt(owner.transform.position.y / TileSize)
            );
        }

        public Vector2Int WorldToGrid(GameObject owner, Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x - owner.transform.position.x / TileSize),
                Mathf.RoundToInt(worldPosition.y - owner.transform.position.y / TileSize)
            );
        }
    }
}