using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicAudioSource;

    [Header("Game State Music")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;
    [SerializeField] private AudioClip _gameOverMusic;

    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent @event)
    {
        if (@event.NewState == GameState.StartMenu)
        {
            _musicAudioSource.clip = _menuMusic;
            _musicAudioSource.Play();
        }
        else if (@event.NewState == GameState.GamePlay)
        {
            _musicAudioSource.clip = _gameMusic;
            _musicAudioSource.Play();
        }
        else if (@event.NewState == GameState.GameOver)
        {
            _musicAudioSource.clip = _gameOverMusic;
            _musicAudioSource.Play();
        }
    }
}
