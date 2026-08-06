using UnityEngine;

public readonly struct ConfirmBuildEvent
{
    public readonly Vector2Int Position;

    public ConfirmBuildEvent(Vector2Int position)
    {
        Position = position;
    }
}