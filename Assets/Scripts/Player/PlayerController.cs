using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Player.Events;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Resources;
using DontWaterMyBurrow.Resources.Events;
using DontWaterMyBurrow.Player.States;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Data;
using System;

namespace DontWaterMyBurrow.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Movement")]
        [SerializeField] private float _baseMoveSpeed = 5f;
        [SerializeField] private float _currentMoveSpeed = 5f;
        [SerializeField] private Vector2 _moveInput;
        [SerializeField] private Vector2 _facingDirection = Vector2.down;
        [SerializeField] private float _interactionDistance = 1f;
        [SerializeField] private Vector2Int _targetCell;
        [SerializeField] private int _repairAmount = 10;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;
        public bool DebugMode => _debugMode;

        private Rigidbody2D _rigidBody2d;

        public StateMachine StateMachine;
        public PlayerDisabledState DisabledState { get; private set; }
        public PlayerNormalState NormalState { get; private set; }
        public PlayerMudState MudState { get; private set; }
        public PlayerUncloggingState UncloggingState { get; private set; }
        public PlayerBuildState BuildState { get; private set; }

        private void Awake()
        {
            StateMachine = new();
            DisabledState = new PlayerDisabledState(this);
            NormalState = new PlayerNormalState(this, _baseMoveSpeed);
            MudState = new PlayerMudState(this, _baseMoveSpeed * 0.5f);

            _rigidBody2d = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            StateMachine.ChangeState(DisabledState);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<PlayerOnMudEvent>(OnPlayerOnMudEvent);
            EventBus.Subscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Subscribe<BuildFailedEvent>(OnBuildFailed);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<PlayerOnMudEvent>(OnPlayerOnMudEvent);
            EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Unsubscribe<BuildFailedEvent>(OnBuildFailed);
        }

        private void Update()
        {
            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            PlayerMove();
            StateMachine.FixedUpdate();
        }

        private void PlayerMove()
        {
            Vector2 nextPosition = _rigidBody2d.position + _moveInput * (_currentMoveSpeed * Time.fixedDeltaTime);

            if (_mapGridConfig != null)
            {
                nextPosition.x = Mathf.Clamp(nextPosition.x, _mapGridConfig.MinXBoundary, _mapGridConfig.MaxXBoundary);
                nextPosition.y = Mathf.Clamp(nextPosition.y, _mapGridConfig.MinYBoundary, _mapGridConfig.MaxYBoundary);
            }

            _rigidBody2d.MovePosition(nextPosition);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.MainMenu ||
                @event.NewState == GameState.GameOver ||
                @event.NewState == GameState.Victory ||
                @event.NewState == GameState.Pause)
            {
                StateMachine.ChangeState(DisabledState);
            }
            else if (@event.NewState == GameState.WavePreparation || @event.NewState == GameState.WaveActive)
            {
                if (_debugMode) Debug.Log($"[PlayerController] Game state changed to {@event.NewState}, Time Scale: {Time.timeScale}");
                StateMachine.ChangeState(NormalState);
            }
        }

        private void OnPlayerOnMudEvent(PlayerOnMudEvent @event)
        {
            StateMachine.ChangeState(@event.OnMud ? MudState : NormalState);
        }

        private void OnBuildFailed(BuildFailedEvent @event)
        {
            if (StateMachine.CurrentState is PlayerBuildState buildState)
            {
                StateMachine.ChangeState(buildState.PreviousState);
            }
        }

        private void OnStructureBuilt(StructureBuiltEvent @event)
        {
            if (StateMachine.CurrentState is PlayerBuildState buildState)
            {
                StateMachine.ChangeState(buildState.PreviousState);
            }
        }

        private Vector2Int TargetCell()
        {
            var currentCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            var facingCell = new Vector2Int(Mathf.RoundToInt(_facingDirection.x), Mathf.RoundToInt(_facingDirection.y));

            return currentCell + facingCell;
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            _currentMoveSpeed = moveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Mud"))
            {
                EventBus.Publish(new PlayerOnMudEvent(true));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Mud"))
            {
                EventBus.Publish(new PlayerOnMudEvent(false));
            }
        }

        public void OnMove(InputValue value)
        {
            var rawInput = value.Get<Vector2>();

            if (Mathf.Abs(rawInput.y) > 0.01f && Mathf.Abs(rawInput.y) >= Mathf.Abs(rawInput.x))
            {
                _moveInput = new Vector2(0f, Mathf.Sign(rawInput.y));
            }
            else if (Mathf.Abs(rawInput.x) > 0.01f)
            {
                _moveInput = new Vector2(Mathf.Sign(rawInput.x), 0f);
            }
            else
            {
                _moveInput = Vector2.zero;
            }

            if (_moveInput.sqrMagnitude > 0.01f)
            {
                _facingDirection = _moveInput.normalized;

                var targetCell = this.TargetCell();

                if (_targetCell != targetCell)
                {
                    _targetCell = targetCell;
                    EventBus.Publish(new PlayerBuildTargetChangedEvent(targetCell));
                }
            }
        }

        public void OnInteract(InputValue value)
        {
            if (!value.isPressed) return;

            if (StateMachine.CurrentState is PlayerBuildState buildState)
            {
                EventBus.Publish(new ConfirmBuildEvent(_targetCell));
                StateMachine.ChangeState(buildState.PreviousState);
            }
            else
            {
                InteractWith();
            }
        }

        public void InteractWith()
        {
            var objetive = transform.position + (Vector3)_facingDirection * _interactionDistance;
            var hit = Physics2D.OverlapCircle(objetive, 0.3f);

            if (hit)
            {
                // Structure is target
                if (hit.gameObject.TryGetComponent(out StructureController structure))
                {
                    if (structure.IsDamaged)
                    {
                        EventBus.Publish(new RepairStructureRequestEvent(structure.Position, structure.DataSO, (success) =>
                        {
                            if (success) structure.Repair(_repairAmount);
                            else if (_debugMode) Debug.LogWarning("Can't repair. Player doesn't have enough resources.");
                        }));
                    }
                }
                // Water pump is target
                else if (hit.gameObject.TryGetComponent(out WaterPumpController waterPump))
                {
                    if (waterPump.IsClogged)
                    {
                        UncloggingState = new PlayerUncloggingState(this, StateMachine.CurrentState, 1f, waterPump.gameObject);
                        StateMachine.ChangeState(UncloggingState);
                    }
                }
                else if (hit.gameObject.TryGetComponent(out DrainController drain))
                {
                    if (drain.IsClogged)
                    {
                        UncloggingState = new PlayerUncloggingState(this, StateMachine.CurrentState, 1f, drain.gameObject);
                        StateMachine.ChangeState(UncloggingState);
                    }
                }
                else if (hit.gameObject.TryGetComponent(out ResourceNodeController resourceNode))
                {
                    EventBus.Publish(new ResourceCollectedEvent(_targetCell, resourceNode.Type, resourceNode.Amount));

                    // TODO: Return to pool
                    resourceNode.gameObject.SetActive(false);
                }
            }
        }

        public void OnBuild(InputValue value)
        {
            if (!value.isPressed) return;

            BuildState = new PlayerBuildState(this, _targetCell, StateMachine.CurrentState);
            StateMachine.ChangeState(BuildState);
        }

        public void OnCancel(InputValue value)
        {
            if (!value.isPressed) return;

            if (StateMachine.CurrentState is PlayerBuildState buildState)
            {
                StateMachine.ChangeState(buildState.PreviousState);
            }
        }

        public void OnMap(InputValue value)
        {
            if (!value.isPressed) return;

            // TODO: Show all map
            if (_debugMode) Debug.Log("Map opened");
        }
    }
}
