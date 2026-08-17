using UnityEngine;

namespace DontWaterMyBurrow.Player.Events
{
    public readonly struct PlayerOpenedBuilMenuEvent
    {
        public readonly Vector2Int TargetCell;

        public PlayerOpenedBuilMenuEvent(Vector2Int targetCell)
        {
            TargetCell = targetCell;
        }
    }
}