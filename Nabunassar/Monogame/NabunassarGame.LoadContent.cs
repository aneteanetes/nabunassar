using AssetManagementBase;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using Myra;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Localization;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Screens;
using Nabunassar.Struct;
using Nabunassar.Widgets;

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
            Desktop = new Desktop();
            WidgetFactory = new Widgets.WidgetFactory(this);
            WidgetFactory.LoadContent();
            ApplyMyraCustomStyle();

            // backbuffer
            
            _backBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.Resolution.Width, Game.Resolution.Height);
            _grayscaleMapBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.Resolution.Width, Game.Resolution.Height);

            // shaders
            _grayscaleMapShader = Content.Load<Effect>("Assets/Shaders/GrayscaleMap.fx");

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