using UnityEngine;

namespace DontWaterMyBurrow.Player.Events
{
    public readonly struct PlayerOpenedBuildMenuEvent
    {
        public readonly Vector2Int TargetCell;

        public PlayerOpenedBuildMenuEvent(Vector2Int targetCell)
        {
            TargetCell = targetCell;
        }
    }
}