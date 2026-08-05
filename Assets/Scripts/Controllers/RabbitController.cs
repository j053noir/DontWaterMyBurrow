using System;
using UnityEngine;

public class RabbitController : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Clear();
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        if (@event.NewState == GameState.StartMenu || @event.NewState == GameState.GameOver || @event.NewState == GameState.Victory)
        {
            // TODO: Bloquear movimiento.
        }
        else if (@event.NewState == GameState.GamePlay || @event.NewState == GameState.WavePreparation)
        {
            // TODO: Habilitar movimiento.
        }
    }
}
