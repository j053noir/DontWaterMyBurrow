using System;
using System.Collections.Generic;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Water.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Water
{
    [Serializable]
    public class WaterCell
    {
        public Vector2Int Position;
        public float WaterLevel;
    }

    public class WaterVisualRenderer : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Visuals")]
        [SerializeField] private Sprite _waterSprite;
        [SerializeField] private Color _waterColor = new(0.5f, 0.7f, 0.8f, 0.7f);
        [SerializeField] private string _sortingLayerName = "Default";
        [SerializeField] private int _sortingOrder = -1;

        [Header("Debug")]
        [SerializeField] private bool _debugMode;

        private Dictionary<Vector2Int, SpriteRenderer> _waterSpritePool;
        [SerializeField] private List<WaterCell> _DebugCellLevel;

        private void Awake()
        {
            _waterSpritePool = new();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WaterGridUpdateEvent>(OnWaterGridUpdated);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaterGridUpdateEvent>(OnWaterGridUpdated);
        }

        private void OnWaterGridUpdated(WaterGridUpdateEvent @event)
        {
            foreach (var waterCell in @event.WaterGrid)
            {
                if (waterCell.Value > 0)
                {
                    GetWaterSpriteAtPosition(waterCell.Key, waterCell.Value);
                }
                else if (_waterSpritePool.TryGetValue(waterCell.Key, out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.gameObject.SetActive(false);
                }
            }
        }

        private void GetWaterSpriteAtPosition(Vector2Int key, float waterLevel)
        {
            if (!_waterSpritePool.TryGetValue(key, out SpriteRenderer spriteRenderer))
            {
                var waterObject = new GameObject($"Water_{key.x}_{key.y}");
                waterObject.transform.parent = transform;
                waterObject.transform.position = new Vector3(key.x * _mapGridConfig.TileSize, key.y * _mapGridConfig.TileSize, 0);

                if (_waterSprite != null)
                {
                    float sprideWidth = _waterSprite.bounds.size.x;
                    float sprideHeight = _waterSprite.bounds.size.y;
                    spriteRenderer = waterObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _waterSprite;
                    spriteRenderer.sortingOrder = _sortingOrder;
                    spriteRenderer.sortingLayerName = _sortingLayerName;
                    spriteRenderer.transform.localScale = new Vector3(_mapGridConfig.TileSize / sprideWidth, _mapGridConfig.TileSize / sprideHeight, 1);
                }

                if (_debugMode) _DebugCellLevel.Add(new WaterCell { Position = key, WaterLevel = waterLevel });
                _waterSpritePool.Add(key, spriteRenderer);
            }

            var calculatedAlpha = Mathf.Lerp(0.1f, _waterColor.a, waterLevel);
            spriteRenderer.color = new Color(_waterColor.r, _waterColor.g, _waterColor.b, calculatedAlpha);
            spriteRenderer.gameObject.SetActive(true);

            if (_debugMode) Debug.Log($"WaterVisualRenderer: Set water at position {key} with level {waterLevel}");
        }
    }
}