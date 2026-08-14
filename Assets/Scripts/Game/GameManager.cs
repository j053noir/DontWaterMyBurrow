using System;
using System.Collections.Generic;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Game.States;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Wave.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Game
{
    public enum GameState
    {
        MainMenu,
        WavePreparation,
        WaveActive,
        WaveCompleted,
        GameOver,
        Pause,
        Restart,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _currentWave = 0;
        public int CurrentWave => _currentWave;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;
        public bool DebugMode => _debugMode;

        private HashSet<Type> _knownManagers;
        private HashSet<Type> _readyManagers;

        public StateMachine StateMachine;
        Dictionary<GameState, IState> _gameStateMap;

        private void Awake()
        {
            _knownManagers = new();
            _readyManagers = new();


            StateMachine = new();

            _gameStateMap = new()
            {
                { GameState.MainMenu, new MainMenuState()},
                { GameState.WavePreparation, new WavePreparationState(this)},
                { GameState.WaveActive, new WaveActiveState()},
                { GameState.GameOver, new GameOverState()},
                { GameState.Pause, new PauseState()},
                { GameState.Restart, new RestartState(this)},
                { GameState.Victory, new VictoryState()},
            };
        }

        private void Start()
        {
            StateMachine.ChangeState(_gameStateMap[GameState.MainMenu]);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStartEvent>(OnGameStart);
            EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<RegisterManagerEvent>(OnRegisterManager);
            EventBus.Subscribe<UnregisterManagerEvent>(OnUnregisterManager);
            EventBus.Subscribe<ManagerReadyEvent>(OnManagerReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartEvent>(OnGameStart);
            EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Unsubscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<RegisterManagerEvent>(OnRegisterManager);
            EventBus.Unsubscribe<UnregisterManagerEvent>(OnUnregisterManager);
            EventBus.Unsubscribe<ManagerReadyEvent>(OnManagerReady);
        }

        private void OnGameStart(GameStartEvent @event)
        {
            StateMachine.ChangeState(_gameStateMap[GameState.WavePreparation]);
        }

        private void OnWaveCompleted(WaveCompletedEvent @event)
        {
            if (@event.WaveNumber <= @event.MaxWaveNumber)
            {
                StateMachine.ChangeState(_gameStateMap[GameState.WavePreparation]);
            }
            else
            {
                StateMachine.ChangeState(_gameStateMap[GameState.Victory]);
            }
        }

        private void OnBurrowWaterLevelChanged(BurrowFloodUpdatedEvent @event)
        {
            if (@event.MaxFloodCapacity == @event.FloodMeter)
            {
                StateMachine.ChangeState(_gameStateMap[GameState.GameOver]);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (_gameStateMap.TryGetValue(@event.NewState, out var targetState) &&
                StateMachine.CurrentState != targetState)
            {
                StateMachine.ChangeState(targetState);
            }
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

            if (_readyManagers.Count == _knownManagers.Count && StateMachine.CurrentState is RestartState)
            {
                StartNextWave();
            }
        }

        public void StartNextWave()
        {
            _currentWave++;
            EventBus.Publish(new WaveStartedEvent(_currentWave));
            StateMachine.ChangeState(_gameStateMap[GameState.WavePreparation]);
        }

        public void PauseGame()
        {
            StateMachine.ChangeState(_gameStateMap[GameState.Pause]);
        }

        public void GameRestart()
        {
            StateMachine.ChangeState(_gameStateMap[GameState.Restart]);
        }

        public void ResetGame()
        {
            _currentWave = 0;
            _readyManagers.Clear();
        }
    }
}