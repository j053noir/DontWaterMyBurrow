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

namespace DontWaterMyBurrow.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _baseMoveSpeed = 5f;
        [SerializeField] private float _currentMoveSpeed = 5f;
        [SerializeField] private Vector2 _moveInput;
        [SerializeField] private Vector2 _facingDirection = Vector2.down;
        [SerializeField] private float _interactionDistance = 1f;
        [SerializeField] private Vector2Int _targetCell;
        [SerializeField] private int _repairAmount = 10;

        private Rigidbody2D _rigidBody2d;

        public StateMachine StateMachine;
        public PlayerDisabledState DisabledState { get; private set; }
        public PlayerNormalState NormalState { get; private set; }
        public PlayerMudState MudState { get; private set; }
        public PlayerUncloggingState UncloggingState { get; private set; }

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
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<PlayerOnMudEvent>(OnPlayerOnMudEvent);
        }

        private void Update()
        {
            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            _rigidBody2d.MovePosition(_rigidBody2d.position + _currentMoveSpeed * Time.fixedDeltaTime * _moveInput);
            StateMachine.FixedUpdate();
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.MainMenu || @event.NewState == GameState.GameOver || @event.NewState == GameState.Victory)
            {
                StateMachine.ChangeState(DisabledState);
            }
            else if (@event.NewState == GameState.GamePlay || @event.NewState == GameState.WavePreparation)
            {
                StateMachine.ChangeState(NormalState);
            }
        }

        private void OnPlayerOnMudEvent(PlayerOnMudEvent @event)
        {
            StateMachine.ChangeState(@event.OnMud ? MudState : NormalState);
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
            _moveInput = value.Get<Vector2>();

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
                            else Debug.LogWarning("Can't repair. Player doesn't have enough resources.");
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

            var targetBuildCell = TargetCell();

            EventBus.Publish(new ConfirmBuildEvent(targetBuildCell));
        }
    }
}
