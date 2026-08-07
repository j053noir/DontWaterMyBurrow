using System;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Hazards;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Water.Events;

namespace DontWaterMyBurrow.Structures
{
    [RequireComponent(typeof(Collider2D))]
    public class DrainController : MonoBehaviour
    {
    [Header("Setup")]
    [SerializeField] private Vector2Int _position;
    [SerializeField] private int _drainRadius;

    [Header("State")]
    [SerializeField] private bool _isClogged;

    public bool IsClogged => _isClogged;

    private void OnEnable()
    {
        EventBus.Publish(new RegisterWaterDrainEvent(_position, _drainRadius));
        EventBus.Subscribe<ClearCloggedDrainEvent>(OnClearCloggedDrain);
    }

    private void OnDisable()
    {
        EventBus.Publish(new RemoveWaterDrainEvent(_position));
        EventBus.Unsubscribe<ClearCloggedDrainEvent>(OnClearCloggedDrain);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collisioned with {collision.gameObject.tag}");

        if (collision.gameObject.TryGetComponent<HazardsController>(out var hazard))
        {
            OnCollisionWithHazard(hazard);
        }
    }

    private void OnCollisionWithHazard(HazardsController hazard)
    {
        if (hazard.Type != HazardType.Leaves)
        {
            SetClogState(true);
        }
    }

    private void OnClearCloggedDrain(ClearCloggedDrainEvent @event)
    {
        if (@event.position == _position)
        {
            SetClogState(false);
        }
    }

    public void SetClogState(bool isClogged)
    {
        _isClogged = isClogged;
    }
}
}