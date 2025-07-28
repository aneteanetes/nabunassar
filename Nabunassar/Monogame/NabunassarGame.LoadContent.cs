using AssetManagementBase;
using MonoGame;
using Myra;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Localization;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Screens;
using Nabunassar.Struct;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        protected override void LoadContent()
        {
            var x = SelectedMonitorBounds.w / 2 - Settings.WidthPixel / 2;
            var y = SelectedMonitorBounds.h / 2 - Settings.HeightPixel / 2;
            Window.Position = new Microsoft.Xna.Framework.Point(x, y);

            FrameCounter = new FrameCounter();
            ResourceLoader = new ResourceLoader(this.Settings);
            base.Content = new NabunassarContentManager(this, ResourceLoader);
            SpriteBatch = new SpriteBatchManager(this, GraphicsDevice, Content);

            // myra  & desktops

            MyraEnvironment.Game = this;
            MyraEnvironment.DefaultAssetManager = new AssetManager(new MyraAssetAccessor(ResourceLoader), Settings.PathData);
            DesktopContainer = new Desktop();
            Desktop = new Panel();
            DesktopContainer.Widgets.Add(Desktop);
            Dialogues = new Widgets.WidgetFactory(this);

            //

            Strings = new LocalizedStrings(this);

            SwitchScreen<MainMenuScreen>();

            Game.InitializeGameState();

            GlowEffect.InitializeAndLoad(Content, GraphicsDevice);

            LoadPenumbra();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }
}