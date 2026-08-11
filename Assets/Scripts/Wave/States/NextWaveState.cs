using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Wave.States
{
    public class NextWaveState : IState
    {
        private WaveManager _waveManager;

        public NextWaveState(WaveManager waveManager)
        {
            _waveManager = waveManager;
        }

        public void Enter()
        {
            if (_waveManager.debugMode) Debug.Log("Starting wave");

            _waveManager.StartWave();
        }

        public void Exit() { }
    }
}