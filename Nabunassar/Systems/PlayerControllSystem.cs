using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;
using Geranium.Reflection;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial;
using Nabunassar.Components.Effects;

namespace Nabunassar.Systems
{
    internal class PlayerControllSystem : BaseSystem
    {
        ComponentMapper<Party> _partyComponentMapper;
        ComponentMapper<MapObject> _gameObjectComponentMapper;

        public PlayerControllSystem(NabunassarGame game) : base(game,Aspect.One(typeof(Party)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _partyComponentMapper = mapperService.GetMapper<Party>();
            _gameObjectComponentMapper = mapperService.GetMapper<MapObject>();
        }

        public override void Update(GameTime gameTime, bool sys)
        {
            var keyboard = KeyboardExtended.GetState();
            var mouse = MouseExtended.GetState();

            if (IsPreventNextMove && !mouse.WasButtonPressed(MouseButton.Left))
                IsPreventNextMove = false;

            var entityId = ActiveEntities.FirstOrDefault();

            if (entityId != default)
            {
                var party = _partyComponentMapper.Get(entityId);
                var gameobj = _gameObjectComponentMapper.Get(entityId);

                if (mouse.WasButtonPressed(MouseButton.Right) && Game.IsMouseMoveAvailable)
                {
                    SelectObjectByMouse(gameTime, mouse);
                }
                if (mouse.WasButtonPressed(MouseButton.Left))
                {
                    MoveByMouse(mouse, party, gameobj);
                }

                PartyActions(keyboard,party,gameobj);

                MoveByKeyboard(keyboard, gameobj);

                PartyAnimationsAndViewDirection(gameobj, party);
            }
        }

        private void PartyAnimationsAndViewDirection(MapObject gameobj, Party party)
        {
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
        }

        private void MoveByKeyboard(KeyboardStateExtended keyboard, MapObject gameobj)
        {
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
                gameobj.MoveToDirection(gameobj.Position, moveVector * gameobj.MoveSpeed);

            if (gameobj.IsMoving && gameobj.Ray2==default && moveVector == Vector2.Zero)
                gameobj.StopMove();
        }

        private void PartyActions(KeyboardStateExtended keyboard, Party party, MapObject gameobj)
        {
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

            if(keyboard.WasKeyPressed(Keys.G))
                foreach (var hero in party)
                {
                    hero.Entity.Attach(new DissolveEffect(Game, hero.Entity) as EffectComponent);
                }
        }

        private void MoveByMouse(MouseStateExtended mouse, Party party, MapObject gameobj)
        {
            if (IsPreventNextMove)
            {
                IsPreventNextMove = false;
                return;
            }

            if (Game.IsMouseMoveAvailable)
            {
                Game.GameState.Log("moved party by mouse click");

                var targetPosition = Game.Camera.ScreenToWorld(mouse.X, mouse.Y);

                void MovePartyByMouseInternal()
                {
                    var mouseScreenPos = mouse.Position.ToVector2();

                    party.MoveTo(targetPosition, Game.GameState.Cursor.FocusedMapObject, mouseScreenPos);
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
                        MovePartyByMouseInternal();
                    }
                }
                else
                {
                    MovePartyByMouseInternal();
                }
            }
        }

        public static bool IsPreventNextMove = false;

        private void SelectObjectByMouse(GameTime gameTime, MouseStateExtended mouse)
        {
            var cursor = Game.GameState.Cursor;
            var radialMenuGameObject = cursor.FocusedMapObject;

            if (radialMenuGameObject == null)
            {
                var layer = Game.CollisionComponent.Layers["ground"];
                var mousePos = Game.Camera.ScreenToWorld(mouse.Position.ToVector2());
                var groundTile = layer.Space.Query(new RectangleF(mousePos, new SizeF(2, 2))).FirstOrDefault();
                if (groundTile != default)
                {
                    radialMenuGameObject = groundTile.As<MapObject>().GameObject;
                }
            }

            RadialMenu.Open(Game, radialMenuGameObject, new Vector2(mouse.X, mouse.Y));

            return;
        }
    }
}