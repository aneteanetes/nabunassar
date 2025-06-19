using MonoGame.Extended;
using MonoGame.Extended.ECS;
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

        public Entity DirectionEntity { get; set; }

        public Ray2 Ray2 { get; set; }

        public Vector2 TargetPosition { get; set; } = Vector2.Zero;

        public Direction MoveDirection { get; set; } = Direction.Idle;

        public float MoveSpeed { get; set; } = 0f;

        public void MoveToDirection(Vector2 from, Vector2 moveSpeedVector)
        {
            TargetPosition = new Vector2(from.X + moveSpeedVector.X, from.Y + moveSpeedVector.Y);
            MoveDirection = Vector2.Zero.DetectDirection(moveSpeedVector);
            Ray2 = default;
        }

        public void MoveToPosition(Vector2 from, Vector2 positionToMoving)
        {
            TargetPosition = positionToMoving;
            MoveDirection = from.DetectDirection(positionToMoving);
            Ray2 = new Ray2(from, positionToMoving);
        }

        public void Stop()
        {
            Ray2 = default;
            MoveDirection = Direction.Idle;
            TargetPosition = Vector2.Zero;

            if (DirectionEntity != null)
            {
                var dirRender = DirectionEntity.Get<RenderComponent>();
                dirRender.Sprite.IsVisible = false;
            }
        }

        public void ResetMoveSpeed()
        {
            MoveSpeed = Game.DataBase.GetGroundTypeSpeed(GroundType.Dirt);
        }

        public bool IsMoving() => MoveDirection != Direction.Idle || Ray2 != default;
    }
}
