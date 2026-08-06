using UnityEngine;

public class StructureController : MonoBehaviour
{
    [SerializeField] private StructureType _type;
    [SerializeField] private Vector2Int _position;

    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private bool _hasBeenDamaged = false;

    public StructureType Type => _type;
    public Vector2Int Position => _position;
    public bool IsDamaged => _hasBeenDamaged;
    public int Health => _health;

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

    protected void DestroyStructure()
    {
        // TODO: Implement object pooling to about instancing new ones
        Destroy(this.gameObject);
    }
}