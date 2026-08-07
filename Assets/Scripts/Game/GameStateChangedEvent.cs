namespace DontWaterMyBurrow.Game.Events
{
    public readonly struct GameStateChangedEvent
    {
        public readonly GameState NewState;

        public GameStateChangedEvent(GameState newState)
        {
            NewState = newState;
        }
    }
}