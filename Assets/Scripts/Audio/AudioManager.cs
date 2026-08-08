using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Game;
using DontWaterMyBurrow.Game.Events;
using System;
using System.Collections.Generic;

namespace DontWaterMyBurrow.Audio
{
    [Serializable]
    public class StateMusic
    {
        public GameState GameState;
        public AudioClip Music;
    }

    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;

        [Header("Game State Music")]
        [SerializeField] private List<StateMusic> _stateMusics;

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Subscribe<PlaySFXEvent>(OnPlaySFX);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            EventBus.Unsubscribe<PlaySFXEvent>(OnPlaySFX);
        }

        private void OnGameStateChanged(GameStateChangedEvent @event)
        {
            StateMusic stateMusic = _stateMusics.Find(x => x.GameState == @event.NewState);

            if (stateMusic == null || stateMusic.Music == null)
            {
                Debug.LogError("AudioManager: No music found for game state: " + @event.NewState);
                _musicAudioSource.Stop();
                return;
            }

            _musicAudioSource.clip = stateMusic.Music;
            _musicAudioSource.Play();
        }

        public void OnPlaySFX(PlaySFXEvent @event)
        {
            _sfxAudioSource.PlayOneShot(@event.AudioClip);
        }
    }
}
