using DontWaterMyBurrow.Building.Events;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Core.Interfaces;
using DontWaterMyBurrow.Data;
using UnityEngine;

namespace DontWaterMyBurrow.Player.States
{
    public class PlayerPlacementState : IState
    {
        private readonly PlayerController _player;
        public StructureDataSO StructureData { get; private set; }
        public IState PreviousState { get; private set; }
        public int EntryFrame { get; private set; }
        public bool CanConfirmPlacement => Time.frameCount > EntryFrame;

        public PlayerPlacementState(PlayerController player, StructureDataSO structureData, IState previousState)
        {
            _player = player;
            StructureData = structureData;
            PreviousState = previousState;
        }

        public void Enter()
        {
            EntryFrame = Time.frameCount;

            if (_player.DebugMode) Debug.Log("Enter from Player Placement State");

            EventBus.Publish(new SelectStructureToBuildEvent(StructureData));
        }

        public void Exit()
        {
            if (_player.DebugMode) Debug.Log("Exit from Player Placement State");

            EventBus.Publish(new ClearStructureSelectionEvent());
        }
    }
}