global using Microsoft.Xna.Framework;
global using Point = Microsoft.Xna.Framework.Point;
using Geranium.Reflection;
using ioi.Monogame.Settings;
using ioi.Monogame.Viewports;
using ioi.Native;
using ioi.Resources;
using ioi.Screens.Abstract;
using ioi.Screens.LoadingScreens;
using ioi.Struct;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Input;
using System.Collections;

namespace ioi
{
    internal partial class GameHost : Game
    {
        public static GameHost Game { get; private set; }

        private Viewport viewportMap;
        public ViewportAdapterCustom viewportAdapter;

        public GameHost(GameSettings settings)
        {
            Game = this;

            if (!settings.IsInitialized)
                throw new Exception("Settings is not initialized!");

            Settings = settings;

            InactiveSleepTime = settings.DropFpsOnUnfocus;
                        
            try
            {
                SDL_InitMonitors();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SDL cant be inited in release mode, TODO: dev build, ex:{ex}");
            }

            GraphicsDeviceManagerInitialization();

            var audioVolume = (float)Audio.Volume;
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

            Window.AllowUserResizing = true;
            Window.Position = new Microsoft.Xna.Framework.Point(500, 500);
            //Window.IsBorderless = true;


            IsMouseVisible = true;

            // fixing framerate
            this.IsFixedTimeStep = true;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);

            ScreenManager = new Monogame.Extended.CustomScreenManager(this);
            ScreenManager.UpdateOrder = 10;
            this.Components.Add(this.ScreenManager);
        }

        int GetBestBaseResolution(int screenWidth, int screenHeight)
        {
            // Приводим к 16:9
            int viewportHeight = screenHeight;
            int viewportWidth = screenHeight * 16 / 9;

            // Проверяем 1920×1080
            int scale1080 = Math.Min(viewportWidth / 1920, viewportHeight / 1080);

            // Проверяем 1280×720  
            int scale720 = Math.Min(viewportWidth / 1280, viewportHeight / 720);

            // Выбираем то, где scale больше (меньше чёрных полос)
            if (scale1080 >= scale720 && scale1080 >= 1)
                return 1080; // используем 1920×1080
            else if (scale720 >= 1)
                return 720;  // используем 1280×720
            else
                return 1080; // fallback
        }

        protected virtual void GraphicsDeviceManagerInitialization()
        {
            var monitor = MonitorBounds.ElementAtOrDefault(Settings.MonitorIndex);
            if (monitor.w == 0)
                monitor = MonitorBounds.ElementAtOrDefault(0);

            var width = 0;
            var height = 0;

            if (Settings.WindowMode is WindowMode.FullScreenSoftware or WindowMode.FullScreenHardware)
            {
                width = monitor.w;
                height = monitor.h;
            }
            else
            {
                width = Settings.WidthPixel;
                height = Settings.HeightPixel;
            }
            var resolution = ViewportProportion = GetBestBaseResolution(width, height);
            //if (resolution == 720)
            //{
            //    Settings.OriginWidthPixel = 1280;
            //    Settings.OriginHeightPixel = 720;
            //}
            //else
            //{
                Settings.OriginWidthPixel = 1920;
                Settings.OriginHeightPixel = 1080;
            //}
            Settings.WidthPixel = width;
            Settings.HeightPixel = height;

            originSize = new Point(Settings.OriginWidthPixel, Settings.OriginHeightPixel);

            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = Settings.WindowMode == WindowMode.FullScreenSoftware || Settings.WindowMode == WindowMode.FullScreenHardware,
                PreferredBackBufferWidth = Settings.WidthPixel,
                PreferredBackBufferHeight = Settings.HeightPixel,
                SynchronizeWithVerticalRetrace = Game.Settings.VerticalSync,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            };

            if (Settings.WindowMode == WindowMode.FullScreenSoftware)
            {
                graphics.HardwareModeSwitch = false;
            }

            graphics.SynchronizeWithVerticalRetrace = Game.Settings.VerticalSync;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            this.Window.Title = Settings.GameTitle;

            SetMonitor(Settings.MonitorIndex);

            var state = GamePad.GetState(0);
            //IsMouseVisible = !state.IsConnected;
            Settings.IsGamePadConnected = state.IsConnected;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            InitializeCameras();

            DataBase = new DataBase(this);

            IsMouseMoveAvailable = new GameLoopFeatureValue<bool>(this, true);

            base.Initialize();
        }

        public void SwitchScreen<TScreen>(IEnumerator loadingMethod = default, IScreenTransition transition = default)
             where TScreen : BaseScreen
        => SwitchScreen(typeof(TScreen).New(this).As<TScreen>(), loadingMethod, transition);

        public void SwitchScreen<TScreen>(TScreen screen, IEnumerator loadingMethod = null, IScreenTransition transition = default)
             where TScreen : BaseScreen
        {
            _screenLoaded = false;

            var loadingScreen = new BaseLoadingScreen(this)
            {
                LoadingCorutine = loadingMethod,
                NextScreen = screen
            };

            if (transition == default)
                transition = new FadeScreenTransition(GraphicsDevice, Color.Black);

            transition.Completed += (s, e) => loadingScreen.TransitionCompleted();
            ScreenManager.LoadScreen(loadingScreen, transition);
        }

        public void SwitchScreenInternal(BaseScreen screen, IScreenTransition transition = default)
        {
            _screenLoaded = false;

            if (transition == default)
                transition = new FadeScreenTransition(GraphicsDevice, Color.Black);

            transition.Completed += (s, e) => _screenLoaded = true;

            ScreenManager.LoadScreen(screen, transition);
        }

        protected override void Dispose(bool disposing)
        {
            GameController.GlobalBlurShader?.Disable();
            base.Dispose(disposing);
        }

        private Vector2 MoveCamera()
        {
            var movementDirection = Vector2.Zero;
            var state = KeyboardExtended.GetState();
            if (state.IsControlDown() && state.IsKeyDown(Keys.Up))
            {
                movementDirection += Vector2.UnitY;
            }
            if (state.IsControlDown() && state.IsKeyDown(Keys.Down))
            {
                movementDirection -= Vector2.UnitY;
            }
            if (state.IsControlDown() && state.IsKeyDown(Keys.Right))
            {
                movementDirection -= Vector2.UnitX;
            }
            if (state.IsControlDown() && state.IsKeyDown(Keys.Left))
            {
                movementDirection += Vector2.UnitX;
            }
            return movementDirection;
        }

        private void AdjustZoom()
        {
            var state = Keyboard.GetState();
            var keyboardState = KeyboardExtended.GetState();
            float zoomPerTick = 0.001f;
            if (keyboardState.IsShiftDown() && state.IsKeyDown(Keys.Z))
            {
                CameraMain.ZoomIn(zoomPerTick);
            }
            if (keyboardState.IsShiftDown() && state.IsKeyDown(Keys.X))
            {
                CameraMain.ZoomOut(zoomPerTick);
            }
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            PrintScreenHandler.End();
            base.OnExiting(sender, args);
        }
    }
}