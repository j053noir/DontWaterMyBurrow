using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Game.Events;
using System;
using System.Collections.Generic;

namespace DontWaterMyBurrow.Game
{
    public enum GameState
    {
        MainMenu,
        WavePreparation,
        WaveActive,
        GamePlay,
        Resume,
        WaveCompleted,
        GameOver,
        Pause,
        Restart,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameState _currentGameState = GameState.MainMenu;
        [SerializeField] private int _currentWave = 0;
        public GameState CurrentGameState => _currentGameState;
        public int CurrentWave => _currentWave;

        private HashSet<Type> _knownManagers;
        private HashSet<Type> _readyManagers;

        private void Awake()
        {
            _knownManagers = new();
            _readyManagers = new();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<RegisterManagerEvent>(OnRegisterManager);
            EventBus.Subscribe<UnregisterManagerEvent>(OnUnregisterManager);
            EventBus.Subscribe<ManagerReadyEvent>(OnManagerReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Unsubscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<RegisterManagerEvent>(OnRegisterManager);
            EventBus.Unsubscribe<UnregisterManagerEvent>(OnUnregisterManager);
            EventBus.Unsubscribe<ManagerReadyEvent>(OnManagerReady);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            Time.timeScale = @event.NewState == GameState.Pause || @event.NewState == GameState.Restart || @event.NewState == GameState.GameOver || @event.NewState == GameState.Victory ? 0 : 1;

            _currentGameState = @event.NewState;

            if (@event.NewState == GameState.Restart) ResetGame();
        }

        private void ResetGame()
        {
            _currentWave = 0;
            _readyManagers.Clear();
        }

        private void OnRegisterManager(RegisterManagerEvent @event)
        {
            _knownManagers.Add(@event.ManagerType);
        }

        private void OnUnregisterManager(UnregisterManagerEvent @event)
        {
            _knownManagers.Remove(@event.ManagerType);
        }

        private void OnManagerReady(ManagerReadyEvent @event)
        {
            _readyManagers.Add(@event.ManagerType);

            if (_readyManagers.Count == _knownManagers.Count && _currentGameState == GameState.Restart)
            {
                StartNextWave();
            }
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