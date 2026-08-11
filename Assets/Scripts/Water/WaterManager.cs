using System.Collections.Generic;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Data;

namespace DontWaterMyBurrow.Water
{
    public class WaterManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;

        [Header("Water Grid")]
        [SerializeField] private Dictionary<Vector2Int, float> _waterGrid;
        [SerializeField] private Dictionary<Vector2Int, Vector2Int> _channelDirections;
        [SerializeField] private HashSet<Vector2Int> _drainCells;
        [SerializeField] private List<Vector2Int> _waterFlowVectors;

        [Header("Water Properties")]
        [SerializeField] private float _globalWaterPressure = 0.33f;

        private bool _isSimulatingWater = false;
        private HashSet<Vector2Int> _occupiedCells;

        private void Awake()
        {
            _waterGrid = new();
            _channelDirections = new();
            _drainCells = new();
            _occupiedCells = new();
            _waterFlowVectors = new()
        {
            Vector2Int.up,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
        };
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WaterDrainEvent>(OnWaterDrain);
            EventBus.Subscribe<ChannelBuiltEvent>(OnChannelBuilt);
            EventBus.Subscribe<RegisterWaterDrainEvent>(OnRegisterWaterDrain);
            EventBus.Subscribe<RemoveWaterDrainEvent>(OnRemoveWaterDrain);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);

