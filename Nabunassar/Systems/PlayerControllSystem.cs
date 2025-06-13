using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Systems
{
    internal class PlayerControllSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<PlayerComponent> _playerComponentMapper;
        ComponentMapper<RenderComponent> _renderComponentMapper;
        ComponentMapper<CollisionsComponent> _collisionComponentMapper;

        public PlayerControllSystem(NabunassarGame game) : base(Aspect.All(typeof(PlayerComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerComponentMapper = mapperService.GetMapper<PlayerComponent>();
            _renderComponentMapper = mapperService.GetMapper<RenderComponent>();
            _collisionComponentMapper = mapperService.GetMapper<CollisionsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            var state = KeyboardExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var player = _playerComponentMapper.Get(entityId);
                var render = _renderComponentMapper.Get(entityId);
                var bound = _collisionComponentMapper.Get(entityId);

                float x = 0, y = 0;

                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    y += player.Character.Speed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                {
                    y -= player.Character.Speed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                {
                    x -= player.Character.Speed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    x += player.Character.Speed;
                }

                MoveBounds(state, player, bound);

                var newPos = new Vector2(render.Position.X + x, render.Position.Y + y);

                if (render.Sprite is AnimatedSprite animatedSprite)
                {
                    if (newPos != render.Position)
                    {
                        if (animatedSprite.CurrentAnimation != "run")
                        {
                            animatedSprite.SetAnimation("run");
                        }

                        var dir = render.Position.DetectDirection(newPos);
                        var newDir = dir;
                        if (dir.Is(Direction.Left))
                            newDir = Direction.Left;
                        else if (dir.Is(Direction.Right))
                            newDir = Direction.Right;
                        else
                            newDir = player.Character.ViewDirection;

                        player.Character.ViewDirection = newDir;

                        animatedSprite.Effect = player.Character.ViewDirection == Direction.Left
                            ? SpriteEffects.FlipHorizontally
                            : SpriteEffects.None;

                        render.PrevPosition = render.Position;
                        render.Position = newPos;
                        bound.PrevBoundPosition = bound.Bounds.Position;
                        bound.Bounds.Position = new Vector2(bound.Bounds.Position.X + x, bound.Bounds.Position.Y + y);
                    }
                    else if (animatedSprite.CurrentAnimation != "idle")
                    {
                        animatedSprite.SetAnimation("idle");
                    }
                }

                player.Character.SetDirtSpeed();
            }
        }

        private static void MoveBounds(KeyboardStateExtended state, PlayerComponent player, CollisionsComponent bound)
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