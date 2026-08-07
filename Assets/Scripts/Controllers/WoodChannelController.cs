using UnityEngine;

public class WoodChannelController : MonoBehaviour
{
    [SerializeField] private Vector2Int _channelDirection = Vector2Int.left;

    public Vector2Int Direction => _channelDirection;

    private void OnEnable()
    {
        var gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        var rightDir = transform.right;
        _channelDirection = new Vector2Int(Mathf.RoundToInt(rightDir.x), Mathf.RoundToInt(rightDir.y));

        EventBus.Publish(new ChannelBuiltEvent(gridPosition, _channelDirection));
    }
}