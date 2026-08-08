using UnityEngine;

namespace DontWaterMyBurrow.Wave.Events
{
    public readonly struct WaveTimerChangedEvent
    {
        public readonly int WaveNumber;
        public readonly float TimeUntilNextWave;
        public readonly float WaveTimer;

        public WaveTimerChangedEvent(int waveNumber, float timeUntilNextWave, float waveTimer)
        {
            WaveNumber = waveNumber;
            TimeUntilNextWave = timeUntilNextWave;
            WaveTimer = waveTimer;
        }
    }
}