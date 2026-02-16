using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;

namespace ioi.Components
{
    internal class ObjectMap : ICollisionActor
    {
        public Sprite Sprite { get; set; }

        public Color Color { get; set; }

        public bool IsBounds { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Coords { get; set; }

        public Vector2 Size { get; set; }

        public IShapeF Bounds => new RectangleF(Position, Size);

        public void RecalculatePositionFromCoords()
        {
            Position = new Vector2(Coords.X * GameHost.Game.CellSize.X, Coords.Y * GameHost.Game.CellSize.Y);
        }

        public Vector2 KeyCoords() => Coords;

        public void OnCollision(CollisionEventArgs collisionInfo) { }

        public Action<ObjectMap> OnCollide;

        public void ProcessCollision(List<ObjectMap> collided)
        {
            if (collided.Count == 0)
                return;                

            foreach (var collision in collided)
            {
                if(OnCollide!=default)
                    OnCollide(collision);

                if (collision.OnCollide != default)
                    collision.OnCollide(this);
            }
        }
    }
}
