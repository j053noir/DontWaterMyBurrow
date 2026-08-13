using System.Collections.Generic;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Resources.Events;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Data;

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
        private Dictionary<Vector2Int, GridObject> _occupiedCells;
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        public int MinXBoundary => _mapGridConfig.MinXBoundary;
        public int MaxXBoundary => _mapGridConfig.MaxXBoundary;
        public int YBottomBoundary => _mapGridConfig.MinYBoundary;
        public int YTopBoundary => _mapGridConfig.MaxYBoundary;

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
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);

            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Unsubscribe<StructureDestroyedEvent>(OnStructureDestroyed);
            EventBus.Unsubscribe<DamCreatedEvent>(OnDamCreated);
            EventBus.Unsubscribe<DamDestroyedEvent>(OnDamDestroyed);
            EventBus.Unsubscribe<ResourceSpawnedEvent>(OnResourceSpawned);
            EventBus.Unsubscribe<ResourceCollectedEvent>(OnResourceCollected);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);

            EventBus.Publish(new UnregisterManagerEvent(this.GetType()));
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

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.Restart)
            {
                ClearGrid();
            }
        }

        private void ClearGrid()
        {
            foreach (var item in _occupiedCells)
            {
                // TODO: Return instance to pool
                Destroy(item.Value.Instance);
            }

            _occupiedCells.Clear();

            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }

        private void OnBuildValidationRequested(BuildValidationRequestEvent @event)
        {
            if (IsCellOccupied(@event.BuildPosition))
            {
                @event.Invalidate();
            }
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