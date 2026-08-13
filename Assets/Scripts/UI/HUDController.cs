using System;
using System.Collections.Generic;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Resources.Events;
using DontWaterMyBurrow.Water.Events;
using DontWaterMyBurrow.Wave.Events;
using UnityEngine.UIElements;
using System.Linq;

namespace DontWaterMyBurrow.UI
{
    /// <summary>
    /// Helper class to represent a resource counter in the HUD
    /// </summary>
    [Serializable]
    public class ResourceCounter
    {
        public ResourceType ResourceType;
        public int Quantity;
    }

    /// <summary>
    /// Helper class to represent a resource counter in the HUD
    /// </summary>
    [Serializable]
    public class ResourceUILabels
    {
        public string ResourceType;
        public Label ResourceLabel;
    }

    /// <summary>
    /// Controller that manages the HUD UI Document
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class HUDController : MonoBehaviour
    {
        /// <summary>
        /// Reverse countdown until next wave starts
        /// </summary>
        [SerializeField] private float _timeUntilNextWave = 10;

        /// <summary>
        /// Reverse countdown until wave ends
        /// </summary>
        [SerializeField] private float _waveTimer = 0;

        /// <summary>
        /// The current wave number
        /// </summary>
        [SerializeField] private int _waveNumber = 0;

        [Header("Config")]
        [SerializeField] private float _currentWaterLevel = 0;
        [SerializeField] private float _maxWaterLevel = 0;

        [Header("UI Components")]
        [SerializeField] private UIDocument _hudDocument;
        [SerializeField] private VisualElement _root;

        [Header("Resource Counters")]
        private List<ResourceCounter> _resourceCounters = new();
        private List<ResourceUILabels> _resourceUILabels;

        [Header("Wave Info")]
        private Label _waveNumberLabel;
        private VisualElement _waveTimerWrapper;
        private Label _waveTimerLabel;
        private VisualElement _nextWaveTimerWrapper;
        private Label _nextWaveTimerLabel;

        [Header("Flood Level")]
        private ProgressBar _floodProgressBar;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;

        private void Awake()
        {
            _resourceCounters = new();

            // Add a resource counter for each resource type
            foreach (var type in Enum.GetValues(typeof(ResourceType)))
            {
                _resourceCounters.Add(new ResourceCounter { ResourceType = (ResourceType)type, Quantity = 0 });
            }
        }

        private void OnEnable()
        {
            InitializeHUD();

            EventBus.Subscribe<ResourceChangedEvent>(OnResourceChanged);
            EventBus.Subscribe<BurrowFloodUpdatedEvent>(OnBurrowFloodLevelChanged);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<WaveTimerChangedEvent>(OnWaveTimerChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ResourceChangedEvent>(OnResourceChanged);
            EventBus.Unsubscribe<BurrowFloodUpdatedEvent>(OnBurrowFloodLevelChanged);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<WaveTimerChangedEvent>(OnWaveTimerChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            ToggleHUDElements(@event.NewState);
        }

        private void OnBurrowFloodLevelChanged(BurrowFloodUpdatedEvent @event)
        {
            _currentWaterLevel = @event.FloodMeter;
            _maxWaterLevel = @event.MaxFloodCapacity;

            UpdateFloodLevelBar(_currentWaterLevel, _maxWaterLevel);
        }

        private void OnResourceChanged(ResourceChangedEvent @event)
        {
            var resource = _resourceCounters.FirstOrDefault(r => r.ResourceType == @event.ResourceType);
            if (resource != null)
            {
                resource.Quantity = @event.CurrentAmount;
                UpdateResourceCounter(@event.ResourceType, resource.Quantity);
            }
        }

        private void OnWaveTimerChanged(WaveTimerChangedEvent @event)
        {
            _waveNumber = @event.WaveNumber;
            _waveTimer = @event.WaveTimer;
            _timeUntilNextWave = @event.TimeUntilNextWave;

            UpdateWaveTimer(_waveNumber, _timeUntilNextWave, _waveTimer);
        }

        #region UI Elements

        /// <summary>
        /// Initializes the HUD with the given UIDocument.
        /// /// </summary>
        private void InitializeHUD()
        {
            if (_hudDocument == null && !TryGetComponent(out _hudDocument)) Debug.LogError("HUDController: No UIDocument found");

            _root = _hudDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;

            // Initialize resource counters
            #region Resource Counters
            _resourceUILabels = new List<ResourceUILabels>();

            foreach (var type in Enum.GetValues(typeof(ResourceType)))
            {
                var typeName = type.ToString().ToLower();
                if (_debugMode) Debug.Log("Initializing resource counter for: " + typeName);

                var resourceCounterLabel = _root.Q<VisualElement>($"resource-counter-{typeName}")?.Q<Label>("resource-quantity");
                if (resourceCounterLabel == null)
                {
                    if (_debugMode) Debug.LogError("HUDController: No resource counter found for: " + typeName);
                }
                else
                {
                    resourceCounterLabel.text = "0";
                    _resourceUILabels.Add(new ResourceUILabels { ResourceType = typeName, ResourceLabel = resourceCounterLabel });
                }

                if (_debugMode) Debug.Log("Resource counter initialized for: " + typeName);
            }
            #endregion

            // Initialize wave info
            #region Wave Info

            // Wave number label
            _waveNumberLabel = _root.Q<Label>("wave-number");
            if (_waveNumberLabel == null)
                Debug.LogError("HUDController: No wave number label found");
            else
                _waveNumberLabel.text = "Wave 0";

            // Wave timer wrapper
            _waveTimerWrapper = _root.Q<VisualElement>("wave-timer-wrapper");
            if (_waveTimerWrapper == null)
                Debug.LogError("HUDController: No wave timer wrapper found");
            else
                _waveTimerWrapper.style.display = DisplayStyle.None;

            // Wave timer label
            _waveTimerLabel = _root.Q<Label>("wave-timer");
            if (_waveTimerLabel == null)
                Debug.LogError("HUDController: No wave timer label found");
            else
                _waveTimerLabel.text = "00:00";

            // Next wave timer wrapper
            _nextWaveTimerWrapper = _root.Q<VisualElement>("time-until-next-wave-wrapper");
            if (_nextWaveTimerWrapper == null)
                Debug.LogError("HUDController: No next wave timer wrapper found");
            else
                _nextWaveTimerWrapper.style.display = DisplayStyle.None;

            // Next wave timer label
            _nextWaveTimerLabel = _root.Q<Label>("time-until-next-wave");
            if (_nextWaveTimerLabel == null)
                Debug.LogError("HUDController: No next wave timer label found");
            else
                _nextWaveTimerLabel.text = "00:00";
            #endregion

            // Initialize flood level
            #region Flood Level
            _floodProgressBar = _root.Q<ProgressBar>("flood-bar");
            if (_floodProgressBar == null)
                Debug.LogError("HUDController: No flood progress bar found");
            else
            {
                _floodProgressBar.value = 0;
                _floodProgressBar.title = "0%";
            }
            #endregion
        }

        /// <summary>
        /// Toggles HUD elements based on the current game state
        /// </summary>
        /// <param name="@event">The game state changed event</param>
        private void ToggleHUDElements(GameState state)
        {
            if (_root == null) return;

            _root.style.display = DisplayStyle.None;

            if (_debugMode) Debug.Log("Game State: " + state);

            if (state == GameState.WaveActive)
            {
                _root.style.display = DisplayStyle.Flex;
                if (_debugMode) Debug.Log($"Wave Active, wave timer display: {_waveTimerWrapper.style.display}");
                if (_waveTimerWrapper is not null) _waveTimerWrapper.style.display = DisplayStyle.Flex;
                if (_nextWaveTimerWrapper is not null) _nextWaveTimerWrapper.style.display = DisplayStyle.None;
            }
            else if (state == GameState.WavePreparation)
            {
                _root.style.display = DisplayStyle.Flex;
                if (_debugMode) Debug.Log($"Wave Preparation, wave timer display: {_waveTimerWrapper.style.display}");
                if (_waveTimerWrapper is not null) _waveTimerWrapper.style.display = DisplayStyle.None;
                if (_nextWaveTimerWrapper is not null) _nextWaveTimerWrapper.style.display = DisplayStyle.Flex;
            }
        }

        /// <summary>
        /// Updates the resource counter for a specific resource type
        /// </summary>
        /// <param name="resourceType">The type of resource to update</param>
        /// <param name="quantity">The new quantity of the resource</param>
        private void UpdateResourceCounter(ResourceType resourceType, int quantity)
        {
            var resourceTypeName = resourceType.ToString().ToLower();
            var resourceUIElement = _resourceUILabels.FirstOrDefault(r => r.ResourceType == resourceTypeName);
            if (resourceUIElement != null)
            {
                resourceUIElement.ResourceLabel.text = quantity.ToString();
            }
        }

        /// <summary>
        /// Updates the wave information
        /// </summary>
        /// <param name="waveNumber">The current wave number</param>
        /// <param name="timeUntilNextWave">The time until the next wave starts</param>
        /// <param name="waveTimer">The time until the current wave ends</param>
        private void UpdateWaveTimer(int waveNumber, float timeUntilNextWave, float waveTimer)
        {
            if (_waveNumberLabel is not null) _waveNumberLabel.text = waveNumber.ToString();
            if (_nextWaveTimerLabel is not null) _nextWaveTimerLabel.text = FormatTime(timeUntilNextWave);
            if (_waveTimerLabel is not null) _waveTimerLabel.text = FormatTime(waveTimer);
        }

        /// <summary>
        /// Formats a float value in minutes and seconds
        /// </summary>
        /// <param name="time">The time to format</param>
        /// <returns>The formatted time</returns>
        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            return $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Updates the flood level
        /// </summary>
        /// <param name="currentLevel">The current flood level</param>
        /// <param name="maxLevel">The maximum flood level</param>
        private void UpdateFloodLevelBar(float currentLevel, float maxLevel)
        {
            if (_floodProgressBar is not null)
            {
                var percentage = maxLevel > 0 ? Mathf.RoundToInt((currentLevel / maxLevel) * 100) : 0;
                _floodProgressBar.value = percentage;
                _floodProgressBar.highValue = maxLevel > 0 ? 100 : 0;
                _floodProgressBar.title = $"{percentage}%";
            }
        }
        #endregion
    }
}
