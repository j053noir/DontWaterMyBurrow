namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct BurrowFloodUpdatedEvent
    {
        public readonly float FloodMeter;
        public readonly float MaxFloodCapacity;

        public BurrowFloodUpdatedEvent(float floodMeter, float maxFloodCapacity)
        {
            FloodMeter = floodMeter;
            MaxFloodCapacity = maxFloodCapacity;
        }
    }
}