using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;

namespace Nabunassar
{
    internal static class PerPixelCollisionExtensions
    {
        public static bool IsIntersects(this RenderComponent wall, CollisionsComponent player)
        {
            // Get Color data of each Texture
            Color[] bitsA = wall.Data;
            Color[] bitsB = player.BoundsData;

            var aBounds = new RectangleF(wall.Position, new SizeF(wall.Sprite.TextureRegion.Size.Width, wall.Sprite.TextureRegion.Size.Height));
            var bBounds = player.Bounds.As<RectangleF>();

            // Calculate the intersecting rectangle
            int x1 = (int)Math.Floor(Math.Max(aBounds.X, bBounds.X));
            int x2 = (int)Math.Floor(Math.Min(aBounds.X + aBounds.Width, bBounds.X + bBounds.Width));

            int y1 = (int)Math.Floor(Math.Max(aBounds.Y, bBounds.Y));
            int y2 = (int)Math.Floor(Math.Min(aBounds.Y + aBounds.Height, bBounds.Y + bBounds.Height));

            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    Color colorA = bitsA[(x - (int)Math.Floor(aBounds.X)) + (y - (int)Math.Floor(aBounds.Y)) * wall.Sprite.TextureRegion.Width];
                    Color colorB = bitsB[(x - (int)Math.Floor(bBounds.X)) + (y - (int)Math.Floor(bBounds.Y)) * (int)bBounds.Width];

                    if (colorA.A != 0 && colorB.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            //If no collision occurred by now, we're clear.
            return false;
        }

        public static bool IsIntersects_PerPixelCollision(this RenderComponent component, RectangleF otherBounds, RenderComponent other)
        {
            var texture1 = component.Sprite.TextureRegion;

            return CollisionIsIntersectsetection(
                
                new Rectangle((int)other.Position.X, (int)other.Position.Y, (int)otherBounds.Width, (int)otherBounds.Height),
                other.Data,
                new Rectangle((int)component.Position.X, (int)component.Position.Y, texture1.Width, texture1.Height),
                component.Data);
        }

        public static bool CollisionIsIntersectsetection(Rectangle rectA, Color[] dataA, Rectangle rectB, Color[] dataB)
        {
            var top = Math.Max(rectA.Top, rectB.Top);
            var bottom = Math.Min(rectA.Bottom, rectB.Bottom);
            var left = Math.Max(rectA.Left, rectB.Left);
            var right = Math.Min(rectA.Right, rectB.Right);

            for (var i = top; i < bottom; i++)
            {
                for (var j = left; j < right; j++)
                {
                    Color a = dataA[((int)j - (int)rectA.Left) + ((int)i - (int)rectA.Top) * (int)rectA.Width];
                    Color b = dataB[((int)j - (int)rectB.Left) + ((int)i - (int)rectB.Top) * (int)rectB.Width];

                    if (a.A != 0 && b.A != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
