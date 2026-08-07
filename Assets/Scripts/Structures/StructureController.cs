using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Structures.Events;

namespace DontWaterMyBurrow.Structures
{
    public class StructureController : MonoBehaviour
    {
    [SerializeField] private StructureType _type;
    [SerializeField] private StructureDataSO _dataSO;
    [SerializeField] private Vector2Int _position;

    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private bool _hasBeenDamaged = false;

    public StructureType Type => _type;
    public StructureDataSO DataSO => _dataSO;
    public Vector2Int Position => _position;
    public bool IsDamaged => _hasBeenDamaged;
    public int Health => _health;

    public void Repair(int amount)
    {
        _health = Mathf.Min(_maxHealth, _health + amount);

        if (_health >= _maxHealth) _hasBeenDamaged = false;

        EventBus.Publish(new StructureRepairedEvent(_type, this.gameObject));
    }

    public void TakeDamage(int damageTaken)
    {
        _health -= damageTaken;
        _hasBeenDamaged = true;

        if (_health <= 0)
        {
            _health = 0;
            DestroyStructure();
        }
    }

    private void DestroyStructure()
    {
        // TODO: Implement object pooling to about instancing new ones
        EventBus.Publish(new StructureDestroyedEvent(Position, gameObject));
        Destroy(this.gameObject);
    }
}
}