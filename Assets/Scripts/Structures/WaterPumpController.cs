using UnityEngine;
using DontWaterMyBurrow.Core;
using DontWaterMyBurrow.Structures.State;

namespace DontWaterMyBurrow.Structures
{
    public class WaterPumpController : StructureController
    {
        [SerializeField] private int _drainRadius = 2;
        [SerializeField] private float _drainRate = 3f;

        public int DrainRadius => _drainRadius;
        public float DrainRate => _drainRate;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;
        public bool DebugMode => _debugMode;

        private StateMachine _stateMachine;
        public PumpOperationalState OperationalState { get; private set; }
        public PumpCloggedState CloggedState { get; private set; }
        public bool IsClogged => _stateMachine.CurrentState is PumpCloggedState;

        private void Awake()
        {
            _stateMachine = new();
            OperationalState = new PumpOperationalState(this);
            CloggedState = new PumpCloggedState(this);

            _stateMachine.ChangeState(OperationalState);
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        public void SetClogState(bool isClogged)
        {
            _stateMachine.ChangeState(isClogged ? CloggedState : OperationalState);
        }

        public void CleanPump()
        {
            _stateMachine.ChangeState(OperationalState);
        }
    }
}