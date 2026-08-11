using System;
using System.Collections.Generic;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Building;
using DontWaterMyBurrow.Resources.Events;
using Random = UnityEngine.Random;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures.Events;

namespace DontWaterMyBurrow.Resources
{
    [System.Serializable]
    public class SpawnConfig
    {
        public ResourceType resourceType;
        public int maxResources;
        public GameObject resourcePrefab;
    }

    public class ResourceSpawner : MonoBehaviour
    {
        [Header("Spawn Configs")]
        [SerializeField] private List<SpawnConfig> _spawnConfigs;

        [Header("Dependencies")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        private HashSet<Vector2Int> _occupiedCells;

        private void Awake()
        {
            _occupiedCells = new();
        }


        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Subscribe<StructureDestroyedEvent>(OnStructureDestroyed);
            EventBus.Subscribe<ResourceSpawnedEvent>(OnResourceSpawned);
            EventBus.Subscribe<ResourceCollectedEvent>(OnResourceCollected);
            EventBus.Subscribe<DamCreatedEvent>(OnDamCreated);
            EventBus.Subscribe<DamDestroyedEvent>(OnDamDestroyed);

            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Unsubscribe<StructureDestroyedEvent>(OnStructureDestroyed);
            EventBus.Unsubscribe<ResourceSpawnedEvent>(OnResourceSpawned);
            EventBus.Unsubscribe<ResourceCollectedEvent>(OnResourceCollected);
            EventBus.Unsubscribe<DamCreatedEvent>(OnDamCreated);
            EventBus.Unsubscribe<DamDestroyedEvent>(OnDamDestroyed);

            EventBus.Publish(new UnregisterManagerEvent(this.GetType()));
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.WavePreparation)
            {
                for (int i = 0; i < _spawnConfigs.Count; i++)
                {
                    SpawnResource(_spawnConfigs[i]);
                }
            }
            else if (@event.NewState == GameState.Restart)
            {
                ClearResources();
            }
        }

        private void ClearResources()
        {
            _occupiedCells.Clear();

            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }

        private void OnStructureBuilt(StructureBuiltEvent @event)
        {
            _occupiedCells.Add(@event.Position);
        }

        private void OnStructureDestroyed(StructureDestroyedEvent @event)
        {
            _occupiedCells.Remove(@event.Position);
        }

        private void OnResourceSpawned(ResourceSpawnedEvent @event)
        {
            _occupiedCells.Add(@event.Position);
        }

        private void OnResourceCollected(ResourceCollectedEvent @event)
        {
            _occupiedCells.Remove(@event.Position);
        }

        private void OnDamCreated(DamCreatedEvent @event)
        {
            _occupiedCells.Add(@event.Position);
        }

        private void OnDamDestroyed(DamDestroyedEvent @event)
        {
            _occupiedCells.Remove(@event.Position);
        }

        private void SpawnResource(SpawnConfig spawnConfig)
        {
            for (int i = 0; i < spawnConfig.maxResources; i++)
            {
                var randomPosition = GetRandomPosition();
                var worldPosition = new Vector3(randomPosition.x, randomPosition.y, 0);
                var resourceObject = Instantiate(spawnConfig.resourcePrefab, worldPosition, Quaternion.identity);

                EventBus.Publish(new ResourceSpawnedEvent(resourceObject, randomPosition, spawnConfig.resourceType));
            }
        }

        private Vector2Int GetRandomPosition()
        {
            var randomPosition = new Vector2Int(
                Random.Range(_mapGridConfig.MinXBoundary, _mapGridConfig.MaxXBoundary), // X within boundaries
                Random.Range(_mapGridConfig.YBottomBoundary, _mapGridConfig.YTopBoundary) // Y within boundaries
            );

            while (_occupiedCells.Contains(randomPosition))
            {
                // Keep generating random positions until an empty cell is found
                randomPosition = new Vector2Int(
                    Random.Range(_mapGridConfig.MinXBoundary, _mapGridConfig.MaxXBoundary),
                    Random.Range(_mapGridConfig.YBottomBoundary, _mapGridConfig.YTopBoundary)
                );
            }

            return randomPosition;
        }
    }
}