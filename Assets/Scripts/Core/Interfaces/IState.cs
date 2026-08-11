namespace DontWaterMyBurrow.Core.Interfaces
{
    public interface IState
    {
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }
}