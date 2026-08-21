using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Hazards;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Structures.States;
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

        public StructureDataSO StructureData => _structureData;

        [Header("Parameters")]
        [SerializeField] private StructureType _type;
        [SerializeField] private Vector2Int _currentCell;

        public StructureType Type => _type;
        public Vector2Int CurrentCell => _currentCell;

        [Header("Health")]
        [SerializeField] private bool _hasBeenDamaged = false;
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;

        public bool IsDamaged => _hasBeenDamaged;
        public int Health => _health;

        [Header("Debug")]
        [SerializeField] public bool debugMode = false;

        private StateMachine _stateMachine;
        private StructureIdleState _idleState;
        private StructurePushedState _pushedState;

        private void Awake()
        {
            if (_mapGridConfig == null) Debug.LogError("[StructureController] MapGridConfigSO is null");
            if (_structureData == null) Debug.LogError("[StructureController] StructureDataSO is null");

            _stateMachine = new StateMachine();
            _idleState = new StructureIdleState(this);

            _stateMachine.ChangeState(_idleState);
        }

        private void Update()
        {
            if (_stateMachine.CurrentState is IUpdateableState updateableState)
            {
                updateableState.Update();
            }
        }

        private void OnEnable()
        {
            _maxHealth = _structureData.MaxHealth;
            _health = _maxHealth;
            _currentCell = _mapGridConfig.WorldToGrid(transform.position);

            EventBus.Subscribe<StructureChangeCellEvent>(OnStructureChangeCell);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<StructureChangeCellEvent>(OnStructureChangeCell);
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

        private void OnStructureChangeCell(StructureChangeCellEvent @event)
        {
            if (@event.Structure == gameObject)
            {
                var targetWorldPosition = _mapGridConfig.GridToWorld(@event.To);
                var offset = transform.position - @event.PusherTransform.position;
                _pushedState = new StructurePushedState(this, @event.PusherTransform, targetWorldPosition, offset);
                _stateMachine.ChangeState(_pushedState);
            }
        }

        public void Repair(int amount)
        {
            _health = Mathf.Min(_maxHealth, _health + amount);

            if (_health >= _maxHealth) _hasBeenDamaged = false;

            EventBus.Publish(new StructureRepairedEvent(_type, gameObject));
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
            EventBus.Publish(new StructureDestroyedEvent(_currentCell, gameObject));
            Destroy(this.gameObject);
        }

        public void SetPosition(Vector3 targetPosition)
        {
            transform.position = targetPosition;
        }

        public void ToIdle()
        {
            _stateMachine.ChangeState(_idleState);
        }

        public void SetCell(Vector3 targetPosition)
        {
            _currentCell = _mapGridConfig.WorldToGrid(targetPosition);
        }
    }
}