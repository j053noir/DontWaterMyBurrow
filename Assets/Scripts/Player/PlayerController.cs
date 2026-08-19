using System.Collections.Generic;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Player.Events;
using DontWaterMyBurrow.Player.States;
using DontWaterMyBurrow.Resources;
using DontWaterMyBurrow.Resources.Events;
using DontWaterMyBurrow.Structures;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.UI.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DontWaterMyBurrow.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private WorldGridDataSO _worldGridData;
        [SerializeField] private TransformAnchorSO _playerTransformAnchor;

        [Header("Movement")]
        [SerializeField] private Vector2Int _currentCell;
        [SerializeField] private Vector2Int _targetCell;
        [SerializeField] private Vector2Int _facingDirection = Vector2Int.down;

        [Header("Movement Speed")]
        [SerializeField] private float _baseMoveSpeed = 5f;
        [SerializeField] private float _currentMoveSpeed = 5f;

        [Header("Actions")]
        [SerializeField] private float _interactionDistance = 1f;
        [SerializeField] private int _repairAmount = 10;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;
        public bool DebugMode => _debugMode;

        private Vector2Int _moveDirection;
        private bool _isMoving;

        private Rigidbody2D _rigidBody2d;

        #region States
        public StateMachine StateMachine;
        public PlayerDisabledState DisabledState { get; private set; }
        public PlayerNormalState NormalState { get; private set; }
        public PlayerMudState MudState { get; private set; }
        public PlayerUncloggingState UncloggingState { get; private set; }
        public PlayerBuildMenuState BuildMenuState { get; private set; }
        public PlayerPlacementState PlacementState { get; private set; }
        #endregion

        private void Awake()
        {
            StateMachine = new();

            DisabledState = new PlayerDisabledState(this);
            NormalState = new PlayerNormalState(this, _baseMoveSpeed);
            MudState = new PlayerMudState(this, _baseMoveSpeed * 0.5f);

            _rigidBody2d = GetComponent<Rigidbody2D>();

            if (_mapGridConfig == null) Debug.LogError("[PlayerController] MapGridConfigSO is null");
            if (_worldGridData == null) Debug.LogError("[PlayerController] WorldGridDataSO is null");
            if (_playerTransformAnchor == null) Debug.LogError("[PlayerController] PlayerTransformAnchorSO is null");
        }

        private void Start()
        {
            _currentCell = _mapGridConfig.WorldToGrid(transform.position);
            StateMachine.ChangeState(DisabledState);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<BuildFailedEvent>(OnBuildFailed);
            EventBus.Subscribe<ClosedBuildMenuEvent>(OnClosedBuildMenuEvent);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<OutOfResourcesEvent>(OnOutOfResources);
            EventBus.Subscribe<PlayerOnMudEvent>(OnPlayerOnMudEvent);
            EventBus.Subscribe<SelectStructureToBuildEvent>(OnSelectStructureToBuildEvent);
            EventBus.Subscribe<StructureBuiltEvent>(OnStructureBuilt);

            _playerTransformAnchor.Transform = transform;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<BuildFailedEvent>(OnBuildFailed);
            EventBus.Unsubscribe<ClosedBuildMenuEvent>(OnClosedBuildMenuEvent);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<OutOfResourcesEvent>(OnOutOfResources);
            EventBus.Unsubscribe<PlayerOnMudEvent>(OnPlayerOnMudEvent);
            EventBus.Unsubscribe<SelectStructureToBuildEvent>(OnSelectStructureToBuildEvent);
            EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);

            _playerTransformAnchor.Transform = null;
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

        #region Event Handlers

        private void OnBuildFailed(BuildFailedEvent @event)
        {
            if (StateMachine.CurrentState is PlayerBuildMenuState buildState)
            {
                StateMachine.ChangeState(buildState.PreviousState);
            }
        }

        private void OnClosedBuildMenuEvent(ClosedBuildMenuEvent @event)
        {
            if (StateMachine.CurrentState is PlayerBuildMenuState buildState)
            {
                StateMachine.ChangeState(buildState.PreviousState);
            }
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

        private void OnOutOfResources(OutOfResourcesEvent @event)
        {
            if (StateMachine.CurrentState is PlayerPlacementState placementState && placementState.StructureData == @event.StructureData)
            {
                StateMachine.ChangeState(placementState.PreviousState);
            }
        }

        private void OnPlayerOnMudEvent(PlayerOnMudEvent @event)
        {
            StateMachine.ChangeState(@event.OnMud ? MudState : NormalState);
        }

        private void OnSelectStructureToBuildEvent(SelectStructureToBuildEvent @event)
        {
            if (StateMachine.CurrentState is PlayerBuildMenuState buildState)
            {
                PlacementState = new PlayerPlacementState(this, @event.StructureData, buildState.PreviousState);
                StateMachine.ChangeState(PlacementState);
            }
        }

        private void OnStructureBuilt(StructureBuiltEvent @event)
        {
            if (StateMachine.CurrentState is PlayerBuildMenuState buildState)
            {
                StateMachine.ChangeState(buildState.PreviousState);
            }
        }
        #endregion

        private void PlayerMove()
        {
            if (_isMoving)
            {
                var targetWorldPosition = _mapGridConfig.GridToWorld(_targetCell);

                var nextPos = Vector2.MoveTowards(_rigidBody2d.position, targetWorldPosition, _currentMoveSpeed * Time.fixedDeltaTime);
                _rigidBody2d.MovePosition(nextPos);

                if (Vector2.Distance(targetWorldPosition, _rigidBody2d.position) < 0.001f)
                {
                    _rigidBody2d.position = targetWorldPosition;
                    _currentCell = _targetCell;
                    _isMoving = false;
                    _targetCell = _currentCell + _facingDirection;
                }
            }
            else if (_moveDirection != Vector2Int.zero)
            {
                var desiredCell = _currentCell + _moveDirection;
                if (_mapGridConfig.IsWithinBounds(desiredCell) && _worldGridData.IsWalkable(desiredCell))
                {
                    _targetCell = desiredCell;
                    _isMoving = true;
                }
                else
                {
                    _isMoving = false;
                }
            }
        }

        private Vector2Int TargetCell()
        {
            var currentCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

            return currentCell + _moveDirection;
        }

        # region Input Actions
        public void OnMove(InputValue value)
        {
            var rawInput = value.Get<Vector2>();

            if (Mathf.Abs(rawInput.y) > 0.01f && Mathf.Abs(rawInput.y) >= Mathf.Abs(rawInput.x))
            {
                _moveDirection = new Vector2Int(0, (int)Mathf.Sign(rawInput.y));
            }
            else if (Mathf.Abs(rawInput.x) > 0.01f)
            {
                _moveDirection = new Vector2Int((int)Mathf.Sign(rawInput.x), 0);
            }
            else
            {
                _moveDirection = Vector2Int.zero;
            }

            if (_moveDirection != Vector2Int.zero)
            {
                _facingDirection = _moveDirection;

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

            if (StateMachine.CurrentState is PlayerBuildMenuState)
            {
                return;
            }
            else if (StateMachine.CurrentState is PlayerPlacementState placementState)
            {
                if (!placementState.CanConfirmPlacement) return;

                EventBus.Publish(new ConfirmBuildEvent(_targetCell));
            }
            else
            {
                InteractWith();
            }
        }

        public void InteractWith()
        {
            var facingDirection = new Vector3(_facingDirection.x, _facingDirection.y, 0);
            var objetive = transform.position + facingDirection * _interactionDistance;
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
            if (!value.isPressed || StateMachine.CurrentState is PlayerBuildMenuState) return;

            BuildMenuState = new PlayerBuildMenuState(this, _targetCell, StateMachine.CurrentState);
            StateMachine.ChangeState(BuildMenuState);
        }

        public void OnCancel(InputValue value)
        {
            if (!value.isPressed) return;

            if (StateMachine.CurrentState is PlayerBuildMenuState)
            {
                return;
            }
            else if (StateMachine.CurrentState is PlayerPlacementState placementState)
            {
                StateMachine.ChangeState(placementState.PreviousState);
            }
        }

        public void OnMap(InputValue value)
        {
            if (!value.isPressed) return;

            // TODO: Show all map
            if (_debugMode) Debug.Log("Map opened");
        }
        #endregion

        public void SetMoveSpeed(float moveSpeed)
        {
            _currentMoveSpeed = moveSpeed;
        }
    }
}
