using UnityEngine;

public enum HazardType
{
    Rock,
    Leaves,
    Log,
    Mud
}

public readonly struct HazardSpawnedEvent
{
    public readonly HazardType HazardType;
    public readonly Vector2Int Position;

    public HazardSpawnedEvent(HazardType hazardType, Vector2Int position)
    {
        HazardType = hazardType;
        Position = position;
    }
}