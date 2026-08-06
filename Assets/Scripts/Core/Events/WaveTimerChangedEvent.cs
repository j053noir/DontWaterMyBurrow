using UnityEngine;

public readonly struct WaveTimerChangedEvent
{
    public readonly float TimeUntilNextWave;
    public readonly float WaveTimer;

    public WaveTimerChangedEvent(float timeUntilNextWave, float waveTimer)
    {
        TimeUntilNextWave = timeUntilNextWave;
        WaveTimer = waveTimer;
    }
}