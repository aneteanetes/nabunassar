using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data
{
    internal class Party : Quad<Hero>
    {
        public Entity Entity { get; set; }

        public MoveComponent Move { get; set; }

        public Party(NabunassarGame game)
        {
            First = new Hero(game);
            Second = new Hero(game);
            Third = new Hero(game);
            Fourth = new Hero(game);
        }

        public Direction Direction { get; set; } = Direction.Right;

        public void Rotate(Direction direction)
        {
            var first = First;
            var second = Second;
            var third = Third;
            var fourth = Fourth;

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
    }
}
