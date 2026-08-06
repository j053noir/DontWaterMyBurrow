public enum GameState
{
    /// <summary>
    /// Main menu before start
    /// </summary>
    StartMenu,
    // <summary>
    /// Restarting game from pause state
    /// </summary>
    Restart,
    /// <summary>
    /// Main gameplay
    /// </summary>
    GamePlay,
    /// <summary>
    /// Game paused
    /// </summary>
    Pause,
    /// <summary>
    /// Waiting for next wave
    /// </summary>
    WavePreparation,
    /// <summary>
    /// Burrow flooded
    /// </summary>
    GameOver,
    /// <summary>
    /// Last wave survived
    /// </summary>
    Victory
}

public readonly struct GameStateChangedEvent
{
    public readonly GameState NewState;

    public GameStateChangedEvent(GameState newState)
    {
        NewState = newState;
    }
}