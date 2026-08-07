using System;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Structures.Events;

namespace DontWaterMyBurrow.Hazards
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class DamController : MonoBehaviour
    {
        [SerializeField] private int _lenght = 1;

        private void OnEnable()
        {
            var occupiedCells = GetOccupiedCells();
            EventBus.Publish(new DamCreatedEvent(occupiedCells[0], gameObject));
        }

        private void OnDisable()
        {
            var occupiedCells = GetOccupiedCells();
            EventBus.Publish(new DamDestroyedEvent(occupiedCells[0], gameObject));
        }

    public Vector2Int[] GetOccupiedCells()
    {
        var centerCell = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        var directionVector = new Vector2Int(
            Mathf.RoundToInt(transform.right.x),
            Mathf.RoundToInt(transform.right.y)
        );

        switch (_lenght)
        {
            case 1: return new Vector2Int[] { centerCell };
            case 2:
                return new Vector2Int[]
            {
                centerCell,
                centerCell + directionVector
            };
            case 3:
                return new Vector2Int[]
            {
                centerCell,
                centerCell + directionVector,
                centerCell + directionVector * 2
            };
            case 4:
                return new Vector2Int[]
            {
                centerCell,
                centerCell + directionVector,
                centerCell + directionVector * 2,
                centerCell + directionVector * 3
            };
            case 5:
                return new Vector2Int[]
            {
                centerCell,
                centerCell + directionVector,
                centerCell + directionVector * 2,
                centerCell + directionVector * 3,
                centerCell + directionVector * 4
            };
            default:
                Debug.LogWarning($"Unexpected dam lenght: {_lenght}");
                return Array.Empty<Vector2Int>();
        }
    }
}
}