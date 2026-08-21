using DontWaterMyBurrow.Core.Interfaces;
using UnityEngine;

namespace DontWaterMyBurrow.Structures.States
{
    public class StructureIdleState : IState
    {
        private readonly StructureController _structure;

        public StructureIdleState(StructureController structure)
        {
            _structure = structure;
        }

        public void Enter()
        {
            if (_structure.debugMode) Debug.Log($"[StructureIdleState] {_structure.Type} entering idle state");
        }

        public void Exit()
        {
            if (_structure.debugMode) Debug.Log($"[StructureIdleState] {_structure.Type} exiting idle state");
        }
    }
}