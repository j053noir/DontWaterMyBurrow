namespace DontWaterMyBurrow.Water.Events
{
    public readonly struct WaterReachedBurrowEvent
    {
        public readonly float InflowAmount;

        public WaterReachedBurrowEvent(float inflowAmount = 1f)
        {
            InflowAmount = inflowAmount;
        }
    }
}