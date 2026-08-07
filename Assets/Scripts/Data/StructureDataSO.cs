using System.Collections.Generic;
using UnityEngine;
using System;

namespace DontWaterMyBurrow.Data
{
    [Serializable]
    public struct StructureCost
    {
        public ResourceType Type;
        public int Cost;
    }

    [CreateAssetMenu(fileName = "NewStructureData", menuName = "ScriptableObjects/StructureData")]
    public class StructureDataSO : ScriptableObject
    {
        public GameObject Prefab;
        public Sprite PreviewSprite;
        public StructureType Type;
        public List<StructureCost> Costs;
        public int MaxHealth;
    }
}