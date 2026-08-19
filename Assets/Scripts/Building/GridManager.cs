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
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private WorldGridDataSO _worldGridData;

        private void Awake()
        {
            _worldGridData.Reset();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);
            EventBus.Subscribe<DamCreatedEvent>(OnDamCreated);
            EventBus.Subscribe<DamDestroyedEvent>(OnDamDestroyed);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<ResourceCollectedEvent>(OnResourceCollected);
            EventBus.Subscribe<ResourceSpawnedEvent>(OnResourceSpawned);
            EventBus.Subscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Subscribe<StructureDestroyedEvent>(OnStructureDestroyed);

            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);
            EventBus.Unsubscribe<DamCreatedEvent>(OnDamCreated);
            EventBus.Unsubscribe<DamDestroyedEvent>(OnDamDestroyed);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<ResourceCollectedEvent>(OnResourceCollected);
            EventBus.Unsubscribe<ResourceSpawnedEvent>(OnResourceSpawned);
            EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Unsubscribe<StructureDestroyedEvent>(OnStructureDestroyed);

            EventBus.Publish(new UnregisterManagerEvent(this.GetType()));
        }

        private void OnBuildValidationRequested(BuildValidationRequestEvent @event)
        {
            if (_worldGridData.IsCellOccupied(@event.BuildPosition))
            {
                @event.Invalidate();
            }
        }

        private void OnDamCreated(DamCreatedEvent @event)
        {
            var gridObject = new GridObject(@event.Instance, GridCellType.Dam);

            foreach (var position in @event.OccupiedCells)
            {
                _worldGridData.SetCell(position, gridObject);
            }
        }

        private void OnDamDestroyed(DamDestroyedEvent @event)
        {
            foreach (var position in @event.OccupiedCells)
            {
                _worldGridData.RemoveCell(position);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.Restart)
            {
                ClearGrid();
            }
        }

        private void OnResourceSpawned(ResourceSpawnedEvent @event)
        {
            var gridObject = new GridObject(@event.Instance, GridCellType.Resource);
            _worldGridData.SetCell(@event.Position, gridObject);
        }

        private void OnResourceCollected(ResourceCollectedEvent @event)
        {
            _worldGridData.RemoveCell(@event.Position);
        }

        private void OnStructureBuilt(StructureBuiltEvent @event)
        {
            var gridObject = new GridObject(@event.StructureInstance, GridCellType.Structure);
            _worldGridData.SetCell(@event.Position, gridObject);
        }

        private void OnStructureDestroyed(StructureDestroyedEvent @event)
        {
            _worldGridData.RemoveCell(@event.Position);
        }

        private void ClearGrid()
        {
            _worldGridData.Reset();

            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }
    }
}