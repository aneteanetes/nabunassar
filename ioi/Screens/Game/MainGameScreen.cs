using ioi.Screens.Abstract;
using ioi.Widgets.UserInterfaces;
using ioi.Widgets.UserInterfaces.GameWindows;
using ioi.Widgets.Views.IconButtons;
using MonoGame.Extended.Input;

namespace ioi.Screens.Game
{
    internal class MainGameScreen : BaseGameScreen
    {
        private bool logWindow = false;

        public MainGameScreen(GameHost game) : base(game)
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
            Game.AddDesktopWidget(new MinimapWindow(Game) { Position = new Vector2(Game.MainViewport.Width, Game.MainViewport.Height) });
            //Game.AddDesktopWidget(new ChatWindow(Game));
            Game.AddDesktopWidget(new ControlPanel(Game));
            Game.AddDesktopWidget(new GameDateTime(Game));
            Game.AddDesktopWidget(new PartyConditions(Game));
            Game.AddDesktopWidget(new PartyPlates(Game));
        }

        public override void Update(GameTime gameTime)
        {
            GameController.GlobalMenuWidget();

            if (!Game.IsGameActive)
                return;

            Game.CollisionComponent?.Update(gameTime);
            Game.OldECSMoveSystem?.Update(gameTime);

            GlobalMapGameControls();
        }

        protected override void DrawInternal(GameTime gameTime)
        {
            Game.OldECSMoveSystem.Draw(gameTime);

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