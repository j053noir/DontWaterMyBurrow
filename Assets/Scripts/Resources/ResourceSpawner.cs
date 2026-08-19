using System.Collections.Generic;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Resources.Events;
using UnityEngine;

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
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private WorldGridDataSO _worldGridData;

        [Header("Parameters")]
        [SerializeField] private List<SpawnConfig> _spawnConfigs;
        [SerializeField] private Transform _resourcesParent;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;

        private void Awake()
        {
            if (_resourcesParent == null) _resourcesParent = transform;
        }


        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);

            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);

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
            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }

        private void SpawnResource(SpawnConfig spawnConfig)
        {
            for (int i = 0; i < spawnConfig.maxResources; i++)
            {
                var randomPosition = GetRandomPosition();
                var worldPosition = _mapGridConfig.GridToWorld(randomPosition);
                var resourceObject = Instantiate(spawnConfig.resourcePrefab, worldPosition, Quaternion.identity, _resourcesParent);

                EventBus.Publish(new ResourceSpawnedEvent(resourceObject, randomPosition, spawnConfig.resourceType));
            }
        }

        private Vector2Int GetRandomPosition()
        {
            if (_mapGridConfig == null)
            {
                if (_debugMode) Debug.LogError("[ResourceSpawner] _mapGridConfig is not assigned in the Inspector!");
                return Vector2Int.zero;
            }

            var randomPosition = GenerateRandomPosition();

            int attempts = 0;
            int maxAttempts = 100;
            while (_worldGridData.IsCellOccupied(randomPosition) && attempts < maxAttempts)
            {
                attempts++;
                // Keep generating random positions until an empty cell is found
                randomPosition = GenerateRandomPosition();
            }

            return randomPosition;
        }

        private Vector2Int GenerateRandomPosition()
        {
            var randomPosition = new Vector2Int(
                Random.Range(_mapGridConfig.MinXBoundary + 1, _mapGridConfig.MaxXBoundary - 1), // X within boundaries
                Random.Range(_mapGridConfig.MinYBoundary + 1, _mapGridConfig.MaxYBoundary - 1) // Y within boundaries
            );

            return randomPosition;
        }
    }
}