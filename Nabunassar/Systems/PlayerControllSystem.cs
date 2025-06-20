using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;

namespace Nabunassar.Systems
{
    internal class PlayerControllSystem : EntityUpdateSystem
    {
        NabunassarGame _game;
        ComponentMapper<Party> _partyComponentMapper;
        ComponentMapper<GameObject> _gameObjectComponentMapper;

        public PlayerControllSystem(NabunassarGame game) : base(Aspect.One(typeof(Party)))
        {
            _game = game;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _partyComponentMapper = mapperService.GetMapper<Party>();
            _gameObjectComponentMapper = mapperService.GetMapper<GameObject>();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();
            var mouse = MouseExtended.GetState();

            foreach (var entityId in ActiveEntities)
            {
                var party = _partyComponentMapper.Get(entityId);
                var gameobj = _gameObjectComponentMapper.Get(entityId);

                Vector2 moveVector = Vector2.Zero;

                if (keyboard.WasKeyPressed(Keys.Tab))
                    party.Rotate();

                if (keyboard.WasKeyPressed(Keys.D1))
                    party.Select(QuadPosition.First);

                if (keyboard.WasKeyPressed(Keys.D2))
                    party.Select(QuadPosition.Second);

                if (keyboard.WasKeyPressed(Keys.D3))
                    party.Select(QuadPosition.Thrid);

                if (keyboard.WasKeyPressed(Keys.D4))
                    party.Select(QuadPosition.Fourth);

                if (keyboard.IsKeyDown(Keys.Space))
                    gameobj.StopMove();

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
                    gameobj.MoveToDirection(gameobj.Position, moveVector * gameobj.MoveSpeed);

                if (mouse.WasButtonPressed(MouseButton.Left))
                {
                    var targetPosition = _game.Camera.ScreenToWorld(mouse.X, mouse.Y);

                    void MovePartyInternal()
                    {
                        gameobj.MoveToPosition(gameobj.Position, targetPosition);

                        var dirComp = party.DirectionRender;
                        dirComp.Sprite.IsVisible = true;
                        dirComp.Position = targetPosition;
                    }

                    if (gameobj.IsMoving)
                    {
                        var targetRectangle = new RectangleF(new Vector2(targetPosition.X - 2, targetPosition.Y - 2), new SizeF(6, 6));
                        if (targetRectangle.Intersects(new RectangleF(gameobj.TargetPosition, new SizeF(1, 1))))
                        {
                            gameobj.StopMove();
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

                if (gameobj.IsMoving)
                {
                    foreach (var hero in party)
                    {
                        var animatedSprite = hero.Entity.Get<AnimatedSprite>();

                        //set sprite face view
                        if (gameobj.MoveDirection.OneOf([Direction.Left, Direction.LeftUp, Direction.LeftDown]))
                        {
                            animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                            party.ChangeDirection(Direction.Left);
                        }
                        else if (gameobj.MoveDirection.OneOf([Direction.Right, Direction.RightUp, Direction.RightDown]))
                        {
                            animatedSprite.Effect = SpriteEffects.None;
                            party.ChangeDirection(Direction.Right);
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