using UnityEngine;

namespace DontWaterMyBurrow.Structures.Events
{
    public readonly struct StructureChangeCellEvent
    {
        public readonly Transform PusherTransform { get; }
        public readonly Vector2Int From { get; }
        public readonly Vector2Int To { get; }
        public readonly GameObject Structure;

        public StructureChangeCellEvent
        (
            Transform pusherTransform,
            Vector2Int from,
            Vector2Int to,
            GameObject structure
        )
        {
            PusherTransform = pusherTransform;
            From = from;
            To = to;
            Structure = structure;
        }
    }
}