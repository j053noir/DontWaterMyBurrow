using System;
using System.Collections.Generic;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Player.Events;
using DontWaterMyBurrow.UI.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace DontWaterMyBurrow.UI
{
    [Serializable]
    public class BuildingMenuInfo
    {
        public StructureType Type { get; set; }
        public StructureDataSO StructureData { get; set; }
        public Button Button { get; set; }
        public Action Handle { get; set; }
    }

    public class BuildingMenuController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private TransformAnchorSO _playerTransformAnchor;
        [SerializeField] private List<StructureDataSO> _structures;

        [Header("Input Actions")]
        [SerializeField] private InputActionReference _onMoveAction;
        [SerializeField] private InputActionReference _onInteractAction;
        [SerializeField] private InputActionReference _onCancelAction;

        [Header("UI Components")]
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private VisualElement _root;
        private List<BuildingMenuInfo> _menuButtons;

        private int _selectedIndex = 0;
        private float _nextAllowedTime;
        private bool _isMenuOpen = false;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;

        private void Awake()
        {
            _menuButtons = new();

            if (_mapGridConfig == null) Debug.LogError("[BuildingMenuController] MapGridConfigSO is null");
            if (_playerTransformAnchor == null) Debug.LogError("[BuildingMenuController] PlayerTransformAnchorSO is null");

            InitializeUI();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerOpenedBuildMenuEvent>(OnPlayerOpenedBuildMenuEvent);
            EventBus.Subscribe<PlayerClosedBuildMenuEvent>(OnPlayerClosedBuildMenuEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerOpenedBuildMenuEvent>(OnPlayerOpenedBuildMenuEvent);
            EventBus.Unsubscribe<PlayerClosedBuildMenuEvent>(OnPlayerClosedBuildMenuEvent);
        }

        private void LateUpdate()
        {
            // Guard clauses
            if (!_isMenuOpen || _mapGridConfig == null ||
                _playerTransformAnchor == null || !_playerTransformAnchor.IsSet) return;

            // 1. Obtener tamaño dinámico del menú en unidades de mundo
            var ppu = (_menuDocument != null && _menuDocument.panelSettings != null)
                ? _menuDocument.panelSettings.referenceSpritePixelsPerUnit
                : 100f;
            if (ppu <= 0) ppu = 100f;

            var visualElement = _root.Q<VisualElement>("building-menu-container") ?? _root;
            var widthInWorld = (visualElement.layout.width > 0 ? visualElement.layout.width : visualElement.worldBound.width) / ppu;
            var heightInWorld = (visualElement.layout.height > 0 ? visualElement.layout.height : visualElement.worldBound.height) / ppu;

            var halfWidthInWorld = widthInWorld * 0.5f;
            var playerPos = _playerTransformAnchor.Transform.position;
            var tileSize = _mapGridConfig.TileSize;

            // 2. Clampear X usando el helper del MapGridConfigSO
            var posX = _mapGridConfig.ClampWorldX(playerPos.x, halfWidthInWorld);

            // 3. Calcular Y con Pivot BottomCenter:
            // Despejamos las orejas del conejo con un gap de 1.1 tiles
            var gap = tileSize * 1.5f;
            var targetPosY = playerPos.y + gap;

            if (targetPosY + heightInWorld > _mapGridConfig.MaxWorldY)
            {
                // Si sobrepasa el techo, se posiciona por debajo de los pies (-0.5 tiles - altura del menú)
                targetPosY = playerPos.y - (tileSize * 0.5f) - heightInWorld;
            }

            // Clampear Y inferior
            var posY = Mathf.Max(targetPosY, _mapGridConfig.MinWorldY);

            // 4. Asignar posición con Z = -1f para renderizar delante de los sprites
            transform.position = new Vector3(posX, posY, -1f);
        }

        private void OnDestroy()
        {
            UnsubscribeInputActions();
            ClearMenuButtons();
        }

        private void OnPlayerOpenedBuildMenuEvent(PlayerOpenedBuildMenuEvent @event)
        {
            if (_root != null)
            {
                _root.style.display = DisplayStyle.Flex;
                _isMenuOpen = true;
                if (_mapGridConfig != null)
                {
                    var menuPosition = _mapGridConfig.GridToWorld(@event.TargetCell);
                    transform.position = menuPosition;
                }
            }

            _selectedIndex = 0;
            if (_menuButtons != null && _menuButtons.Count > 0 && _menuButtons[_selectedIndex]?.Button != null)
            {
                _menuButtons[_selectedIndex].Button.AddToClassList("selected");
            }

            SubscribeInputActions();
        }

        private void OnPlayerClosedBuildMenuEvent(PlayerClosedBuildMenuEvent @event)
        {
            HideMenu();
            UnsubscribeInputActions();
        }

        private void SubscribeInputActions()
        {
            if (_onMoveAction?.action != null) _onMoveAction.action.performed += OnMoveActionPerformed;
            if (_onInteractAction?.action != null) _onInteractAction.action.performed += OnInteractPerformed;
            if (_onCancelAction?.action != null) _onCancelAction.action.performed += OnCancelPerformed;
        }

        private void UnsubscribeInputActions()
        {
            if (_onMoveAction?.action != null) _onMoveAction.action.performed -= OnMoveActionPerformed;
            if (_onInteractAction?.action != null) _onInteractAction.action.performed -= OnInteractPerformed;
            if (_onCancelAction?.action != null) _onCancelAction.action.performed -= OnCancelPerformed;
        }

        private void InitializeUI()
        {
            if (_menuDocument == null && !TryGetComponent(out _menuDocument))
            {
                Debug.LogError("[BuildingMenuController] No UIDocument found");
                return;
            }

            if (_menuDocument.rootVisualElement == null)
            {
                Debug.LogError("[BuildingMenuController] UIDocument has no rootVisualElement");
                return;
            }

            _root = _menuDocument.rootVisualElement.Q<VisualElement>();
            if (_root == null)
            {
                Debug.LogError("[BuildingMenuController] No root VisualElement found in UIDocument");
                return;
            }

            HideMenu();

            var menuContainer = _root.Q<VisualElement>("building-menu-buttons");
            if (menuContainer == null)
            {
                Debug.LogError("[BuildingMenuController] No building-menu-buttons container found");
                return;
            }

            menuContainer.Clear();
            _menuButtons = new();

            if (_structures == null) return;

            foreach (var structure in _structures)
            {
                if (structure == null) continue;

                var button = new Button
                {
                    text = structure.name,
                };
                button.AddToClassList("building-menu-button");
                Action handler = () => OnStructureButtonClicked(structure);
                button.clicked += handler;
                _menuButtons.Add(new BuildingMenuInfo
                {
                    Type = structure.Type,
                    StructureData = structure,
                    Button = button,
                    Handle = handler
                });
                menuContainer.Add(button);
            }
        }

        private void ClearMenuButtons()
        {
            if (_menuButtons == null) return;

            foreach (var buildingMenuInfo in _menuButtons)
            {
                if (buildingMenuInfo?.Button != null && buildingMenuInfo.Handle != null)
                {
                    buildingMenuInfo.Button.clicked -= buildingMenuInfo.Handle;
                }
            }
            _menuButtons.Clear();
        }

        private void OnStructureButtonClicked(StructureDataSO structureSO)
        {
            if (structureSO == null) return;

            if (_debugMode) Debug.Log("[BuildingMenuController] Structure button clicked: " + structureSO);

            EventBus.Publish(new SelectStructureToBuildEvent(structureSO));
            HideMenu();
        }

        private void HideMenu()
        {
            _isMenuOpen = false;
            if (_root != null) _root.style.display = DisplayStyle.None;

            if (_menuButtons != null && _menuButtons.Count > 0 && _selectedIndex >= 0 && _selectedIndex < _menuButtons.Count)
            {
                _menuButtons[_selectedIndex]?.Button?.RemoveFromClassList("selected");
                _selectedIndex = 0;
            }
        }

        private void OnMoveActionPerformed(InputAction.CallbackContext context)
        {
            if (_menuButtons == null || _menuButtons.Count == 0) return;
            if (Time.time < _nextAllowedTime) return;

            var value = context.ReadValue<Vector2>();
            if (Math.Abs(value.y) < 0.5f) return;

            _nextAllowedTime = Time.time + 0.5f;

            if (_selectedIndex >= 0 && _selectedIndex < _menuButtons.Count)
            {
                _menuButtons[_selectedIndex]?.Button?.RemoveFromClassList("selected");
            }

            if (value.y > 0.5f) _selectedIndex = _selectedIndex == 0 ? 0 : _selectedIndex - 1;
            else if (value.y < -0.5f) _selectedIndex = _selectedIndex == _menuButtons.Count - 1 ? _menuButtons.Count - 1 : _selectedIndex + 1;

            if (_selectedIndex >= 0 && _selectedIndex < _menuButtons.Count)
            {
                _menuButtons[_selectedIndex]?.Button?.AddToClassList("selected");
            }
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            HideMenu();

            if (_menuButtons == null || _selectedIndex < 0 || _selectedIndex >= _menuButtons.Count) return;

            var selectedData = _menuButtons[_selectedIndex]?.StructureData;
            if (selectedData != null)
            {
                EventBus.Publish(new SelectStructureToBuildEvent(selectedData));
            }
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            HideMenu();
            EventBus.Publish(new ClosedBuildMenuEvent());
        }
    }
}