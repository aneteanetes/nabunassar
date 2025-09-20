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
        public static GaussianBlur GlobalBlurShader;

        private bool logWindow = false;

        public MainGameScreen(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            if (GlobalBlurShader == null)
                GlobalBlurShader = new GaussianBlur(base.Game, 1.5f);

            Game.Camera.Zoom = 4;
            Game.Camera.Origin = new Vector2(0, 0);
            Game.Camera.Position = new Vector2(0, 0);
            Game.Camera.SetBounds(Vector2.Zero, new Vector2(205, 115));

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
            if (!Game.IsGameActive)
                return;

            Game.CollisionComponent?.Update(gameTime);
            Game.MapWorld?.Update(gameTime);

            GlobalMapGameControls();
        }

        private void GlobalMapGameControls()
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (!Game.GameState.EscapeSwitch)
                    SettingsIconButton.OpenCloseSettings(Game);
                else
                    Game.GameState.EscapeSwitch = false;
            }


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
            GlobalBlurShader.Disable();

            var game = Game;
            game.Camera.Zoom = 1;
            game.Camera.Origin = new Vector2(0, 0);
            game.Camera.Position = new Vector2(0, 0);
        }
    }
}