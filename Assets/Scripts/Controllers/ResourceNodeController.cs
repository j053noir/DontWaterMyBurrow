using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResourceNodeController : MonoBehaviour
{
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _amount = 1;

    public ResourceType Type => _resourceType;
    public int Amount => _amount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            var nodeCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            EventBus.Publish(new ResourceCollectedEvent(nodeCell, _resourceType, _amount));

            // TODO: Return to pool
            gameObject.SetActive(false);
        }
    }
}