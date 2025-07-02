using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;
using System.IO;

namespace Nabunassar.Entities.Data
{
    internal class Party : Quad<Hero>
    {
        public Entity Entity { get; set; }

        public MapObject MapObject { get; set; }

        public Direction ViewDirection { get; set; } = Direction.Right;

        public RenderComponent DirectionRender { get; internal set; }

        private NabunassarGame _game;

        public Party(NabunassarGame game)
        {
            _game = game;
            First = new Hero(game);
            Second = new Hero(game);
            Third = new Hero(game);
            Fourth = new Hero(game);
        }

        public void OnCollision(CollisionEventArgs collisionInfo, MapObject host, MapObject other)
        {

        }

        public void Select(QuadPosition position)
        {
            _game.GameState.Log($"Select hero at {position} position");

            var current = this[position];
                while (First != current)
                    Rotate();
        }

        public void Rotate()
        {
            _game.GameState.Log($"Rotate party");

            var first = First;
            var firstOrder = first.Order;

            var second = Second;
            var secondOrder = second.Order;

            var third = Third;
            var thirdOrder = third.Order;

            var fourth = Fourth;
            var fourthOrder = fourth.Order;

            First = second;
            First.Order = firstOrder;

            Second = third;
            Second.Order = secondOrder;

            Third = fourth;
            Third.Order = thirdOrder;

            Fourth = first;
            Fourth.Order = fourthOrder;

            if (ViewDirection == Direction.Left)
            {
                SetPositionOnAllPartyLeft();
            }

            if (ViewDirection == Direction.Right)
            {
                SetPositionOnAllPartyRight();
            }
        }

        internal void SetPositionOnAllPartyLeft()
        {
            SetPositionOn(First, QuadPosition.First);
            SetPositionOn(Second, QuadPosition.Second);
            SetPositionOn(Third, QuadPosition.Thrid);
            SetPositionOn(Fourth, QuadPosition.Fourth);
        }

        internal void SetPositionOnAllPartyRight()
        {
            SetPositionOn(First, QuadPosition.Fourth);
            SetPositionOn(Second, QuadPosition.Thrid);
            SetPositionOn(Third, QuadPosition.Second);
            SetPositionOn(Fourth, QuadPosition.First);
        }

        internal void ChangeDirection(Direction direction)
        {
            var gameobject = First.GameObject;

            if (!gameobject.IsMoving)
            {
                if (ViewDirection != direction)
                {
                    ViewDirection = direction;

                    if (ViewDirection == Direction.Left)
                    {
                        SetPositionOnAllPartyLeft();
                    }

                    if (ViewDirection == Direction.Right)
                    {
                        SetPositionOnAllPartyRight();
                    }
                }
            }
        }

        private void SetPositionOn(Hero hero, QuadPosition position)
        {
            var y = -12;
            switch (position)
            {
                case QuadPosition.First:
                    hero.GameObject.SetAbsolutePosition(-4,y);
                    break;
                case QuadPosition.Second:
                    hero.GameObject.SetAbsolutePosition(4, y);
                    break;
                case QuadPosition.Thrid:
                    hero.GameObject.SetAbsolutePosition(12, y);
                    break;
                case QuadPosition.Fourth:
                    hero.GameObject.SetAbsolutePosition(20, y);
                    break;
                default:
                    break;
            }
        }

        public void MoveTo(Vector2 to)
        {
            MapObject.MoveToPosition(MapObject.Position, to);

            DirectionRender.Sprite.IsVisible = true;
            DirectionRender.Position = to;
        }

        public void OnStopMoving()
        {
            foreach (var hero in this)
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