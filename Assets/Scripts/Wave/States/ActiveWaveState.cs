using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Wave.States
{
    public class ActiveWaveState : IState
    {
        private readonly WaveManager _waveManager;

        public ActiveWaveState(WaveManager waveManager)
        {
            _waveManager = waveManager;
        }

        public void Enter()
        {
            if (_waveManager.debugMode) Debug.Log("Enter ActiveWaveState");
            _waveManager.RegisterWaterLeaks();
        }

        public void Exit()
        {
            if (_waveManager.debugMode) Debug.Log("Exit ActiveWaveState");
            _waveManager.RemoveWaterLeaks();
        }
    }
}