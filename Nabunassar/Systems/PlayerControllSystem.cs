using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class PlayerControllSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<PlayerComponent> _playerComponentMapper;
        ComponentMapper<MoveComponent> _moveComponentMapper;
        ComponentMapper<BoundsComponent> _boundComponentMapper;

        public PlayerControllSystem(NabunassarGame game) : base(Aspect.All(typeof(PlayerComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerComponentMapper = mapperService.GetMapper<PlayerComponent>();
            _moveComponentMapper = mapperService.GetMapper<MoveComponent>();
            _boundComponentMapper = mapperService.GetMapper<BoundsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();
            var mouse = MouseExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var player = _playerComponentMapper.Get(entityId);
                var bounds = _boundComponentMapper.Get(entityId);
                var moving = _moveComponentMapper.Get(entityId);

                Vector2 moveVector = Vector2.Zero;

                if (keyboard.IsKeyDown(Keys.S))
                {
                    moveVector.Y += 1;
                }
                if (keyboard.IsKeyDown(Keys.W))
                {
                    moveVector.Y -= 1;
                }
                if (keyboard.IsKeyDown(Keys.A))
                {
                    moveVector.X -= 1;
                }
                if (keyboard.IsKeyDown(Keys.D))
                {
                    moveVector.X += 1;
                }

                if (moveVector != Vector2.Zero)
                    moving.MoveToDirection(bounds.Position, moveVector);

                if (mouse.WasButtonPressed(MouseButton.Left))
                {
                    var targetPosition = _game.Camera.ScreenToWorld(mouse.X, mouse.Y);
                    moving.MoveToPosition(bounds.Origin, targetPosition);
                }
            }
        }

        private static void MoveBounds(KeyboardStateExtended state, PlayerComponent player, BoundsComponent bound)
        {
            //return;

            if (state.IsKeyDown(Keys.Up))
            {
                bound.Bounds.Position = new Vector2(bound.Bounds.Position.X, bound.Bounds.Position.Y + player.Character.Speed);
            }
            if (state.IsKeyDown(Keys.Down))
            {
                bound.Bounds.Position = new Vector2(bound.Bounds.Position.X, bound.Bounds.Position.Y - player.Character.Speed);
            }
            if (state.IsKeyDown(Keys.Left))
            {
                bound.Bounds.Position = new Vector2(bound.Bounds.Position.X - player.Character.Speed, bound.Bounds.Position.Y);
            }
            if (state.IsKeyDown(Keys.Right))
            {
                bound.Bounds.Position = new Vector2(bound.Bounds.Position.X + player.Character.Speed, bound.Bounds.Position.Y);
            }
        }
    }
}