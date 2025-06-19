using Nabunassar.Components.Abstract;

namespace Nabunassar.Components
{
    internal class PositionComponent : BaseComponent
    {
        public PositionComponent(NabunassarGame game, PositionComponent parent=null) : base(game)
        {
            Parent = parent;
        }

        public PositionComponent Parent { get; set; }

        public BoundsComponent BoundsComponent { get; set; }

        private Vector2 _position;

        public virtual Vector2 Position
        {
            get => Parent == null ? _position : _position + Parent.Position;
            set => _position = value;
        }

        public virtual Vector2 Origin => new Vector2(Position.X + 32, Position.Y);

        public virtual void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetPosition(float x, float y) 
            => SetPosition(new Vector2(x, y));
    }
}
