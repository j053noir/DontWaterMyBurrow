using System.Collections.Generic;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Data;
using DontWaterMyBurrow.Game.Events;
using UnityEngine;

namespace DontWaterMyBurrow.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;

        [Header("Game State Music")]
        [SerializeField] private List<StateAudioSO> _stateMusics;

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
            var stateMusic = _stateMusics.Find(x => x.GameState == @event.NewState);

            if (stateMusic == null || stateMusic.AudioClip == null)
            {
                Debug.LogError("AudioManager: No music found for game state: " + @event.NewState);
                _musicAudioSource.Stop();
                return;
            }

            _musicAudioSource.clip = stateMusic.AudioClip;
            _musicAudioSource.Play();
        }

        public void OnPlaySFX(PlaySFXEvent @event)
        {
            _sfxAudioSource.PlayOneShot(@event.AudioClip);
        }
    }
}
