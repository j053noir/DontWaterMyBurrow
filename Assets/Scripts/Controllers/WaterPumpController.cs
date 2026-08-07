using UnityEngine;

public class WaterPumpController : StructureController
{
    [SerializeField] private int _drainRadius = 2;
    [SerializeField] private float _drainRate = 3f;
    [SerializeField] private bool _isClogged = false;

    public bool IsClogged => _isClogged;

    public void SetClogState(bool isClogged)
    {
        _isClogged = isClogged;
        EventBus.Publish(new PumpCloggedStateChangedEvent(this.gameObject, _isClogged));
    }

    private void Update()
    {
        PumpWater();
    }

    private void PumpWater()
    {
        if (_isClogged)
        {
            return;
        }

        // Drain water in the drain radius 
        if (_drainRadius > 0 && _drainRate > 0)
        {
            var vector = this.gameObject.transform.position;
            var position = new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
            EventBus.Publish(new WaterDrainEvent(position, _drainRate * Time.deltaTime, _drainRadius));
        }
    }

    public void CleanPump()
    {
        _isClogged = false;
        EventBus.Publish(new PumpCloggedStateChangedEvent(this.gameObject, false));
    }
}