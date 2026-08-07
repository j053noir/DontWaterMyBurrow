using UnityEngine;

public class BurrowManager : MonoBehaviour
{
    [SerializeField] private float _floodMeter = 0;
    [SerializeField] private float _inflowRate = 0;
    [SerializeField] private float _maxFloodCapacity = 100;

    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Subscribe<WaterReachedBurrowEvent>(OnWaterReachedBurrow);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Unsubscribe<WaterReachedBurrowEvent>(OnWaterReachedBurrow);
    }

    private void Update()
    {
        if (_inflowRate > 0)
        {
            UpdateFloodStatus(Time.deltaTime);
        }
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        if (@event.NewState == GameState.StartMenu || @event.NewState == GameState.GameOver)
        {
            ResetFloodMeter();
        }
    }

    private void OnWaterReachedBurrow(WaterReachedBurrowEvent @event)
    {
        _inflowRate += @event.InflowAmount;
    }

    private void UpdateFloodStatus(float deltaTime)
    {
        _floodMeter += _inflowRate * deltaTime;
        _floodMeter = Mathf.Clamp(_floodMeter, 0, _maxFloodCapacity);

        EventBus.Publish(new BurrowFloodUpdatedEvent(_floodMeter, _maxFloodCapacity));

        // The player should have lost the game if the flood meter reaches the max flood capacity
        if (_floodMeter >= _maxFloodCapacity)
        {
            EventBus.Publish(new GameStateChangedEvent(GameState.GameOver));
        }
    }

    public void ResetFloodMeter()
    {
        _floodMeter = 0;
    }
}