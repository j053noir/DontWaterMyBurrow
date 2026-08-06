public readonly struct WaveCompletedEvent
{
    public readonly int WaveNumber;
    public readonly int MaxWaveNumber;

    public WaveCompletedEvent(int waveNumber, int maxWaveNumber)
    {
        WaveNumber = waveNumber;
        MaxWaveNumber = maxWaveNumber;
    }
}