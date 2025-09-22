using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Screens.Abstract;
using Nabunassar.Shaders;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;
using Nabunassar.Widgets.UserInterfaces.GameWindows;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Screens.Game
{
    internal class MainGameScreen : BaseGameScreen
    {
        private bool logWindow = false;

        public MainGameScreen(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            GameController.SetCameraToWorld();

            InitGameUI();

            base.LoadContent();
        }

        public void InitGameUI()
        {
            Game.AddDesktopWidget(new MinimapWindow(Game) { Position = new Vector2(Game.Resolution.Width, Game.Resolution.Height) });
            Game.AddDesktopWidget(new ChatWindow(Game));
            Game.AddDesktopWidget(new ControlPanel(Game));
            Game.AddDesktopWidget(new GameDateTime(Game));
            Game.AddDesktopWidget(new PartyConditions(Game));
            Game.AddDesktopWidget(new PartyPlates(Game));
        }

        public override void Update(GameTime gameTime)
        {
            GameController.CallGlobalMenu();

            if (!Game.IsGameActive)
                return;

            Game.CollisionComponent?.Update(gameTime);
            Game.MapWorld?.Update(gameTime);

            GlobalMapGameControls();
        }

        protected override void DrawInternal(GameTime gameTime)
        {
            Game.MapWorld.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.Penumbra.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        private void GlobalMapGameControls()
        {

            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
            {
                MinimapIconButton.OpenCloseMiniMap(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.I))
            {
                InventoryIconButton.OpenCloseInventory(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
            {
                AbilityIconButton.OpenCloseAbilities(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.L) && keyboardState.IsControlDown())
            {

                logWindow = !logWindow;
            }
        }

        public override void UnloadContent()
        {
            GameController.GlobalBlurShader.Disable();
            Game.RemoveDesktopWidgets(true);
        }
    }
}