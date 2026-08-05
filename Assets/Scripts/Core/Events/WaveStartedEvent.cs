public struct WaveStartedEvent
{
    public int WaveNumber;
    public float WaveDuration;

    public WaveStartedEvent(int waveNumber, float waveDuration)
    {
        WaveNumber = waveNumber;
        WaveDuration = waveDuration;
    }
}