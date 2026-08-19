using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Structures;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Data;
using System.Collections.Generic;

namespace DontWaterMyBurrow.Hazards
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class HazardsController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] MapGridConfigSO _mapGridConfig;
        [SerializeField] private HazardType _hazardType;
        [SerializeField] private int _damageAmount;
        [SerializeField] private float _speed = 1.25f;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;

        private Dictionary<Vector2Int, Vector2Int> _waterFlows;
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;

        public HazardType Type => _hazardType;
        public int DamageAmount => _damageAmount;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WaterFlowUpdatedEvent>(OnWaterFlowUpdated);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaterFlowUpdatedEvent>(OnWaterFlowUpdated);
        }

        private void FixedUpdate()
        {
            MoveWithCurrent();
        }

        private void MoveWithCurrent()
        {
            if (_rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
            {
                if (_mapGridConfig == null)
                {
                    if (_debugMode) Debug.LogWarning("[HazardsController] _mapGridConfig is not assigned!");
                    return;
                }

                var gridPostion = _mapGridConfig.WorldToGrid(transform.position);
                if (_waterFlows == null ||
                    !_waterFlows.TryGetValue(gridPostion, out var flow) ||
                    flow == Vector2Int.zero)
                {
                    return;
                }
                var targetPosition = _rigidbody2D.position + _speed * Time.fixedDeltaTime * (Vector2)flow;

                float minX = (_mapGridConfig.MinXBoundary + 0.5f) * _mapGridConfig.TileSize;
                float maxX = (_mapGridConfig.MaxXBoundary + 0.5f) * _mapGridConfig.TileSize;
                float minY = (_mapGridConfig.MinYBoundary + 0.5f) * _mapGridConfig.TileSize;
                float maxY = (_mapGridConfig.MaxYBoundary + 0.5f) * _mapGridConfig.TileSize;

                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

                _rigidbody2D.MovePosition(targetPosition);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_debugMode) Debug.Log($"Collisioned with {collision.gameObject.tag}");

            if (collision.gameObject.TryGetComponent<StructureController>(out var structure))
            {
                OnCollisionWithStructure(structure);
            }
            else if (collision.gameObject.TryGetComponent<HazardsController>(out var hazard))
            {
                OnCollisionWithHazard(hazard);
            }
            else if (_debugMode)
            {
                Debug.Log($"Collisioned with uknown object: {collision.gameObject.name} {collision.gameObject.tag}");
            }
        }

        private void OnWaterFlowUpdated(WaterFlowUpdatedEvent @event)
        {
            _waterFlows = @event.CellsFlow;
        }

        protected virtual void OnCollisionWithStructure(StructureController structure)
        {
            // Create a dam if the hazard is a log and the structure is a sandbag
            if (_hazardType == HazardType.Log && structure.Type == StructureType.SandBag)
            {
                CreateDam();
            }
            // Deal damage if the hazard is not leaves
            else if (_hazardType != HazardType.Leaves)
            {
                structure.TakeDamage(_damageAmount);
            }
            // Leaves only interact with water pumps
            else if (_hazardType == HazardType.Leaves && structure is WaterPumpController waterPump)
            {
                waterPump.SetClogState(true);
                // TODO: Put leaves in object pool
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionWithHazard(HazardsController hazard)
        {
            // Create a dam if both hazards are logs or rock
            if (_hazardType == HazardType.Log && (hazard.Type == HazardType.Rock || hazard.Type == HazardType.Log))
            {
                CreateDam();
            }
        }

        private void CreateDam()
        {
            if (_hazardType != HazardType.Log)
            {
                return;
            }

            transform.SetPositionAndRotation(
                _mapGridConfig.GridToWorld(transform.position),
                _mapGridConfig.SnapToGridRotation(transform.rotation)
            );
            _rigidbody2D.bodyType = RigidbodyType2D.Static;
            _collider2D.isTrigger = true;

            if (TryGetComponent<DamController>(out var dam))
            {
                dam.enabled = true;
            }
        }

        public void EnableDebugMode()
        {
            _debugMode = true;
        }

        public void DisableDebugMode()
        {
            _debugMode = false;
        }
    }
}