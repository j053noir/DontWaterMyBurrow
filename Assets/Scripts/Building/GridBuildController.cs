using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Player.Events;
using DontWaterMyBurrow.Building.Events;
using System;

namespace DontWaterMyBurrow.Building
{
    public class GridBuildController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MapGridConfigSO _mapGridConfig;
        [SerializeField] private Transform _structureContainer;

        [Header("Components")]
        [SerializeField] private SpriteRenderer _structurePreview;
        [SerializeField] private StructureDataSO _selectedStructureSO;

        [Header("Build")]
        [SerializeField] private Vector2Int _buildPosition;
        [SerializeField] private Color _validColor = new Color(0, 1, 0, 0.5f);
        [SerializeField] private Color _invalidColor = new Color(1, 0, 0, 0.5f);

        private bool _canBuild = false;
        private bool _isValid = true;

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<SelectStructureToBuildEvent>(OnSelectStructureToBuild);
            EventBus.Subscribe<PlayerBuildTargetChangedEvent>(OnPlayerBuildTargetChanged);
            EventBus.Subscribe<ConfirmBuildEvent>(OnConfirmBuild);
            EventBus.Subscribe<PlayerClosedBuildMenuEvent>(OnClosedBuildMenu);
            EventBus.Subscribe<ClearStructureSelectionEvent>(OnClearStructureSelection);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<SelectStructureToBuildEvent>(OnSelectStructureToBuild);
            EventBus.Unsubscribe<PlayerBuildTargetChangedEvent>(OnPlayerBuildTargetChanged);
            EventBus.Unsubscribe<ConfirmBuildEvent>(OnConfirmBuild);
            EventBus.Unsubscribe<PlayerClosedBuildMenuEvent>(OnClosedBuildMenu);
            EventBus.Unsubscribe<ClearStructureSelectionEvent>(OnClearStructureSelection);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            if (@event.NewState == GameState.MainMenu || @event.NewState == GameState.GameOver || @event.NewState == GameState.Victory)
            {
                _canBuild = false;
            }
            else if (@event.NewState == GameState.WavePreparation || @event.NewState == GameState.WaveActive)
            {
                _canBuild = true;
            }
        }

        private void OnSelectStructureToBuild(SelectStructureToBuildEvent @event)
        {
            _selectedStructureSO = @event.StructureData;
            ValidateBuildPosition(_buildPosition, _selectedStructureSO);
        }

        private void OnPlayerBuildTargetChanged(PlayerBuildTargetChangedEvent @event)
        {
            _buildPosition = @event.TargetCell;
            ValidateBuildPosition(@event.TargetCell, _selectedStructureSO);
        }

        private void OnClosedBuildMenu(PlayerClosedBuildMenuEvent @event)
        {
            _selectedStructureSO = null;
            if (_structurePreview != null) _structurePreview.enabled = false;
        }

        private void OnClearStructureSelection(ClearStructureSelectionEvent @event)
        {
            _selectedStructureSO = null;
            if (_structurePreview != null) _structurePreview.enabled = false;
        }

        public void ValidateBuildPosition(Vector2Int gridPosition, StructureDataSO structureSO)
        {
            if (!_canBuild || structureSO == null)
            {
                SetPreviewState(isValid: false);
                return;
            }

            var validationEvent = new BuildValidationRequestEvent(gridPosition, structureSO);
            EventBus.Publish(validationEvent);

            SetPreviewState(validationEvent.IsValid);
        }

        public void SetPreviewState(bool isValid)
        {
            _isValid = isValid;

            if (_selectedStructureSO == null) return;

            _structurePreview.color = isValid ? _validColor : _invalidColor;
            _structurePreview.sprite = _selectedStructureSO.PreviewSprite;
            _structurePreview.transform.position = _mapGridConfig.GridToWorld(_buildPosition);
            _structurePreview.enabled = _canBuild;
        }

        public void OnConfirmBuild(ConfirmBuildEvent @event)
        {
            if (!_canBuild || !_isValid || !_selectedStructureSO)
            {
                EventBus.Publish(new BuildFailedEvent());
                return;
            }

            _buildPosition = @event.Position;
            var structureGO = Instantiate(_selectedStructureSO.Prefab, _mapGridConfig.GridToWorld(_buildPosition), Quaternion.identity, _structureContainer);
            var builtSO = _selectedStructureSO;
            EventBus.Publish(new StructureBuiltEvent(builtSO.Type, _buildPosition, structureGO, builtSO));

            ValidateBuildPosition(_buildPosition, _selectedStructureSO);
        }
    }
}