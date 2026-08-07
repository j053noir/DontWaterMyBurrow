public enum ResourceType
{
    Wood,
    Stone,
    Sand,
}

namespace DontWaterMyBurrow.Resources.Events
{
    public readonly struct ResourceChangedEvent
    {
        public readonly ResourceType ResourceType;
        public readonly int CurrentAmount;
        public readonly int Quantity;

        public ResourceChangedEvent(ResourceType resourceType, int currentAmount, int quantity = 0)
        {
            ResourceType = resourceType;
            CurrentAmount = currentAmount;
            Quantity = quantity;
        }
    }
}