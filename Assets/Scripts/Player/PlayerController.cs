using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Player.Events;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Resources;
using DontWaterMyBurrow.Resources.Events;

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

        public Vector2Int TargetCell => _targetCell;

        [Header("State")]
        [SerializeField] private bool _isOnMud = false;

        private Rigidbody2D _rigidBody2d;

        private void Awake()
        {
            _rigidBody2d = GetComponent<Rigidbody2D>();
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

        private void FixedUpdate()
        {
            _rigidBody2d.MovePosition(_rigidBody2d.position + _currentMoveSpeed * Time.fixedDeltaTime * _moveInput);
        }

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();

            if (_moveInput.sqrMagnitude > 0.01f)
            {
                _facingDirection = _moveInput.normalized;

                var targetCell = this.targetCell();

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
                        StartCoroutine(UnclogWaterPump(waterPump));
                    }
                }
                else if (hit.gameObject.TryGetComponent(out DrainController drain))
                {
                    if (drain.IsClogged)
                    {
                        StartCoroutine(UnclogDrain(drain));
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

        private IEnumerator UnclogWaterPump(WaterPumpController waterPump)
        {
            // TODO: Do unclogging animation, SFX, particles

            yield return new WaitForSeconds(1f);

            waterPump.SetClogState(false);
        }

        private IEnumerator UnclogDrain(DrainController drain)
        {
            // TODO: Do unclogging animation, SFX, particles

            yield return new WaitForSeconds(1f);

            drain.SetClogState(false);
        }

        public void OnBuild(InputValue value)
        {
            if (!value.isPressed) return;

            var targetBuildCell = targetCell();

            EventBus.Publish(new ConfirmBuildEvent(targetBuildCell));
        }

        private Vector2Int targetCell()
        {
            var currentCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            var facingCell = new Vector2Int(Mathf.RoundToInt(_facingDirection.x), Mathf.RoundToInt(_facingDirection.y));

            return currentCell + facingCell;
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.MainMenu || @event.NewState == GameState.GameOver || @event.NewState == GameState.Victory)
            {
                // TODO: Bloquear movimiento.
                _currentMoveSpeed = 0;
                _isOnMud = false;
            }
            else if (@event.NewState == GameState.GamePlay || @event.NewState == GameState.WavePreparation)
            {
                // TODO: Habilitar movimiento.
                SetMoveSpeed();
            }
        }

        private void OnPlayerOnMudEvent(PlayerOnMudEvent @event)
        {
            _isOnMud = @event.OnMud;
            SetMoveSpeed();
        }

        private void SetMoveSpeed()
        {
            _currentMoveSpeed = _isOnMud ? _baseMoveSpeed * 0.5f : _baseMoveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Mud"))
            {
                _isOnMud = true;
                EventBus.Publish(new PlayerOnMudEvent(true));
                SetMoveSpeed();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Mud"))
            {
                _isOnMud = false;
                EventBus.Publish(new PlayerOnMudEvent(false));
                SetMoveSpeed();
            }
        }
    }
}
