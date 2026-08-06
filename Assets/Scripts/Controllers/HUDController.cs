using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller that manages the HUD UI Document
/// </summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] private float _currentWaterLevel = 0;
    [SerializeField] private float _maxWaterLevel = 0;
    [SerializeField] private Dictionary<ResourceType, int> _resourceCounters = new();

    /// <summary>
    /// Reverse countdown until next wave starts
    /// </summary>
    [SerializeField] private float _timeUntilNextWave = 10;

    /// <summary>
    /// Reverse countdown until wave ends
    /// </summary>
    [SerializeField] private float _waveTimer = 0;

    private void Awake()
    {
        _resourceCounters.Add(ResourceType.Wood, 0);
        _resourceCounters.Add(ResourceType.Stone, 0);
        _resourceCounters.Add(ResourceType.Sand, 0);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ResourceChangedEvent>(OnResourceChanged);
        EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowFloodLevelChanged);
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Subscribe<WaveTimerChangedEvent>(OnWaveTimerChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ResourceChangedEvent>(OnResourceChanged);
        EventBus.Unsubscribe<BurrowFloodUpdatedEvent>(OnBurrowFloodLevelChanged);
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Unsubscribe<WaveTimerChangedEvent>(OnWaveTimerChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        switch (@event.NewState)
        {
            case GameState.StartMenu:
                Debug.Log("Show Start Menu UI Document");
                break;
            case GameState.WavePreparation:
                Debug.Log("Show Wave Preparation UI Document");
                break;
            case GameState.GamePlay:
                Debug.Log("Show HUD UI Document");
                break;
            case GameState.Pause:
                Debug.Log("Show Pause Menu UI Document");
                break;
            case GameState.GameOver:
                Debug.Log("Show Game Over UI Document");
                break;
            case GameState.Victory:
                Debug.Log("Show Victory UI Document");
                break;
            default:
                break;
        }
    }

    private void OnBurrowFloodLevelChanged(BurrowFloodUpdatedEvent @event)
    {
        _currentWaterLevel = @event.CurrentWaterLevel;
        _maxWaterLevel = @event.MaxWaterLevel;
    }

    private void OnResourceChanged(ResourceChangedEvent @event)
    {
        if (_resourceCounters.TryGetValue(@event.ResourceType, out int currentValue))
        {
            _resourceCounters[@event.ResourceType] = Mathf.Max(0, currentValue + @event.Quantity);
        }

        Debug.Log($"Updated resource counter: {_resourceCounters[@event.ResourceType]} of {@event.ResourceType}");
    }

    private void OnWaveTimerChanged(WaveTimerChangedEvent @event)
    {
        _timeUntilNextWave = @event.TimeUntilNextWave;
        _waveTimer = @event.WaveTimer;
    }
}
