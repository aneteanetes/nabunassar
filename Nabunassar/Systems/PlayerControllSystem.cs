using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Struct;

namespace Nabunassar.Systems
{
    internal class PlayerControllSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<Party> _partyComponentMapper;
        ComponentMapper<MoveComponent> _moveComponentMapper;
        ComponentMapper<BoundsComponent> _boundsComponentMapper;

        public PlayerControllSystem(NabunassarGame game) : base(Aspect.One(typeof(Party)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _partyComponentMapper = mapperService.GetMapper<Party>();
            _moveComponentMapper = mapperService.GetMapper<MoveComponent>();
            _boundsComponentMapper = mapperService.GetMapper<BoundsComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();
            var mouse = MouseExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var party = _partyComponentMapper.Get(entityId);
                var bounds = _boundsComponentMapper.Get(entityId);
                var move = _moveComponentMapper.Get(entityId);

                Vector2 moveVector = Vector2.Zero;

                if (keyboard.IsKeyDown(Keys.Space))
                    move.Stop();

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
                    move.MoveToDirection(bounds.Position, moveVector * move.MoveSpeed);

                if (mouse.WasButtonPressed(MouseButton.Left))
                {
                    var targetPosition = _game.Camera.ScreenToWorld(mouse.X, mouse.Y);

                    void MovePartyInternal()
                    {
                        move.MoveToPosition(bounds.BoundsComponent.Origin, targetPosition);

                        var dirComp = move.DirectionEntity.Get<RenderComponent>();
                        dirComp.Sprite.IsVisible = true;
                        dirComp.Position = targetPosition;
                    }

                    if (move.IsMoving())
                    {
                        var targetRectangle = new RectangleF(new Vector2(targetPosition.X - 2, targetPosition.Y - 2), new SizeF(6, 6));
                        if (targetRectangle.Intersects(new RectangleF(move.TargetPosition, new SizeF(1, 1))))
                        {
                            move.Stop();
                        }
                        else
                        {
                            MovePartyInternal();
                        }
                    }
                    else
                    {
                        MovePartyInternal();
                    }
                }

                if (move.IsMoving())
                {
                    foreach (var hero in party)
                    {
                        var animatedSprite = hero.Entity.Get<AnimatedSprite>();

                        //set sprite face view
                        if (move.MoveDirection.OneOf([Direction.Left, Direction.LeftUp, Direction.LeftDown]))
                        {
                            animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                            //party.Rotate(Direction.Left);
                        }
                        else if (move.MoveDirection.OneOf([Direction.Right, Direction.RightUp, Direction.RightDown]))
                        {
                            animatedSprite.Effect = SpriteEffects.None;
                            //party.Rotate(Direction.Right);
                        }

                        if (animatedSprite != null && animatedSprite.CurrentAnimation != "run")
                        {
                            animatedSprite.SetAnimation("run");
                        }
                    }
                }
                else
                {
                    foreach (var hero in party)
                    {
                        var animatedSprite = hero.Entity.Get<AnimatedSprite>();
                        if (animatedSprite.CurrentAnimation != "idle")
                        {
                            animatedSprite.SetAnimation("idle");
                        }
                    }
                }
            }
        }
    }
}