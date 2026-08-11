using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Game.Events;
using System;
using System.Collections.Generic;
using DontWaterMyBurrow.Game.States;

namespace DontWaterMyBurrow.Game
{
    public enum GameState
    {
        MainMenu,
        WavePreparation,
        WaveActive,
        GamePlay,
        //Resume,
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

        private HashSet<Type> _knownManagers;
        private HashSet<Type> _readyManagers;

        public StateMachine StateMachine;
        public GameOverState GameOverState { get; private set; }
        public MainMenuState MainMenuState { get; private set; }
        public PauseState PauseState { get; private set; }
        public RestartState RestartState { get; private set; }
        public VictoryState VictoryState { get; private set; }
        public WaveActiveState WaveActiveState { get; private set; }
        public WavePreparationState WavePreparationState { get; private set; }

        private void Awake()
        {
            _knownManagers = new();
            _readyManagers = new();

            StateMachine = new();

            GameOverState = new GameOverState();
            MainMenuState = new MainMenuState();
            PauseState = new PauseState();
            RestartState = new RestartState(this);
            VictoryState = new VictoryState();
            WaveActiveState = new WaveActiveState();
            WavePreparationState = new WavePreparationState(this);
        }

        private void Start()
        {
            StateMachine.ChangeState(MainMenuState);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Subscribe<RegisterManagerEvent>(OnRegisterManager);
            EventBus.Subscribe<UnregisterManagerEvent>(OnUnregisterManager);
            EventBus.Subscribe<ManagerReadyEvent>(OnManagerReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
            EventBus.Unsubscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
            EventBus.Unsubscribe<RegisterManagerEvent>(OnRegisterManager);
            EventBus.Unsubscribe<UnregisterManagerEvent>(OnUnregisterManager);
            EventBus.Unsubscribe<ManagerReadyEvent>(OnManagerReady);
        }

        private void OnWaveCompleted(WaveCompletedEvent @event)
        {
            if (@event.WaveNumber <= @event.MaxWaveNumber)
            {
                StateMachine.ChangeState(WavePreparationState);
            }
            else
            {
                StateMachine.ChangeState(VictoryState);
            }
        }

        private void OnBurrowWaterLevelChanged(BurrowFloodUpdatedEvent @event)
        {
            if (@event.MaxFloodCapacity == @event.FloodMeter)
            {
                StateMachine.ChangeState(GameOverState);
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
            StateMachine.ChangeState(WavePreparationState);
        }

        public void PauseGame()
        {
            StateMachine.ChangeState(PauseState);
        }

        public void GameRestart()
        {
            StateMachine.ChangeState(RestartState);
        }

        public void ResetGame()
        {
            _currentWave = 0;
            _readyManagers.Clear();
        }
    }
}