using AssetManagementBase;
using ioi.Content;
using ioi.ECS;
using ioi.Localization;
using ioi.Monogame.Content;
using ioi.Monogame.Interfaces;
using ioi.Monogame.SpriteBatch;
using ioi.Native;
using ioi.Screens;
using ioi.Screens.LoadingScreens;
using ioi.Struct;
using ioi.Widgets;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using Myra;
using Myra.Graphics2D.UI;

namespace ioi
{
    internal partial class GameHost
    {

        protected override void LoadContent()
        {
            var x = SelectedMonitorBounds.w / 2 - Settings.WidthPixel / 2;
            var y = SelectedMonitorBounds.h / 2 - Settings.HeightPixel / 2;
            Window.Position = new Point(x, y);

            FrameCounter = new FrameCounter();
            ResourceLoader = new ResourceLoader(this.Settings);
            base.Content = new GameContentManager(this, ResourceLoader);
            SpriteBatch = new SpriteBatchManager(this, GraphicsDevice, Content);

            // myra  & desktops

            MyraEnvironment.Game = this;
            MyraEnvironment.EventHandlingModel = Myra.Events.EventHandlingStrategy.EventBubbling;
            MyraEnvironment.DefaultAssetManager = new AssetManager(new MyraAssetAccessor(ResourceLoader), Settings.PathData);
            MyraDesktop = new Desktop();
            
            viewportAdapter.Reset();
            MyraDesktop.ViewportAdapterFetcher = () =>
            {
                var matrix = viewportAdapter.GetScaleMatrix();
                //matrix.M41 = Game.MainViewport.X;
                //matrix.M42 = Game.MainViewport.Y;

                return new Myra.Graphics2D.MyraViewportAdapter()
                {
                    VirtualWidth = viewportAdapter.VirtualWidth,
                    VirtualHeight = viewportAdapter.VirtualHeight,
                    TransformMatrix = matrix
                };
            };

            WidgetFactory = new Widgets.WidgetFactory(this);
            WidgetFactory.LoadContent();
            ApplyMyraCustomStyle();

            // backbuffer
            _backBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            _screenShotTarget = new RenderTarget2D(GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            _shareTarget = new RenderTarget2D(GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            //

            // scripting
            Lua = new Scripting.LuaScripts(this);
            Lua.Init();

            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);

            Strings = new LocalizedStrings(this);

            SwitchScreen<MainMenuScreen>(GameController.LoadMainGame());

            GlowEffect.InitializeAndLoad(Content, GraphicsDevice);

            LoadPenumbra();

            PrintScreenHandler.Start();

            IsMakingScreenShot = new GameLoopFeatureValue<bool>(this, false);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }
}