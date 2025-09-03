using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Myra;
using Nabunassar.Entities.Struct;
using Nabunassar.Extensions.Texture2DExtensions;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            foreach (var feature in FeatureValues)
            {
                feature.Update(gameTime);
            }

            if (IsMakingScreenShot == false)
            {
                var path = _screenShotTarget.MakeScreenshot();
                Game.GameState.AddMessage(DrawText.Create("").Color(Color.Yellow).Append(Game.Strings["UI"]["Screenshot was created"] + $": {path}"));
                IsMakingScreenShot = null;
            }

            if (IsGameActive)
                GameState.Update(gameTime);
#if DEBUG
            DebugUpdate(gameTime);
#endif

            MouseExtended.Update();
            KeyboardExtended.Update();

            Penumbra.Transform = Camera.GetViewMatrix();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FrameCounter.Update(deltaTime, gameTime.IsRunningSlowly);

            const float movementSpeed = 200;
            Camera.Move(MoveCamera() * movementSpeed * gameTime.GetElapsedSeconds());

            var mouseState = Mouse.GetState();
            _mousePosition = new Vector2(mouseState.X, mouseState.Y);
            _worldPosition = Camera.ScreenToWorld(_mousePosition);

            Game.MyraDesktop.Update();

            if (IsGameActive)
            {
                CollisionComponent.Update(gameTime);
                WorldGame.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public void DebugUpdate(GameTime gameTime)
        {
            light.Position = MouseExtended.GetState().Position.ToVector2();

            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.X))
                isDrawFPS = !isDrawFPS;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.X))
                isDrawCoords = !isDrawCoords;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.B))
            {
                IsDrawBounds = !IsDrawBounds;
                MyraEnvironment.DrawWidgetsFrames = !MyraEnvironment.DrawWidgetsFrames;
            }

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
