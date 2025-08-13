using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;

namespace Nabunassar
{
    internal partial class NabunassarGame : Game
    {
        protected void InitializeCollisions()
        {
            var quadTreeBounds = new RectangleF(0, 0, Resolution.Width, Resolution.Height);
            CollisionComponent = new CustomCollisionComponent(quadTreeBounds);

            var playerLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("player", playerLayer);

            var hiddenLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("hidden", hiddenLayer);

            var revealedLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("revealed", revealedLayer);

            var objectsLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("objects", objectsLayer);

            var groundLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("ground", groundLayer);

            var cursorLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("cursor", cursorLayer);

            CollisionComponent.AddCollisionBetweenLayer(cursorLayer, objectsLayer);
            CollisionComponent.AddCollisionBetweenLayer(cursorLayer, revealedLayer);

            CollisionComponent.AddCollisionBetweenLayer(playerLayer, objectsLayer);
            CollisionComponent.AddCollisionBetweenLayer(playerLayer, groundLayer);
        }
    }
}