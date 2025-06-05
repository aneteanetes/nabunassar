using AssetManagementBase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Myra;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Content.Compiler;
using Nabunassar.Interface.Menu;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.Viewport;
using System.Reflection;
using Point = Microsoft.Xna.Framework.Point;

namespace Nabunassar
{
    internal partial class NabunassarGame : Game
    {
        public NabunassarGame(GameSettings settings)
        {
            settings.PathBin = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            settings.PathRepository = Directory.GetParent(settings.PathProject).ToString();
            settings.PathData = Path.Combine(settings.PathBin, "Data");

#if DEBUG
            ResourceCompiler.Compile(settings);
#endif

            Settings = settings;

            InactiveSleepTime = settings.DropFpsOnUnfocus;

            Resolution = new PossibleResolution()
            {
                Width = settings.OriginWidthPixel,
                Height = settings.OriginHeightPixel
            };

            try
            {
                SDL_InitMonitors();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SDL cant be inited in release mode, TODO: dev build");
            }

            GraphicsDeviceManagerInitialization();

            var audioVolume= (float)Audio.Volume;
            MediaPlayer.Volume = audioVolume;

            this.Activated += (_, __) =>
            {
                _isControlsBlocked = false;
                MediaPlayer.Volume = audioVolume;
            };

            this.Deactivated += (_, __) =>
            {
                _isControlsBlocked = true;
                audioVolume = MediaPlayer.Volume;
                MediaPlayer.Volume = 0;
            };

            Window.AllowUserResizing = false;
            Window.Position = new Microsoft.Xna.Framework.Point(500, 500);
            //Window.IsBorderless = true;

            
            IsMouseVisible = true;

            // fixing framerate
            this.IsFixedTimeStep = false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);
        }

        protected virtual void GraphicsDeviceManagerInitialization()
        {
            var monitor = MonitorBounds.ElementAtOrDefault(Settings.MonitorIndex);
            if (monitor.w == 0)
                monitor = MonitorBounds.ElementAtOrDefault(0);

            if (Settings.WidthHeightAutomated && monitor.w != originSize.X && (Settings.WindowMode == WindowMode.FullScreenSoftware || Settings.WindowMode == WindowMode.WindowedScaled))
            {
                Settings.WidthPixel = monitor.w;
                Settings.HeightPixel = monitor.h;
                            }

            originSize = new Point(Settings.OriginWidthPixel, Settings.OriginHeightPixel);
            var size = new Point(Settings.WidthPixel, Settings.HeightPixel);

            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = Settings.WindowMode == WindowMode.FullScreenSoftware || Settings.WindowMode == WindowMode.FullScreenHardware,
                PreferredBackBufferWidth = Settings.WidthPixel,
                PreferredBackBufferHeight = Settings.HeightPixel,
                SynchronizeWithVerticalRetrace = false,// settings.VerticalSync,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            };

            ResolutionMatrix = Matrix.Identity;

            bool scaling = false;
            Point left = Point.Zero;
            Point right = Point.Zero;


            if (originSize.X > Settings.WidthPixel)
            {
                scaling = true;
                left = originSize;
                right = size;
            }
            else if (originSize.X < Settings.WidthPixel)
            {
                scaling = true;
                left = size;
                right = originSize;
            }

            if (scaling)
            {
                var scaleX = left.X / (float)right.X;
                var scaleY = left.Y / (float)right.Y;

                var scale = Scale = new Vector3(scaleX, scaleY, 1);

                ResolutionMatrix = Matrix.CreateScale(scale);
            }
            else Scale = new Vector3(1, 1, 1);

            ResolutionScaleMatrix =
                new System.Numerics.Matrix4x4(
                    ResolutionMatrix.M11,
                    ResolutionMatrix.M12,
                    ResolutionMatrix.M13,
                    ResolutionMatrix.M14,
                    ResolutionMatrix.M21,
                    ResolutionMatrix.M22,
                    ResolutionMatrix.M23,
                    ResolutionMatrix.M24,
                    ResolutionMatrix.M31,
                    ResolutionMatrix.M32,
                    ResolutionMatrix.M33,
                    ResolutionMatrix.M34,
                    ResolutionMatrix.M41,
                    ResolutionMatrix.M42,
                    ResolutionMatrix.M43,
                    ResolutionMatrix.M44);

            ChangeResolution += r =>
            {
                graphics.PreferredBackBufferWidth = r.Width;
                graphics.PreferredBackBufferHeight = r.Height;
                graphics.ApplyChanges();
                Resolution = r;
                var scale = Matrix.CreateScale(new Vector3(originSize.X / (float)r.Width, originSize.Y / (float)r.Height, 1));
            };

            if (Settings.WindowMode == WindowMode.FullScreenSoftware)
            {
                graphics.HardwareModeSwitch = false;
            }

            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            this.Window.Title = Settings.GameTitle;

            //Window.TextInput += OnTextInput;

            SetMonitor(Settings.MonitorIndex);

            var state = GamePad.GetState(0);
            IsMouseVisible = !state.IsConnected;
            Settings.IsGamePadConnected = state.IsConnected;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ResourceLoader = new ResourceLoader(this);
            base.Content = new NabunassarContentManager(this, ResourceLoader);
            MyraEnvironment.Game = this;
            MyraEnvironment.DefaultAssetManager = new AssetManager(new MyraAssetAccessor(ResourceLoader), Settings.PathData);

            Desktop = new Desktop();
            Desktop.Root = new MainMenu(this);

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Desktop.Render();

            base.Draw(gameTime);
        }
    }
}