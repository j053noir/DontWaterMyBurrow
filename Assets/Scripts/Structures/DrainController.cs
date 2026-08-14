using System;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Hazards;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Structures.State;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Structures
{
    [RequireComponent(typeof(Collider2D))]
    public class DrainController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Setup")]
        [SerializeField] private Vector2Int _position;
        [SerializeField] private int _drainAmount;
        [SerializeField] private int _drainRadius;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;
        public bool DebugMode => _debugMode;

        public Vector2Int Position => _position;
        public int DrainAmount => _drainAmount;
        public int DrainRadius => _drainRadius;

        private StateMachine _stateMachine;
        public DrainCloggedState CloggedState { get; private set; }
        public DrainUncloggedState UncloggedState { get; private set; }

        public bool IsClogged => _stateMachine.CurrentState is DrainCloggedState;

        private void Awake()
        {
            _stateMachine = new();
            CloggedState = new DrainCloggedState(this);
            UncloggedState = new DrainUncloggedState(this);

            _stateMachine.ChangeState(UncloggedState);
        }

        private void Start()
        {
            if (_mapGridConfig == null) Debug.LogError("MapGridConfig is not assigned to DrainController");

            _position = SetDrainPosition();

            transform.position = new Vector3(
                _position.x * _mapGridConfig.TileSize,
                _position.y * _mapGridConfig.TileSize,
                0
            );
        }

        private void OnEnable()
        {
            EventBus.Publish(new RegisterWaterDrainEvent(_position, _drainRadius));
            EventBus.Subscribe<ClearCloggedDrainEvent>(OnClearCloggedDrain);
        }

        private void OnDisable()
        {
            EventBus.Publish(new RemoveWaterDrainEvent(_position));
            EventBus.Unsubscribe<ClearCloggedDrainEvent>(OnClearCloggedDrain);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_debugMode) Debug.Log($"Collisioned with {collision.gameObject.tag}");

            if (collision.gameObject.TryGetComponent<HazardsController>(out var hazard))
            {
                OnCollisionWithHazard(hazard);
            }
        }

        private void OnCollisionWithHazard(HazardsController hazard)
        {
            if (hazard.Type != HazardType.Leaves && !IsClogged)
            {
                _stateMachine.ChangeState(CloggedState);
            }
        }

        private void OnClearCloggedDrain(ClearCloggedDrainEvent @event)
        {
            if (@event.position == _position)
            {
                _stateMachine.ChangeState(UncloggedState);
            }
        }

        public void CleanDrain()
        {
            _stateMachine.ChangeState(UncloggedState);
        }

        private Vector2Int SetDrainPosition()
        {
            var x = GenerateRandomX();
            var y = GenerateRandomY();

            // Avoid placing drain at the same position as the burrow
            while (x == _mapGridConfig.BurrowPosition.x && y == _mapGridConfig.BurrowPosition.y)
            {
                x = GenerateRandomX();
                y = GenerateRandomY();
            }

            return new Vector2Int(x, y);
        }

        private int GenerateRandomX()
        {
            return UnityEngine.Random.Range(_mapGridConfig.MinXBoundary + 1, _mapGridConfig.MaxXBoundary - 1);
        }

        private int GenerateRandomY()
        {
            return UnityEngine.Random.Range(_mapGridConfig.MinYBoundary + _mapGridConfig.MaxYBoundary / 2, _mapGridConfig.MaxYBoundary - 1);
        }
    }
}