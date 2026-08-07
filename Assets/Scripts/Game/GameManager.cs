using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Game.Events;

namespace DontWaterMyBurrow.Game
{
    public enum GameState
    {
        StartMenu,
        WavePreparation,
        WaveActive,
        GamePlay,
        WaveCompleted,
        GameOver,
        Pause,
        Restart,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameState _currentGameState = GameState.StartMenu;
        [SerializeField] private int _currentWave = 0;

        private void OnEnable()
        {
            EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Unsubscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            _currentGameState = @event.NewState;
        }

        public void StartNextWave()
        {
            _currentWave++;
            EventBus.Publish(new WaveStartedEvent(_currentWave));
            EventBus.Publish(new GameStateChangedEvent(GameState.WavePreparation));
        }

        public void PauseGame()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.Pause));
        }

        public void GameRestart()
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.Restart));
        }

        public void OnWaveCompleted(WaveCompletedEvent @event)
        {
            if (@event.WaveNumber <= @event.MaxWaveNumber)
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.WavePreparation));
            }
            else
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.Victory));
            }
        }

        public void OnBurrowWaterLevelChanged(BurrowFloodUpdatedEvent @event)
        {
            if (@event.MaxFloodCapacity == @event.FloodMeter)
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.GameOver));
            }
        }
    }
}