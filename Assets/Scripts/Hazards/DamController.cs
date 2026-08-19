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
        [SerializeField] private int _size = 1;

        private void OnEnable()
        {
            var cellSize = GetCellSize();
            EventBus.Publish(new DamCreatedEvent(cellSize, gameObject));
        }

        private void OnDisable()
        {
            var cellSize = GetCellSize();
            EventBus.Publish(new DamDestroyedEvent(cellSize, gameObject));
        }

        public Vector2Int[] GetCellSize()
        {
            var centerCell = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y)
            );

            var directionVector = new Vector2Int(
                Mathf.RoundToInt(transform.right.x),
                Mathf.RoundToInt(transform.right.y)
            );

            switch (_size)
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
                    Debug.LogWarning($"Unexpected dam size: {_size}");
                    return Array.Empty<Vector2Int>();
            }
        }
    }
}