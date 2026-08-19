using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Hazards;
using DontWaterMyBurrow.Structures.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Structures
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class StructureController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private StructureDataSO _structureData;

        [Header("Parameters")]
        [SerializeField] private StructureType _type;
        [SerializeField] private Vector2Int _position;

        [Header("Health")]
        [SerializeField] private bool _hasBeenDamaged = false;
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;

        public bool IsDamaged => _hasBeenDamaged;
        public int Health => _health;
        public StructureDataSO StructureData => _structureData;
        public StructureType Type => _type;
        public Vector2Int Position => _position;

        private void Awake()
        {
            if (_mapGridConfig == null) Debug.LogError("[StructureController] MapGridConfigSO is null");
            if (_structureData == null) Debug.LogError("[StructureController] StructureDataSO is null");
        }

        private void OnEnable()
        {
            _maxHealth = _structureData.MaxHealth;
            _health = _maxHealth;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_mapGridConfig != null && !Application.isPlaying)
            {
                var gridPosition = _mapGridConfig.WorldToGrid(transform.position);
                transform.SetPositionAndRotation(
                    _mapGridConfig.GridToWorld(gridPosition),
                    _mapGridConfig.SnapToGridRotation(transform.rotation)
                );
            }
        }
#endif
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out HazardsController hazard))
            {
                TakeDamage(hazard.DamageAmount);
            }
        }

        public void Repair(int amount)
        {
            _health = Mathf.Min(_maxHealth, _health + amount);

            if (_health >= _maxHealth) _hasBeenDamaged = false;

            EventBus.Publish(new StructureRepairedEvent(_type, this.gameObject));
        }

        public void TakeDamage(int damageTaken)
        {
            _health -= damageTaken;
            _hasBeenDamaged = true;

            if (_health <= 0)
            {
                _health = 0;
                DestroyStructure();
            }
        }

        private void DestroyStructure()
        {
            // TODO: Implement object pooling to about instancing new ones
            EventBus.Publish(new StructureDestroyedEvent(Position, gameObject));
            Destroy(this.gameObject);
        }
    }
}