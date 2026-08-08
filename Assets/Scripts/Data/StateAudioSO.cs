using DontWaterMyBurrow.Game;
using UnityEngine;

namespace DontWaterMyBurrow.Data
{
    [CreateAssetMenu(fileName = "NewStateAudio", menuName = "ScriptableObjects/StateAudio")]
    public class StateAudioSO : ScriptableObject
    {
        public GameState GameState;
        public AudioClip AudioClip;
    }
}