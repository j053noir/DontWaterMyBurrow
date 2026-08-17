using System;
using System.Collections.Generic;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Player.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    [Serializable]
    public class BuildingMenuInfo
    {
        public StructureType Type { get; set; }
        public Button Button { get; set; }
        public Action Handle { get; set; }
    }

    public class BuildingMenuController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private List<StructureDataSO> _structures;

        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private List<BuildingMenuInfo> _menuButtons;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;

        public void Awake()
        {
            _menuButtons = new();

            InitializeUI();
        }

        public void OnEnable()
        {
            EventBus.Subscribe<PlayerOpenedBuildMenuEvent>(OnPlayerOpenedBuilMenuEvent);
            EventBus.Subscribe<PlayerClosedBuildMenuEvent>(OnPlayerClosedBuildMenuEvent);
        }

        public void OnDisable()
        {
            EventBus.Unsubscribe<PlayerOpenedBuildMenuEvent>(OnPlayerOpenedBuilMenuEvent);
            EventBus.Unsubscribe<PlayerClosedBuildMenuEvent>(OnPlayerClosedBuildMenuEvent);
        }

        public void OnDestroy()
        {
            ClearMenuButtons();
        }

        private void OnPlayerClosedBuildMenuEvent(PlayerClosedBuildMenuEvent @event)
        {
            if (_root != null) _root.style.display = DisplayStyle.None;
        }

        private void OnPlayerOpenedBuilMenuEvent(PlayerOpenedBuildMenuEvent @event)
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

            var menuContainer = _root.Q<VisualElement>("building-menu-buttons");

            if (menuContainer == null)
            {
                Debug.LogError("[BuildingMenuController] No building-menu-buttons found");
                return;
            }

            menuContainer.Clear();
            _menuButtons = new();

            foreach (var structure in _structures)
            {
                var button = new Button
                {
                    text = structure.name,
                };
                button.AddToClassList("building-menu-button");
                Action handler = () => OnStructureButtonClicked(structure);
                button.clicked += handler;
                _menuButtons.Add(new BuildingMenuInfo { Type = structure.Type, Button = button, Handle = handler });
                menuContainer.Add(button);
            }
        }

        private void ClearMenuButtons()
        {
            foreach (var buildingMenuInfo in _menuButtons)
            {
                buildingMenuInfo.Button.clicked -= buildingMenuInfo.Handle;
            }
            _menuButtons.Clear();
        }

        private void OnStructureButtonClicked(StructureDataSO structureSO)
        {
            if (_debugMode) Debug.Log("[BuildingMenuController] Structure button clicked: " + structureSO);

            EventBus.Publish(new SelectStructureToBuildEvent(structureSO));
            HideMenu();
        }

        private void HideMenu()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}