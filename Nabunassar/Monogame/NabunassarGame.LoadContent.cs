using AssetManagementBase;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using Myra;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.ECS;
using Nabunassar.Localization;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Native;
using Nabunassar.Screens;
using Nabunassar.Screens.LoadingScreens;
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
            Window.Position = new Point(x, y);

            FrameCounter = new FrameCounter();
            ResourceLoader = new ResourceLoader(this.Settings);
            base.Content = new NabunassarContentManager(this, ResourceLoader);
            SpriteBatch = new SpriteBatchManager(this, GraphicsDevice, Content);

            // myra  & desktops

            MyraEnvironment.Game = this;
            MyraEnvironment.EventHandlingModel = Myra.Events.EventHandlingStrategy.EventBubbling;
            MyraEnvironment.DefaultAssetManager = new AssetManager(new MyraAssetAccessor(ResourceLoader), Settings.PathData);
            MyraDesktop = new Desktop();

            var vectorScale = new Vector2(this.Scale.X, this.Scale.Y);
            //MyraEnvironment.Scale = vectorScale.NormalizedCopy();

            WidgetFactory = new Widgets.WidgetFactory(this);
            WidgetFactory.LoadContent();
            ApplyMyraCustomStyle();

            // backbuffer
            Game.Viewport = new Monogame.Viewport.PossibleResolution(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            _backBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.Viewport.Width, Game.Viewport.Height);
            _screenShotTarget = new RenderTarget2D(GraphicsDevice, Game.Resolution.Width, Game.Resolution.Height);

            //

            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);

            Strings = new LocalizedStrings(this);

            SwitchScreen<MainMenuScreen>(LoadLogos);

            GlowEffect.InitializeAndLoad(Content, GraphicsDevice);

            LoadPenumbra();

            PrintScreenHandler.Start();

            base.LoadContent();
        }

        private void LoadLogos()
        {
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }
}