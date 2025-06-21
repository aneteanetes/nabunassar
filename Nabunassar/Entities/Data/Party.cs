using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;

namespace Nabunassar.Entities.Data
{
    internal class Party : Quad<Hero>
    {
        public Entity Entity { get; set; }

        public GameObject GameObject { get; set; }

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

        public void OnCollision(CollisionEventArgs collisionInfo, GameObject host, GameObject other)
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

        internal void OnMoving(Vector2 prev, Vector2 next)
        {
            return;
            var diff = prev - next;
            var direction = prev.DetectDirection(next);

            foreach (var hero in this)
            {
                if (hero.GameObject.IsMoving)
                {
                    var ray = hero.GameObject.Ray2;

                    float xFrom = ray.Position.X;
                    float yFrom = ray.Position.Y;

                    float xTo = ray.Direction.X;
                    float yTo = ray.Direction.Y;

                    void Up()
                    {
                        yFrom -=diff.Y;
                        yTo -=diff.Y;
                    }

                    void Down()
                    {
                        yFrom += diff.Y;
                        yTo += diff.Y;
                    }

                    void Left()
                    {
                        xFrom -= diff.X;
                        xTo -= diff.X;
                    }

                    void Right()
                    {
                        xFrom += diff.X;
                        xTo += diff.X;
                    }

                    switch (direction)
                    {
                        case Direction.Up:
                            Up();
                            break;
                        case Direction.Down:
                            Down();
                            break;
                        case Direction.Left:
                            Left();
                            break;
                        case Direction.Right:
                            Right();
                            break;
                        case Direction.UpLeft:
                            Up(); Left();
                            break;
                        case Direction.UpRight:
                            Up(); Right();
                            break;
                        case Direction.DownLeft:
                            Down(); Left();
                            break;
                        case Direction.DownRight:
                            Down(); Right();
                            break;
                        default:
                            break;
                    }

                    hero.GameObject.Ray2 = new Ray2(new Vector2(xFrom, yFrom), new Vector2(xTo, yTo));
                }
            }
        }
    }
}