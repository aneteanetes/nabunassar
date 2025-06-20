using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;
using System.Collections.Generic;

namespace Nabunassar.Entities.Data
{
    internal class Party : Quad<Hero>
    {
        public Entity Entity { get; set; }

        public MoveComponent Move { get; set; }

        public Direction ViewDirection { get; set; } = Direction.Right;

        public Party(NabunassarGame game)
        {
            First = new Hero(game);
            Second = new Hero(game);
            Third = new Hero(game);
            Fourth = new Hero(game);
        }

        public void Rotate(Direction direction)
        {
            var first = First;
            var firstOrder = first.Order;

            var second = Second;
            var secondOrder = second.Order;

            var third = Third;
            var thirdOrder = third.Order;

            var fourth = Fourth;
            var fourthOrder = fourth.Order;

            if (direction == Direction.Left)
            {
                First = second;
                Second = third;
                Third = fourth;
                Fourth = first;

            }
            else if (direction == Direction.Right)
            {
                First = fourth;
                Second = first;
                Third = second;
                Fourth = third;
            }
        }

        public void RecalculateOrder()
        {

        }

        public void OnCollision(CollisionEventArgs collisionInfo, Entity host, Entity other)
        {

        }

        private void ChangeOrder(Direction direction)
        {
            var firstOrder = First.Order;
            var secondOrder = Second.Order;
            var thirdOrder = Third.Order;
            var fourthOrder = Fourth.Order;

            //if (direction == Direction.Left)
            //{
            //    First = second;
            //    Second = third;
            //    Third = fourth;
            //    Fourth = first;

            //}
            //else if (direction == Direction.Right)
            //{
            //    First = fourth;
            //    Second = first;
            //    Third = second;
            //    Fourth = third;
            //}
        }

        internal void ChangeDirection(Direction direction)
        {
            var heroMove = First.Move;

            if (!heroMove.IsMoving && ViewDirection != direction)
            {
                ViewDirection = direction;

                var origin = First.Move.Position.BoundsComponent.Origin;

                var shift = 28;

                var x = direction == Direction.Left ? origin.X - shift : origin.X + shift;

                First.Move.MoveToPosition(First.Move.Position.BoundsComponent.Origin, new Vector2(x, origin.Y));
            }
        }

        internal void OnMoving(Vector2 position, Direction direction)
        {
            return;
            foreach (var hero in this)
            {
                if (hero.Move.IsMoving)
                {
                    var ray = hero.Move.Ray2;

                    float xFrom = ray.Position.X;
                    float yFrom = ray.Position.Y;

                    float xTo = ray.Direction.X;
                    float yTo = ray.Direction.Y;

                    void Up()
                    {
                        //yFrom = yFrom - position.Y;
                        yFrom = yTo - position.Y;
                    }

                    void Down()
                    {
                        //yFrom = yFrom + position.Y;
                        yFrom = yTo + position.Y;
                    }

                    void Left()
                    {
                        xFrom = position.X-xFrom;
                        xTo = position.X-xTo;
                    }

                    void Right()
                    {
                        //xFrom = xFrom + position.X;
                        xTo = xTo + position.X;
                    }

                    switch (direction)
                    {
                        case Direction.Up: Up();
                            break;
                        case Direction.Down: Down();
                            break;
                        case Direction.Left: Left();
                            break;
                        case Direction.Right: Right();
                            break;
                        case Direction.UpLeft: Up(); Left();
                            break;
                        case Direction.UpRight: Up(); Right();
                            break;
                        case Direction.DownLeft: Down(); Left();
                            break;
                        case Direction.DownRight: Down(); Right();
                            break;
                        default:
                            break;
                    }

                    hero.Move.Ray2 = new Ray2(new Vector2(xFrom,yFrom),new Vector2(xTo,yTo));
                }
            }
        }
    }
}