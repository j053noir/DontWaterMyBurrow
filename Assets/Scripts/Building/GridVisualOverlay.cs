using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Building
{
    public class GridVisualOverlay : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Visuals")]
        [SerializeField] private Sprite _tileSprite;
        [SerializeField] private Color _gridColor = new Color(1f, 1f, 1f, 0.15f);
        [SerializeField] private string _sortingLayerName = "Default";
        [SerializeField] private int _sortingOrder = -1;

        private Transform _gridParent;

        private void Awake()
        {
            _gridParent = new GameObject("VisualGridContainer").transform;
            _gridParent.SetParent(transform);
            _gridParent.position = Vector3.zero;
            GenerateGrid();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            bool isVisible = @event.NewState == GameState.WavePreparation || @event.NewState == GameState.WaveActive;
            _gridParent.gameObject.SetActive(isVisible);
        }

        private void GenerateGrid()
        {
            for (int x = _mapGridConfig.MinXBoundary; x <= _mapGridConfig.MaxXBoundary; x++)
            {
                for (int y = _mapGridConfig.MinYBoundary; y <= _mapGridConfig.MaxYBoundary; y++)
                {
                    var tile = new GameObject($"Tile_{x}_{y}");
                    tile.transform.SetParent(_gridParent);
                    tile.transform.position = _mapGridConfig.GridToWorld(new Vector2Int(x, y));

                    if (_tileSprite != null)
                    {
                        float sprideWidth = _tileSprite.bounds.size.x;
                        float sprideHeight = _tileSprite.bounds.size.y;
                        tile.transform.localScale = new Vector3(_mapGridConfig.TileSize / sprideWidth, _mapGridConfig.TileSize / sprideHeight, 1);
                        var spriteRenderer = tile.AddComponent<SpriteRenderer>();
                        spriteRenderer.sprite = _tileSprite;
                        spriteRenderer.color = _gridColor;
                        spriteRenderer.sortingOrder = _sortingOrder;
                        spriteRenderer.sortingLayerName = _sortingLayerName;
                    }
                }
            }
        }
    }
}
