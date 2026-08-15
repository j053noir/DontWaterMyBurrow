using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Wave.Events;
using UnityEngine;
using System.Collections.Generic;
using DontWaterMyBurrow.Wave.States;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Hazards;

namespace DontWaterMyBurrow.Wave
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave")]
        [SerializeField] private WaveDataSO _firstWaveData;
        [SerializeField] private WaveDataSO _currentWaveData;
        [SerializeField] private List<Vector2Int> _currentLeekingPositions;
        [SerializeField] private float _timeUntilNextWave;
        [SerializeField] private float _defaultTimeUntilNextWave = 30;
        [SerializeField] private Transform _hazardsParent;


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
            _currentLeekingPositions = new();

            _stateMachine = new StateMachine();

            ActiveWaveState = new ActiveWaveState(this);
            NextWaveState = new NextWaveState(this);

            if (_hazardsParent == null) _hazardsParent = transform;
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
                    SpawnHazard(hazard);
                    _hazardTimers[hazard] = 0f;
                }
            }
        }

        public void StartWave()
        {
            _timeUntilNextWave = _defaultTimeUntilNextWave;
            _waveTimer = _currentWaveData.WaveDuration;
            _currentLeekingPositions.Clear();
            _hazardTimers.Clear();

            foreach (var hazard in _currentWaveData.HazardsToSpawn)
            {
                _hazardTimers[hazard] = 0f;
            }

            EventBus.Publish(new WaveStartedEvent(_currentWaveData.WaveNumber));
        }

        public void EndWave()
        {
            _currentLeekingPositions.Clear();
            EventBus.Publish(new WaveCompletedEvent());
        }

        public void SpawnHazard(HazardsSpawnData hazard)
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

            if (_currentLeekingPositions == null || _currentLeekingPositions.Count == 0)
            {
                Debug.LogError("[WaveManager] _currentLeekingPositions is empty!");
                return;
            }

            var choice = Random.Range(0, _currentLeekingPositions.Count);
            var spawnPosition = _mapGridConfig.GridToWorld(_currentLeekingPositions[choice]);
            var hazardInstance = Instantiate(hazard.Prefab, spawnPosition, Quaternion.identity);
            if (debugMode && hazardInstance.TryGetComponent(out HazardsController controller))
            {
                controller.EnableDebugMode();
            }
            hazardInstance.transform.parent = _hazardsParent;
            var cellPosition = _currentLeekingPositions[choice];
            EventBus.Publish(new HazardSpawnedEvent(hazard.Type, cellPosition));
        }

        public void RegisterWaterLeaks()
        {
            foreach (var leakPosition in _currentWaveData.WaterLeekingPositions)
            {
                if (!_mapGridConfig.IsWithinBounds(leakPosition))
                {
                    if (debugMode) Debug.LogWarning($"[WaveManager] Cannot register water leak: Position {leakPosition} is outside the grid bounds!");
                    continue;
                }
                _currentLeekingPositions.Add(leakPosition);
                EventBus.Publish(new RegisterWaterLeakEvent(leakPosition));
            }
        }

        public void RemoveWaterLeaks()
        {
            foreach (var leakPosition in _currentLeekingPositions)
            {
                EventBus.Publish(new RemoveWaterLeakEvent(leakPosition));
            }

            _currentLeekingPositions.Clear();
        }
    }
}