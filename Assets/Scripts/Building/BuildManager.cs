using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Water;
using DontWaterMyBurrow.Resources;
using DontWaterMyBurrow.Building.Events;

namespace DontWaterMyBurrow.Building
{
    public class BuildManager : MonoBehaviour
    {
    [SerializeField] private WaterManager _waterManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private ResourceManager _resourceManager;

    private void OnEnable()
    {
        EventBus.Subscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<BuildValidationRequestEvent>(OnBuildValidationRequested);
    }

    private void OnBuildValidationRequested(BuildValidationRequestEvent @event)
    {
        bool isValid = !_waterManager.IsCellFlooded(@event.BuildPosition)
                    && !_gridManager.IsCellOccupied(@event.BuildPosition)
                    && _resourceManager.HasEnoughResources(@event.StructureData);

        @event.Callback?.Invoke(isValid);
    }
}
}