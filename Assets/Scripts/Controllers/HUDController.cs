using System;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private float _currentWaterLevel = 0;
    private float _maxWaterLevel = 0;
    private int _woodAmount = 0;
    private int _stoneAmount = 0;

    private void OnEnable()
    {
        EventBus.Subscribe<ResourceCollectedEvent>(OnResourceCollected);
        EventBus.Subscribe<WaterLevelChangedEvent>(OnWaterLevelChanged);
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Clear();
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        if (@event.NewState == GameState.GamePlay)
        {
            // TODO: Show HUD UI Document       
        }
        else
        {
            // TODO: Hide HUD UI Document
        }
    }

    private void OnWaterLevelChanged(WaterLevelChangedEvent @event)
    {
        _currentWaterLevel += @event.CurrentWaterLevel;
        _maxWaterLevel += @event.MaxWaterLevel;
    }

    private void OnResourceCollected(ResourceCollectedEvent @event)
    {
        // TODO: Esto no es escalable XD
        if (@event.ResourceType == "Madera")
        {
            _woodAmount += @event.Quantity;
        }
        else if (@event.ResourceType == "Piedra")
        {
            _stoneAmount += @event.Quantity;
        }
    }
}
