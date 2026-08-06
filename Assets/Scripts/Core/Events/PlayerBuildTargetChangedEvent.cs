using UnityEngine;

public readonly struct PlayerBuildTargetChangedEvent
{
    public readonly Vector2Int TargetCell;

    public PlayerBuildTargetChangedEvent(Vector2Int targetCell)
    {
        TargetCell = targetCell;
    }
}