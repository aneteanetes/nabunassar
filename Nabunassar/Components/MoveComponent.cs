using Nabunassar.Components.Abstract;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class MoveComponent : BaseComponent
    {
        public MoveComponent(NabunassarGame game) : base(game)
        {
            MoveSpeed = game.DataBase.GetGroundTypeSpeed(GroundType.Dirt);
        }

        public Vector2 TargetPosition { get; set; } = Vector2.Zero;

        public Direction MoveDirection { get; set; } = Direction.Idle;

        public float MoveSpeed { get; set; } = 0f;

        public void MoveToDirection(Vector2 from, Vector2 moveSpeedVector)
        {
            TargetPosition = new Vector2(from.X + moveSpeedVector.X, from.Y + moveSpeedVector.Y);
            MoveDirection = Vector2.Zero.DetectDirection(moveSpeedVector);
        }

        public void MoveToPosition(Vector2 from, Vector2 positionToMoving)
        {
            TargetPosition = positionToMoving;
            MoveDirection = Vector2.Zero.DetectDirection(positionToMoving);
        }

        public void Stop()
        {
            MoveDirection = Direction.Idle;
            TargetPosition = Vector2.Zero;
        }

        public void ResetMoveSpeed()
        {
            MoveSpeed = Game.DataBase.GetGroundTypeSpeed(GroundType.Dirt);
        }

        public bool IsMoving() => MoveDirection != Direction.Idle;
    }
}
