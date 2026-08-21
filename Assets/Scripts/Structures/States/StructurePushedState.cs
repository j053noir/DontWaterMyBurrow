using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Structures.States
{
    public class StructurePushedState : IState, IUpdateableState
    {
        private readonly StructureController _structure;
        private readonly Transform _pusherTransform;
        private readonly Vector3 _targetWorldPosition;
        private readonly Vector3 _offset;

        public StructurePushedState
        (
            StructureController structure,
            Transform pusherTransform,
            Vector3 targetWorldPosition,
            Vector3 offset
        )
        {
            _structure = structure;
            _pusherTransform = pusherTransform;
            _targetWorldPosition = targetWorldPosition;
            _offset = offset;
        }

        public void Enter()
        {
            if (_structure.debugMode) Debug.Log($"[StructurePushedState] {_structure.Type} entering pushed state");

            var nextPos = _pusherTransform.position + _offset;
            _structure.SetPosition(new Vector3(nextPos.x, nextPos.y, _structure.transform.position.z));
        }

        public void Exit()
        {
            if (_structure.debugMode) Debug.Log($"[StructurePushedState] {_structure.Type} exiting pushed state");

            _structure.SetPosition(new Vector3(_targetWorldPosition.x, _targetWorldPosition.y, _structure.transform.position.z));
            _structure.SetCell(_targetWorldPosition);
        }

        public void Update()
        {
            var nextPos = _pusherTransform.position + _offset;
            _structure.SetPosition(new Vector3(nextPos.x, nextPos.y, _structure.transform.position.z));

            // Evaluamos la distancia en 2D ignorando el eje Z
            var distance = Vector2.Distance(_structure.transform.position, _targetWorldPosition);
            if (distance <= 0.01f)
            {
                _structure.ToIdle();
            }
        }
    }
}