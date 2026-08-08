using UnityEngine;

public readonly struct PlaySFXEvent
{
    public readonly AudioClip AudioClip;

    public PlaySFXEvent(AudioClip audioClip)
    {
        AudioClip = audioClip;
    }
}