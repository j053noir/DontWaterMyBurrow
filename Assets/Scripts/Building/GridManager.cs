using System;
using System.Collections.Generic;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Resources.Events;

namespace DontWaterMyBurrow.Building
{
    public enum GridCellType
    {
        Structure,
        Dam,
        Resource
    }

    public struct GridObject
    {
        public GameObject Instance;
        public GridCellType Type;

        public GridObject(GameObject instance, GridCellType type)
        {
            Instance = instance;
            Type = type;
        }
    }

    public class GridManager : MonoBehaviour
    {
    [SerializeField] private Vector2Int _burrowPosition;

    private Dictionary<Vector2Int, GridObject> _occupiedCells;

    [Header("Boundaries")]
    [SerializeField] private int _minXBoundary;
    [SerializeField] private int _maxXBoundary;
    [SerializeField] private int _yBottomBoundary;
    [SerializeField] private int _yTopBoundary;

    public Vector2Int BurrowPosition => _burrowPosition;

    public int MinXBoundary => _minXBoundary;
    public int MaxXBoundary => _maxXBoundary;
    public int YBottomBoundary => _yBottomBoundary;
    public int YTopBoundary => _yTopBoundary;

    private void Awake()
    {
        _occupiedCells = new();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<StructureBuiltEvent>(OnStructureBuilt);
        EventBus.Subscribe<StructureDestroyedEvent>(OnStructureDestroyed);
        EventBus.Subscribe<DamCreatedEvent>(OnDamCreated);
        EventBus.Subscribe<DamDestroyedEvent>(OnDamDestroyed);
        EventBus.Subscribe<ResourceSpawnedEvent>(OnResourceSpawned);
        EventBus.Subscribe<ResourceCollectedEvent>(OnResourceCollected);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);
        EventBus.Unsubscribe<StructureDestroyedEvent>(OnStructureDestroyed);
        EventBus.Unsubscribe<DamCreatedEvent>(OnDamCreated);
        EventBus.Unsubscribe<DamDestroyedEvent>(OnDamDestroyed);
        EventBus.Unsubscribe<ResourceSpawnedEvent>(OnResourceSpawned);
        EventBus.Unsubscribe<ResourceCollectedEvent>(OnResourceCollected);
    }

    private void OnStructureBuilt(StructureBuiltEvent @event)
    {
        var gridObject = new GridObject(@event.StructureInstance, GridCellType.Structure);
        _occupiedCells.Add(@event.Position, gridObject);
    }

    private void OnStructureDestroyed(StructureDestroyedEvent @event)
    {
        _occupiedCells.Remove(@event.Position);
    }

    private void OnDamCreated(DamCreatedEvent @event)
    {
        var gridObject = new GridObject(@event.Instance, GridCellType.Dam);
        _occupiedCells.Add(@event.Position, gridObject);
    }

    private void OnDamDestroyed(DamDestroyedEvent @event)
    {
        _occupiedCells.Remove(@event.Position);
    }

    private void OnResourceSpawned(ResourceSpawnedEvent @event)
    {
        var gridObject = new GridObject(@event.Instance, GridCellType.Resource);
        _occupiedCells.Add(@event.Position, gridObject);
    }

    private void OnResourceCollected(ResourceCollectedEvent @event)
    {
        _occupiedCells.Remove(@event.Position);
    }

    public bool IsCellOccupied(Vector2Int position)
    {
        return _occupiedCells.ContainsKey(position);
    }

    public bool IsCellDam(Vector2Int position)
    {
        return _occupiedCells.ContainsKey(position) && _occupiedCells[position].Type == GridCellType.Dam;
    }

    public bool IsCellOccupiedByStructure(Vector2Int position)
    {
        return _occupiedCells.ContainsKey(position) && _occupiedCells[position].Type == GridCellType.Structure;
    }
}
}