global using Microsoft.Xna.Framework;
global using Point = Microsoft.Xna.Framework.Point;
using Geranium.Reflection;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;
using Nabunassar.Entities.Game.Calendars;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.Viewport;
using Nabunassar.Native;
using Nabunassar.Resources;
using Nabunassar.Screens.Abstract;
using Nabunassar.Struct;
using System.Runtime.InteropServices;

namespace Nabunassar
{
    internal partial class NabunassarGame : Game
    {
        public static NabunassarGame Game { get; private set; }

        public NabunassarGame(GameSettings settings)
        {
            Game = this;

            if (!settings.IsInitialized)
                throw new Exception("Settings is not initialized!");

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

            Window.AllowUserResizing = false;
            Window.Position = new Microsoft.Xna.Framework.Point(500, 500);
            //Window.IsBorderless = true;


            IsMouseVisible = true;

            // fixing framerate
            this.IsFixedTimeStep = false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);

            ScreenManager = new MonoGame.Extended.Screens.ScreenManager();
            ScreenManager.UpdateOrder = 10;
            this.Components.Add(this.ScreenManager);
        }

        protected virtual void GraphicsDeviceManagerInitialization()
        {
            var monitor = MonitorBounds.ElementAtOrDefault(Settings.MonitorIndex);
            if (monitor.w == 0)
                monitor = MonitorBounds.ElementAtOrDefault(0);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Game.OSPlatform = OSPlatform.Windows;
                var windowsScale = GetWindowsScreenScalingFactor(false);
                Settings.WidthPixel = ((int)(Settings.WidthPixel / windowsScale));
                Settings.HeightPixel = ((int)(Settings.HeightPixel / windowsScale));
            }
            else
            {
                Game.OSPlatform = OSPlatform.Linux;
            }

            if (Settings.WindowMode == WindowMode.FullScreenSoftware || Settings.WindowMode == WindowMode.WindowedScaled)
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

            SetMonitor(Settings.MonitorIndex);

            var state = GamePad.GetState(0);
            IsMouseVisible = !state.IsConnected;
            Settings.IsGamePadConnected = state.IsConnected;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Resolution.Width, Resolution.Height);
            Camera = new OrthographicCamera(viewportAdapter);

            DataBase = new DataBase(this);

            IsMouseMoveAvailable = new GameLoopFeatureValue<bool>(this, true);

            InitializeCollisions();

            InitGameWorlds();

            base.Initialize();
        }

        public void SwitchScreen<TScreen>(Transition transition = default)
            where TScreen : BaseGameScreen
        {
            _screenLoaded = false;

            var screen = typeof(TScreen).New(this).As<BaseGameScreen>();

            if (transition == default)
                transition = new FadeTransition(GraphicsDevice, Color.Black);

            transition.Completed += (s, e) => _screenLoaded = true;

            RemoveDesktopWidgets();

            var widget = screen.GetWidget();
            if (widget != default)
                AddDesktopWidget(widget);

            ScreenManager.LoadScreen(screen, transition);
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
                Camera.ZoomIn(zoomPerTick);
            }
            if (keyboardState.IsShiftDown() && state.IsKeyDown(Keys.X))
            {
                Camera.ZoomOut(zoomPerTick);
            }
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            PrintScreenHandler.End();
            base.OnExiting(sender, args);
        }
    }
}