using MonoGame.Extended;
using Nabunassar.Components.Abstract;

namespace Nabunassar.Components
{
    internal class PositionComponent : BaseComponent
    {
        public PositionComponent(NabunassarGame game) : base(game)
        {
        }

        public virtual Vector2 Position { get; set; }

        public Vector2 Origin { get; set; }

        public virtual void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetPosition(float x, float y) 
            => SetPosition(new Vector2(x, y));
    }
}
