using System;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Hazards;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Structures.State;

namespace DontWaterMyBurrow.Structures
{
    [RequireComponent(typeof(Collider2D))]
    public class DrainController : MonoBehaviour
    {
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
    }
}