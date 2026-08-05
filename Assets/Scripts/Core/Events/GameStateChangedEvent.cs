public enum GameState
{
    StartMenu,
    GamePlay,
    WavePreparation,
    GameOver,
    Victory,
}

public struct GameStateChangedEvent
{
    public GameState NewState;

    public GameStateChangedEvent(GameState newState)
    {
        NewState = newState;
    }
}