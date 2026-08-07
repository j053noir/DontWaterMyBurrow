using System;
using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Wave.Events;
using DontWaterMyBurrow.Structures;
using DontWaterMyBurrow.Structures.Events;

namespace DontWaterMyBurrow.Hazards
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class HazardsController : MonoBehaviour
    {
        [SerializeField] private HazardType _hazardType;
        [SerializeField] private Vector2 _currentFlowVector;
        [SerializeField] private int _damageAmount;
        [SerializeField] private float _speed = 5.0f;

        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;

    public HazardType Type => _hazardType;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        MoveWithCurrent();
    }

    private void MoveWithCurrent()
    {
        if (_rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + _currentFlowVector * _speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collisioned with {collision.gameObject.tag}");

        if (collision.gameObject.TryGetComponent<StructureController>(out var structure))
        {
            OnCollisionWithStructure(structure);
        }
        else if (collision.gameObject.TryGetComponent<HazardsController>(out var hazard))
        {
            OnCollisionWithHazard(hazard);
        }
        else
        {
            Debug.Log($"Collisioned with uknown object: {collision.gameObject.name} {collision.gameObject.tag}");
        }
    }

    protected virtual void OnCollisionWithStructure(StructureController structure)
    {
        // Create a dam if the hazard is a log and the structure is a sandbag
        if (_hazardType == HazardType.Log && structure.Type == StructureType.SandBag)
        {
            CreateDam();
        }
        // Deal damage if the hazard is not leaves
        else if (_hazardType != HazardType.Leaves)
        {
            structure.TakeDamage(_damageAmount);
        }
        // Leaves only interact with water pumps
        else if (_hazardType == HazardType.Leaves && structure is WaterPumpController waterPump)
        {
            waterPump.SetClogState(true);
            // TODO: Put leaves in object pool
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionWithHazard(HazardsController hazard)
    {
        // Create a dam if both hazards are logs or rock
        if (_hazardType == HazardType.Log && (hazard.Type == HazardType.Rock || hazard.Type == HazardType.Log))
        {
            CreateDam();
        }
    }

    private void CreateDam()
    {
        if (_hazardType != HazardType.Log)
        {
            return;
        }

        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        _collider2D.isTrigger = true;

        if (TryGetComponent<DamController>(out var dam))
        {
            dam.enabled = true;
        }
    }
}
}