using DontWaterMyBurrow.Building;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Hazards;
using UnityEngine;
using System.Collections.Generic;

namespace DontWaterMyBurrow.Wave
{
    public class WaveManager : MonoBehaviour
    {
    [Header("Wave")]
    [SerializeField] private WaveDataSO _currentWaveData; // Use scriptable object
    [SerializeField] private float _timeUntilNextWave = 10f;
    [SerializeField] private bool _isWaveActive = false;

    [Header("References")]
    [SerializeField] private GridManager _gridManager;

    private float _waveTimer = 0f;
    private Dictionary<HazardsSpawnData, float> _hazardTimers = new();

    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        if (@event.NewState == GameState.WavePreparation)
        {
            StartWave();
        }
    }

    private void Update()
    {
        if (!_isWaveActive) return;

        _waveTimer -= Time.deltaTime;
        EventBus.Publish(new WaveTimerChangedEvent(_timeUntilNextWave, _waveTimer));

        if (_waveTimer <= 0)
        {
            EndWave();
            return;
        }

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
        _isWaveActive = true;
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
        _isWaveActive = false;
        EventBus.Publish(new WaveCompletedEvent());
    }

    public void SpawnThreat(HazardsSpawnData hazard)
    {
        var spawnPosition = new Vector3(Random.Range(_gridManager.MinXBoundary, _gridManager.MaxXBoundary), _gridManager.YBottomBoundary, 0);
        Instantiate(hazard.Prefab, spawnPosition, Quaternion.identity);
        var cellPosition = new Vector2Int(Mathf.RoundToInt(spawnPosition.x), Mathf.RoundToInt(spawnPosition.y));
        EventBus.Publish(new HazardSpawnedEvent(hazard.Type, cellPosition));
    }
}
}