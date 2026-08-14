using System;
using System.Collections.Generic;
using UnityEngine;
using DontWaterMyBurrow.Hazards;
using DontWaterMyBurrow.Wave.Events;

namespace DontWaterMyBurrow.Data
{
    [Serializable]
    public struct HazardsSpawnData
    {
        public HazardType Type;
        /// <summary>
        /// How often a hazard is spawned.
        /// </summary>
        public float SpawnInterval;
        /// <summary>
        /// Reference to the hazard prefab.
        /// </summary>
        public GameObject Prefab;
    }

    [CreateAssetMenu(fileName = "NewWaveData", menuName = "ScriptableObjects/WaveData")]
    public class WaveDataSO : ScriptableObject
    {
        public int WaveNumber;
        public float WaveDuration;
        public List<HazardsSpawnData> HazardsToSpawn;
        public List<Vector2Int> WaterLeekingPositions;
    }
}