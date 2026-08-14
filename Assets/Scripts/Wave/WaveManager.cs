using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Wave.Events;
using UnityEngine;
using System.Collections.Generic;
using DontWaterMyBurrow.Wave.States;

namespace DontWaterMyBurrow.Wave
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave")]
        [SerializeField] private WaveDataSO _firstWaveData;
        [SerializeField] private WaveDataSO _currentWaveData;
        [SerializeField] private float _timeUntilNextWave;
        [SerializeField] private float _defaultTimeUntilNextWave = 30;


        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Debug")]
        [SerializeField] public bool debugMode = false;

        private float _waveTimer = 0f;
        private Dictionary<HazardsSpawnData, float> _hazardTimers;

        private StateMachine _stateMachine;
        public ActiveWaveState ActiveWaveState { get; private set; }
        public NextWaveState NextWaveState { get; private set; }

        private void Awake()
        {
            _hazardTimers = new();

            _stateMachine = new StateMachine();

            ActiveWaveState = new ActiveWaveState();
            NextWaveState = new NextWaveState(this);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Publish(new UnregisterManagerEvent(this.GetType()));
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.WavePreparation)
            {
                _stateMachine.ChangeState(NextWaveState);
            }
            else if (@event.NewState == GameState.WaveActive)
            {
                _stateMachine.ChangeState(ActiveWaveState);
            }
            else if (@event.NewState == GameState.Restart)
            {
                ResetWave();
            }
        }

        private void Update()
        {
            if (_currentWaveData == null) return;

            if (_stateMachine.CurrentState == NextWaveState)
            {
                _timeUntilNextWave = Mathf.Max(0, _timeUntilNextWave - Time.deltaTime);

                if (_timeUntilNextWave <= 0)
                {
                    EventBus.Publish(new GameStateChangedEvent(GameState.WaveActive));
                }
            }
            else if (_stateMachine.CurrentState == ActiveWaveState)
            {
                _waveTimer = Mathf.Max(0, _waveTimer - Time.deltaTime);

                if (_waveTimer <= 0)
                {
                    EndWave();
                }
                else
                {
                    SpawnHazards();
                }
            }

            EventBus.Publish(new WaveTimerChangedEvent(_currentWaveData.WaveNumber, _timeUntilNextWave, _waveTimer));
        }

        private void ResetWave()
        {
            _currentWaveData = _firstWaveData;
            _timeUntilNextWave = _defaultTimeUntilNextWave;
            _waveTimer = 0f;
            _hazardTimers.Clear();
            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }

        private void SpawnHazards()
        {
            // Spawn hazards based on their intervals
            foreach (var hazard in _currentWaveData.HazardsToSpawn)
            {
                _hazardTimers[hazard] += Time.deltaTime;
                if (_hazardTimers[hazard] >= hazard.SpawnInterval)
                {
                    SpawnThreat(hazard);
                    _hazardTimers[hazard] = 0f;
                }
            }
        }

        public void StartWave()
        {
            _timeUntilNextWave = _defaultTimeUntilNextWave;
            _waveTimer = _currentWaveData.WaveDuration;
            _hazardTimers.Clear();

            foreach (var hazard in _currentWaveData.HazardsToSpawn)
            {
                _hazardTimers[hazard] = 0f;
            }

            EventBus.Publish(new WaveStartedEvent(_currentWaveData.WaveNumber));
        }

        public void EndWave()
        {
            EventBus.Publish(new WaveCompletedEvent());
        }

        public void SpawnThreat(HazardsSpawnData hazard)
        {
            if (hazard.Prefab == null)
            {
                Debug.LogError("[WaveManager] Cannot spawn threat: Hazard Prefab is null!");
                return;
            }

            if (_mapGridConfig == null)
            {
                Debug.LogError("[WaveManager] _mapGridConfig is not assigned in the Inspector!");
                return;
            }

            var spawnPosition = new Vector3(Random.Range(_mapGridConfig.MinXBoundary, _mapGridConfig.MaxXBoundary), _mapGridConfig.MinYBoundary, 0);
            Instantiate(hazard.Prefab, spawnPosition, Quaternion.identity);
            var cellPosition = new Vector2Int(Mathf.RoundToInt(spawnPosition.x), Mathf.RoundToInt(spawnPosition.y));
            EventBus.Publish(new HazardSpawnedEvent(hazard.Type, cellPosition));
        }
    }
}