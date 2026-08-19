using UnityEngine;

namespace DontWaterMyBurrow.Data
{
    [CreateAssetMenu(fileName = "TransformAnchorSO", menuName = "ScriptableObjects/TransformAnchorSO")]

    public class TransformAnchorSO : ScriptableObject
    {
        public Transform Transform { get; set; }
        public bool IsSet => Transform != null;

    }
}