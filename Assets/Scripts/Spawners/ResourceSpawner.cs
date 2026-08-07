using System.Collections.Generic;
using UnityEngine;

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

    [Header("References")]
    [SerializeField] private GridManager _gridManager;

    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);

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
            Random.Range(_gridManager.MinXBoundary, _gridManager.MaxXBoundary), // X within boundaries
            Random.Range(_gridManager.YBottomBoundary, _gridManager.YTopBoundary) // Y within boundaries
        );

        while (_gridManager.IsCellOccupied(randomPosition))
        {
            // Keep generating random positions until an empty cell is found
            randomPosition = new Vector2Int(Random.Range(_gridManager.MinXBoundary, _gridManager.MaxXBoundary), Random.Range(_gridManager.YBottomBoundary, _gridManager.YTopBoundary));
        }

        return randomPosition;
    }
}