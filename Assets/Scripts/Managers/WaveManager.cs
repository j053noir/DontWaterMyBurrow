using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave")]
    [SerializeField] private WaveDataSO _currentWaveData; // Use scriptable object
    [SerializeField] private float _timeUntilNextWave = 10f;
    [SerializeField] private float _waveTimer;
    [SerializeField] private bool _isWaveActive;
    [SerializeField] private Dictionary<HazardType, float> _spawnTimer;

    [Header("References")]
    [SerializeField] private GridManager _gridManager;

    private void Awake()
    {
        _spawnTimer = new();

        foreach (var hazardSpawnData in _currentWaveData.HazardsToSpawn)
        {
            _spawnTimer[hazardSpawnData.Type] = hazardSpawnData.SpawnInterval;
        }
    }

    public void LoadWave(int waveIndex, int totalWaves = 3, float waveDuration = 300f)
    {
        EventBus.Publish(new WaveStartedEvent(waveIndex, totalWaves, waveDuration));
    }

    private void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (_timeUntilNextWave > 0)
        {
            _timeUntilNextWave -= Time.deltaTime;
        }
        else
        {
            UpdateWaveTimer(Time.deltaTime);
        }

        EventBus.Publish(new WaveTimerChangedEvent(_timeUntilNextWave, _waveTimer));
    }

    public void UpdateWaveTimer(float deltaTime)
    {
        if (!_isWaveActive) return;

        _waveTimer -= deltaTime;

        foreach (var hazardSpawnData in _currentWaveData.HazardsToSpawn)
        {
            _spawnTimer[hazardSpawnData.Type] -= deltaTime;
            if (_spawnTimer[hazardSpawnData.Type] <= 0)
            {
                SpawnThreat(hazardSpawnData);
                _spawnTimer[hazardSpawnData.Type] = hazardSpawnData.SpawnInterval;
            }
        }

        if (_waveTimer <= 0)
        {
            EndWave();
            _timeUntilNextWave = 10f;
        }
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