            EventBus.Publish(new RegisterManagerEvent(this.GetType()));
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaterDrainEvent>(OnWaterDrain);
            EventBus.Unsubscribe<ChannelBuiltEvent>(OnChannelBuilt);
            EventBus.Unsubscribe<RegisterWaterDrainEvent>(OnRegisterWaterDrain);
            EventBus.Unsubscribe<RemoveWaterDrainEvent>(OnRemoveWaterDrain);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);

            EventBus.Publish(new UnregisterManagerEvent(this.GetType()));
        }

        private void Update()
        {
            UpdateWaterFlow(Time.deltaTime);
        }

        /// <summary>
        /// Reduces the water level in the water grid based on the drain amount and radius.
        /// </summary>
        /// <param name="@event">Event containing the drain position, amount, and radius</param>
        private void OnWaterDrain(WaterDrainEvent @event)
        {
            for (int x = -@event.DrainRadius; x <= @event.DrainRadius; x++)
            {
                for (int y = -@event.DrainRadius; y <= @event.DrainRadius; y++)
                {
                    var currentPosition = @event.Position + new Vector2Int(x, y);
                    if (_waterGrid.TryGetValue(currentPosition, out var level))
                    {
                        _waterGrid[currentPosition] = Mathf.Max(0, level - @event.DrainAmount);
                    }
                }
            }
        }

        private void OnChannelBuilt(ChannelBuiltEvent @event)
        {
            _channelDirections[@event.Position] = @event.Direction;
        }

        private void OnRegisterWaterDrain(RegisterWaterDrainEvent @event)
        {
            RegisterDrain(@event.Position, @event.DrainAmount, @event.DrainRadius);
        }

        private void OnRemoveWaterDrain(RemoveWaterDrainEvent @event)
        {
            _drainCells.Remove(@event.Position);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            _isSimulatingWater = @event.NewState == GameState.WaveActive;

            if (@event.NewState == GameState.Restart)
            {
                ResetWater();
            }
        }

        private void OnBuildValidationRequested(BuildValidationRequestEvent @event)
        {
            if (IsCellFlooded(@event.BuildPosition))
            {
                @event.Invalidate();
            }
        }

        private void UpdateWaterFlow(float deltaTime)
        {
            GenerateNewWater();

            // Iterate over a copy of the water grid because we are modifying the original
            var waterGrid = new Dictionary<Vector2Int, float>(_waterGrid);
            foreach (var waterKvP in waterGrid)
            {
                if (waterKvP.Value == 0)
                    continue;

                // If pressure is high enough, water will flow to adjacent cells
                if (waterKvP.Value >= 0.5)
                {
                    // If water reaches a drain, reduce water level
                    if (_drainCells.TryGetValue(waterKvP.Key, out var drainAmount))
                    {
                        _waterGrid[waterKvP.Key] -= _globalWaterPressure;
                    }
                    // If water is in a channel, move with the channel direction
                    else if (_channelDirections.TryGetValue(waterKvP.Key, out var channelDirection))
                    {
                        var nextPosition = waterKvP.Key + channelDirection;
                        MoveWater(waterKvP.Key, nextPosition);
                    }
                    else
                    {
                        // Try to move following water flow vectors
                        foreach (var flowVector in _waterFlowVectors)
                        {
                            var nextPosition = waterKvP.Key + flowVector;
                            if (MoveWater(waterKvP.Key, nextPosition))
                            {
                                break;
                            }
                        }
                    }
                }

                // Check if water has reached the burrow position
                if (waterKvP.Key == _mapGridConfig.BurrowPosition)
                {
                    var inflow = CheckWaterAtSurroundingCells(_mapGridConfig.BurrowPosition);
                    EventBus.Publish(new WaterReachedBurrowEvent(inflow));
                }
            }
        }

        public bool MoveWater(Vector2Int fromPosition, Vector2Int toPosition)
        {
            if (!IsCellOccupied(toPosition))
            {
                _waterGrid.TryGetValue(toPosition, out float currentToLevel);
                _waterGrid[toPosition] = Mathf.Clamp(currentToLevel + _globalWaterPressure, 0f, 1f);

                _waterGrid.TryGetValue(fromPosition, out float currentFromLevel);
                float newFromLevel = currentFromLevel - _globalWaterPressure;
                if (newFromLevel <= 0f)
                {
                    _waterGrid.Remove(fromPosition);
                }
                else
                {
                    _waterGrid[fromPosition] = newFromLevel;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds new water to the bottom of the grid if cell is available.
        /// </summary>
        private void GenerateNewWater()
        {
            if (!_isSimulatingWater) return;

            for (int i = _mapGridConfig.MinXBoundary; i < _mapGridConfig.MaxXBoundary; i++)
            {
                var position = new Vector2Int(i, _mapGridConfig.YBottomBoundary);
                if (!IsCellOccupied(position))
                {
                    _waterGrid.TryGetValue(position, out float currentLevel);
                    // Increase water level, but don't exceed 100%
                    _waterGrid[position] = Mathf.Clamp(currentLevel + _globalWaterPressure, 0f, 1f);
                }
            }
        }

        /// <summary>
        /// Checks how many of the 8 surrounding cells are flooded.
        /// </summary>
        /// <param name="centerPosition">Center position to check surrounding cells</param>
        /// <returns>Number of flooded surrounding cells</returns>
        private int CheckWaterAtSurroundingCells(Vector2Int centerPosition)
        {
            var count = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    var position = centerPosition + new Vector2Int(x, y);
                    if (IsCellFlooded(position))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void ResetWater()
        {
            _waterGrid.Clear();
            _channelDirections.Clear();
            _drainCells.Clear();
            EventBus.Publish(new ManagerReadyEvent(this.GetType()));
        }

        public bool IsCellOccupied(Vector2Int position)
        {
            return _occupiedCells.Contains(position);
        }

        public bool IsCellFlooded(Vector2Int gridPosition)
        {
            return _waterGrid.TryGetValue(gridPosition, out var level) && level >= 0.05;
        }

        public void RegisterChannel(Vector2Int gridPosition, Vector2 direction)
        {
            _channelDirections[gridPosition] = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
        }

        public void RegisterDrain(Vector2Int gridPosition, float amount = 1, int radius = 1)
        {
            _drainCells.Add(gridPosition);
            EventBus.Publish(new WaterDrainEvent(gridPosition, amount, radius));
        }
    }
}