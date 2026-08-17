using System.Collections.Generic;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Player.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    public class BuildingMenuController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private List<StructureDataSO> _structures;

        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private Dictionary<StructureType, Button> _menuButtons;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;

        public void Awake()
        {
            _menuButtons = new();
        }

        public void OnEnable()
        {
            InitializeUI();

            EventBus.Subscribe<PlayerOpenedBuilMenuEvent>(OnPlayerOpenedBuilMenuEvent);
            EventBus.Subscribe<PlayerClosedBuildMenuEvent>(OnPlayerClosedBuildMenuEvent);
        }

        public void OnDisable()
        {
            EventBus.Unsubscribe<PlayerOpenedBuilMenuEvent>(OnPlayerOpenedBuilMenuEvent);
            EventBus.Unsubscribe<PlayerClosedBuildMenuEvent>(OnPlayerClosedBuildMenuEvent);

            ClearMenuButtons();
        }

        private void OnPlayerClosedBuildMenuEvent(PlayerClosedBuildMenuEvent @event)
        {
            if (_root != null) _root.style.display = DisplayStyle.None;
        }

        private void OnPlayerOpenedBuilMenuEvent(PlayerOpenedBuilMenuEvent @event)
        {
            if (_root != null)
            {
                _root.style.display = DisplayStyle.Flex;
                var menuPosition = _mapGridConfig.GridToWorld(@event.TargetCell);

                transform.position = menuPosition;
            }
        }

        private void InitializeUI()
        {
            if (_menuDocument == null && !TryGetComponent(out _menuDocument)) Debug.LogError("[BuildingMenuController] No UIDocument found");

            _root = _menuDocument.rootVisualElement.Q<VisualElement>();
            HideMenu();

            var menuContainer = _root.Q<VisualElement>("building-menu-container");
            _menuButtons = new();

            foreach (var structure in _structures)
            {
                var button = new Button
                {
                    text = structure.name,
                };
                button.AddToClassList("building-menu-button");
                button.clicked += () => OnStructureButtonClicked(structure.Type);
                _menuButtons[structure.Type] = button;
                menuContainer.Add(button);
            }
        }

        private void ClearMenuButtons()
        {
            foreach (var buttonKvp in _menuButtons)
            {
                buttonKvp.Value.clicked -= () => OnStructureButtonClicked(buttonKvp.Key);
            }
            _menuButtons.Clear();
        }

        private void OnStructureButtonClicked(StructureType structure)
        {
            if (_debugMode) Debug.Log("[BuildingMenuController] Structure button clicked: " + structure);

            var dataSO = _structures.Find(x => x.Type == structure);
            if (dataSO == null) return;

            EventBus.Publish(new SelectStructureToBuildEvent(dataSO));
            HideMenu();
        }

        private void HideMenu()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}