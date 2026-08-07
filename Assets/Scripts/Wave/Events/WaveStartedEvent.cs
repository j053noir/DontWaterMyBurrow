namespace DontWaterMyBurrow.Wave.Events
{
    public readonly struct WaveStartedEvent
    {
        public readonly int WaveNumber;
        public readonly int TotalWaves;
        public readonly float WaveDuration;

        public WaveStartedEvent(int waveNumber, int totalWaves = 3, float waveDuration = 300f)
        {
            WaveNumber = waveNumber;
            TotalWaves = totalWaves;
            WaveDuration = waveDuration;
        }
    }
}