using UnityEngine;

public readonly struct PumpCloggedStateChangedEvent
{
    public readonly GameObject PumpInstance;
    public readonly bool IsClogged;

    public PumpCloggedStateChangedEvent(GameObject pumpInstance, bool isClogged)
    {
        PumpInstance = pumpInstance;
        IsClogged = isClogged;
    }
}