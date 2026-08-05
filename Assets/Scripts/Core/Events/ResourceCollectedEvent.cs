public struct ResourceCollectedEvent
{
    public string ResourceType;
    public int Quantity;

    public ResourceCollectedEvent(string resourceType, int quantity)
    {
        ResourceType = resourceType;
        Quantity = quantity;
    }
}