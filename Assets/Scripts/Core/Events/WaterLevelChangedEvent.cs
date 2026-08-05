public struct WaterLevelChangedEvent
{
    public float CurrentWaterLevel;
    public float MaxWaterLevel;

    public WaterLevelChangedEvent(float currentWaterLevel, float maxWaterLevel = 100)
    {
        CurrentWaterLevel = currentWaterLevel;
        MaxWaterLevel = maxWaterLevel;
    }
}