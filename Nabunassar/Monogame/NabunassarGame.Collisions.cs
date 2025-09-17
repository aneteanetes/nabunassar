using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;
using Nabunassar.Monogame.Extended;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        protected void InitializeCollisions()
        {
            var quadTreeBounds = new RectangleF(0, 0, Resolution.Width, Resolution.Height);
            CollisionComponent = new CustomCollisionComponent(quadTreeBounds);

            var playerLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add(CollisionLayers.Player, playerLayer);

            var hiddenLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add(CollisionLayers.Hidden, hiddenLayer);

            var revealedLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add(CollisionLayers.Revealed, revealedLayer);

            var objectsLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add(CollisionLayers.Objects, objectsLayer);

            var groundLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add(CollisionLayers.Ground, groundLayer);

            var cursorLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add(CollisionLayers.Cursor, cursorLayer);

            CollisionComponent.AddCollisionBetweenLayer(cursorLayer, objectsLayer);
            CollisionComponent.AddCollisionBetweenLayer(cursorLayer, revealedLayer);

            CollisionComponent.AddCollisionBetweenLayer(playerLayer, objectsLayer);
            CollisionComponent.AddCollisionBetweenLayer(playerLayer, groundLayer);
        }

        public void DisposeCollisionComponent()
        {
            CollisionComponent.IsEnabled = false;
            CollisionComponent.Dispose();
            CollisionComponent = null;
        }
    }
}