using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using Myra;

namespace ioi
{
    internal partial class GameHost
    {
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

#if DEBUG
            DebugUpdate(gameTime);
            UpdateResources();
#endif

            Lua.Update(gameTime);

            foreach (var feature in FeatureValues)
            {
                feature.Update(gameTime);
            }

            if (IsGameActive)
            {
                GameState?.Update(gameTime);

                if (PostProcessShaders.Count > 0)
                {
                    foreach (var postProcessor in PostProcessShaders)
                    {
                        postProcessor.Update(gameTime);
                    }
                }
            }

            MouseExtended.Update();
            KeyboardExtended.Update();

            Penumbra.Transform = CameraMain.GetViewMatrix();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FrameCounter.Update(deltaTime, gameTime.IsRunningSlowly);

            const float movementSpeed = 200;
            CameraMain.Move(MoveCamera() * movementSpeed * gameTime.GetElapsedSeconds());

            var mouseState = Mouse.GetState();
            _mousePosition = new Vector2(mouseState.X, mouseState.Y);
            _worldPosition = CameraMain.ScreenToWorld(_mousePosition);

            Game.MyraDesktop.Update();

            if (IsGameActive)
                Game.MyraDesktopIngame.Update();

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
                EntityFactoryMap.PartyLight.Scale += new Vector2(50,50);
            }
            if (keyboardState.WasKeyPressed(Keys.OemMinus))
            {
                EntityFactoryMap.PartyLight.Scale -= new Vector2(50, 50);
            }

            if (keyboardState.WasKeyPressed(Keys.D0))
            {
                EntityFactoryMap.PartyLight.Radius += 10;
            }
            if (keyboardState.WasKeyPressed(Keys.D9))
            {
                EntityFactoryMap.PartyLight.Radius -= 10;
            }

            AdjustZoom();
        }
    }
}
