using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Systems
{
    internal class MoveSystem : EntityUpdateSystem, IDrawSystem
    {
        NabunassarGame _game;
        ComponentMapper<MoveComponent> _moveComponentMapper;
        ComponentMapper<RenderComponent> _renderComponentMapper;
        ComponentMapper<BoundsComponent> _collisionComponentMapper;

        public MoveSystem(NabunassarGame game) : base(Aspect.All(typeof(MoveComponent),typeof(RenderComponent)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _moveComponentMapper = mapperService.GetMapper<MoveComponent>();
            _renderComponentMapper = mapperService.GetMapper<RenderComponent>();
            _collisionComponentMapper = mapperService.GetMapper<BoundsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var move = _moveComponentMapper.Get(entityId);
                var render = _renderComponentMapper.Get(entityId);
                var bound = _collisionComponentMapper.Get(entityId);

                var sprite = render.Sprite;
                var animatedSprite = sprite.As<AnimatedSprite>();

                if (move.IsMoving())
                {
                    float xOffset = 0;
                    float yOffset = 0;

                    var renderNewPos = Vector2.Zero;
                    var boundNewPos = Vector2.Zero;

                    if (move.Ray2 == default)
                    {
                        switch (move.MoveDirection)
                        {
                            case Direction.Up:
                                yOffset -= move.MoveSpeed;
                                break;
                            case Direction.Down:
                                yOffset += move.MoveSpeed;
                                break;
                            case Direction.Left:
                                xOffset -= move.MoveSpeed;
                                break;
                            case Direction.Right:
                                xOffset += move.MoveSpeed;
                                break;
                            case Direction.UpLeft:
                                yOffset -= move.MoveSpeed;
                                xOffset -= move.MoveSpeed;
                                break;
                            case Direction.UpRight:
                                yOffset -= move.MoveSpeed;
                                xOffset += move.MoveSpeed;
                                break;
                            case Direction.DownLeft:
                                yOffset += move.MoveSpeed;
                                xOffset -= move.MoveSpeed;
                                break;
                            case Direction.DownRight:
                                yOffset += move.MoveSpeed;
                                xOffset += move.MoveSpeed;
                                break;
                            default:
                                break;
                        }

                        renderNewPos = new Vector2(render.Position.X + xOffset, render.Position.Y + yOffset);
                        boundNewPos = new Vector2(bound.Position.X + xOffset, bound.Position.Y + yOffset);
                    }
                    else
                    {
                        var boundT = move.MoveSpeed / Vector2.Distance(bound.Position, move.Ray2.Direction);

                        renderNewPos = Vector2.Lerp(render.Position, move.Ray2.Direction, boundT);

                        boundNewPos = Vector2.Lerp(bound.Position, move.Ray2.Direction, boundT);
                    }

                    //reset speed after changing position
                    move.ResetMoveSpeed();

                    //set sprite face view
                    if (move.MoveDirection.OneOf([Direction.Left, Direction.LeftUp, Direction.LeftDown]))
                    {
                        sprite.Effect = SpriteEffects.FlipHorizontally;
                    }
                    else if (move.MoveDirection.OneOf([Direction.Right, Direction.RightUp, Direction.RightDown]))
                    {
                        sprite.Effect = SpriteEffects.None;
                    }
                    if (animatedSprite != null && animatedSprite.CurrentAnimation != "run")
                    {
                        animatedSprite.SetAnimation("run");
                    }

                    // reset moving if position reached
                    if (bound.Bounds.Intersects(new RectangleF(move.TargetPosition, new SizeF(1, 1))))
                    {
                        move.Stop();
                    }

                    // setting position
                    render.SetPosition(renderNewPos);
                    bound.SetPosition(boundNewPos);

                }
                else if (animatedSprite!=null && animatedSprite.CurrentAnimation!="idle")
                {
                    animatedSprite.SetAnimation("idle");
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

        public void Draw(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var move = _moveComponentMapper.Get(entityId);
                var ray = move.Ray2;
                if(ray!=default && _game.IsDrawBounds)
                {
                    var sb = _game.BeginDraw();
                    sb.DrawLine(ray.Position, ray.Direction, Color.Red, 1);
                }
            }
        }
    }
}