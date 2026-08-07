using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _currentGameState = GameState.StartMenu;
    [SerializeField] private int _currentWave = 0;

    private void OnEnable()
    {
        EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
        EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowWaterLevelChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
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
        if (@event.MaxWaterLevel == @event.CurrentWaterLevel)
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.GameOver));
        }
    }
}