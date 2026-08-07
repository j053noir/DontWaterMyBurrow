using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using DontWaterMyBurrow.Player.Events;
using DontWaterMyBurrow.Building.Events;

namespace DontWaterMyBurrow.Building
{
    public class GridBuildController : MonoBehaviour
    {
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
        EventBus.Subscribe<PlayerBuildTargetChangedEvent>(OnPlayerBuildTargetChanged);
        EventBus.Subscribe<ConfirmBuildEvent>(OnConfirmBuild);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBus.Unsubscribe<PlayerBuildTargetChangedEvent>(OnPlayerBuildTargetChanged);
        EventBus.Unsubscribe<ConfirmBuildEvent>(OnConfirmBuild);
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        if (@event.NewState == GameState.StartMenu || @event.NewState == GameState.GameOver || @event.NewState == GameState.Victory)
        {
            _canBuild = false;
        }
        else if (@event.NewState == GameState.GamePlay || @event.NewState == GameState.WavePreparation)
        {
            _canBuild = true;
        }
    }

    private void OnPlayerBuildTargetChanged(PlayerBuildTargetChangedEvent @event)
    {
        _buildPosition = @event.TargetCell;
        ValidateBuildPosition(@event.TargetCell, _selectedStructureSO);
    }

    public void ValidateBuildPosition(Vector2Int gridPosition, StructureDataSO structureSO)
    {
        if (!_canBuild)
        {
            SetPreviewState(isValid: false);
            return;
        }

        EventBus.Publish(new BuildValidationRequestEvent(gridPosition, structureSO, isValid =>
        {
            SetPreviewState(isValid);
        }));
    }

    public void SetPreviewState(bool isValid)
    {
        _isValid = isValid;

        _structurePreview.color = isValid ? _validColor : _invalidColor;
        _structurePreview.sprite = _selectedStructureSO.PreviewSprite;
        _structurePreview.transform.position = GetBuildPosition();
        _structurePreview.enabled = _canBuild;
    }

    public void OnConfirmBuild(ConfirmBuildEvent @event)
    {
        if (!_canBuild || !_isValid) return;

        _buildPosition = @event.Position;
        var gameObject = Instantiate(_selectedStructureSO.Prefab, GetBuildPosition(), Quaternion.identity);
        EventBus.Publish(new StructureBuiltEvent(_selectedStructureSO.Type, _buildPosition, gameObject, _selectedStructureSO));
    }

    private Vector3 GetBuildPosition()
    {
        return new Vector3(_buildPosition.x, _buildPosition.y, 0);
    }
}
}