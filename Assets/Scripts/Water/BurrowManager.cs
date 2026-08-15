using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Water
{
    public class BurrowManager : MonoBehaviour
    {
        [Header("Map Grid Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Flood Meter")]
        [SerializeField] private float _floodMeter = 0;
        [SerializeField] private float _inflowRate = 0;
        [SerializeField] private float _maxFloodCapacity = 100;

        public float FloodMeter => _floodMeter;
        public float MaxFloodCapacity => _maxFloodCapacity;

        private void Start()
        {
            if (_mapGridConfig == null) Debug.LogError("MapGridConfig is not assigned to BurrowManager");

            transform.position = _mapGridConfig.GridToWorld(_mapGridConfig.BurrowPosition);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<WaterReachedBurrowEvent>(OnWaterReachedBurrow);

            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<WaterReachedBurrowEvent>(OnWaterReachedBurrow);

            EventBus.Publish(new UnregisterManagerEvent(this.GetType()));
        }

        private void Update()
        {
            if (_inflowRate > 0)
            {
                UpdateFloodMeter(_inflowRate * Time.deltaTime);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.Restart)
            {
                ResetFloodMeter();
            }
        }

        private void OnWaterReachedBurrow(WaterReachedBurrowEvent @event)
        {
            _inflowRate += @event.InflowAmount;
        }

        public void UpdateFloodMeter(float amount)
        {
            _floodMeter += amount;
            _floodMeter = Mathf.Clamp(_floodMeter, 0, _maxFloodCapacity);

            EventBus.Publish(new BurrowFloodUpdatedEvent(_floodMeter, _maxFloodCapacity));

            // The player should have lost the game if the flood meter reaches the max flood capacity
            if (_floodMeter >= _maxFloodCapacity)
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.GameOver));
            }
        }

        public void ResetFloodMeter()
        {
            _floodMeter = 0;

            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }
    }
}