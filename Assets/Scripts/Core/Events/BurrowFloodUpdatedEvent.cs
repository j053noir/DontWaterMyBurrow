public readonly struct BurrowFloodUpdatedEvent
{
    public readonly float CurrentWaterLevel;
    public readonly float MaxWaterLevel;

    public BurrowFloodUpdatedEvent(float currentWaterLevel = 0, float maxWaterLevel = 100)
    {
        CurrentWaterLevel = currentWaterLevel;
        MaxWaterLevel = maxWaterLevel;
    }
}