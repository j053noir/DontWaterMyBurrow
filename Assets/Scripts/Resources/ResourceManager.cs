using UnityEngine;
using System.Collections.Generic;
using System;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures.Events;
using DontWaterMyBurrow.Resources.Events;

namespace DontWaterMyBurrow.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private Dictionary<ResourceType, int> _resources;

        private void Awake()
        {
            _resources = new();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Subscribe<ResourceCollectedEvent>(OnResouceCollected);
            EventBus.Subscribe<RepairStructureRequestEvent>(OnRepairStructureRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<StructureBuiltEvent>(OnStructureBuilt);
            EventBus.Unsubscribe<ResourceCollectedEvent>(OnResouceCollected);
            EventBus.Unsubscribe<RepairStructureRequestEvent>(OnRepairStructureRequested);
        }

    private void OnStructureBuilt(StructureBuiltEvent @event)
    {
        ConsumeResources(@event.StructureData);
    }

    private void OnResouceCollected(ResourceCollectedEvent @event)
    {
        AddResource(@event.ResourceType, @event.Quantity);
    }

    private void OnRepairStructureRequested(RepairStructureRequestEvent @event)
    {
        var isValid = HasEnoughResources(@event.StructureSO);

        if (isValid) ConsumeResources(@event.StructureSO);

        @event.Callback?.Invoke(isValid);
    }

    public void ConsumeResources(StructureDataSO structureSO)
    {
        try
        {
            if (!HasEnoughResources(structureSO)) return;

            foreach (var resource in structureSO.Costs)
            {
                if (_resources.ContainsKey(resource.Type))
                {
                    _resources[resource.Type] -= resource.Cost;
                    EventBus.Publish(new ResourceChangedEvent(resource.Type, _resources[resource.Type], -resource.Cost));
                }
            }
        }
        catch (KeyNotFoundException ex)
        {
            Debug.LogError($"No resources found for the given resource type: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error consuming resources: {ex.Message}");
        }
    }

    public void AddResource(ResourceType type, int quantity)
    {
        // Search for resource in dictionary
        if (_resources.ContainsKey(type))
        {
            _resources[type] += quantity;
        }
        // If resource not found, add it to the dictionary
        else
        {
            _resources.Add(type, quantity);
        }

        EventBus.Publish(new ResourceChangedEvent(type, quantity));
    }

    public bool HasEnoughResources(StructureDataSO structureSO)
    {
        foreach (var resource in structureSO.Costs)
        {
            if (!_resources.ContainsKey(resource.Type) || _resources[resource.Type] < resource.Cost)
            {
                Debug.LogWarning("Insufficient resources to build the structure, missing " + resource.Type + " with cost " + resource.Cost);
                return false;
            }
        }
        return true;
    }
}
}