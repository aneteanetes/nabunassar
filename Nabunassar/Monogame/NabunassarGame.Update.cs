using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Nabunassar.Monogame.SpriteBatch;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public void DebugUpdate(GameTime gameTime)
        {
            light.Position = MouseExtended.GetState().Position.ToVector2();

            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.X))
                isDrawFPS = !isDrawFPS;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.X))
                isDrawCoords = !isDrawCoords;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.B))
                IsDrawBounds = !IsDrawBounds;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.Y))
            {
                Penumbra.Visible = !Penumbra.Visible;
            }
            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.L))
            {
                Penumbra.Debug = !Penumbra.Debug;
            }


            if (keyboardState.WasKeyPressed(Keys.OemPlus))
            {
                EntityFactory.PartyLight.Scale += new Vector2(50,50);
            }
            if (keyboardState.WasKeyPressed(Keys.OemMinus))
            {
                EntityFactory.PartyLight.Scale -= new Vector2(50, 50);
            }

            if (keyboardState.WasKeyPressed(Keys.D0))
            {
                EntityFactory.PartyLight.Radius += 10;
            }
            if (keyboardState.WasKeyPressed(Keys.D9))
            {
                EntityFactory.PartyLight.Radius -= 10;
            }

            AdjustZoom();
        }
    }
}
