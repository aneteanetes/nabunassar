﻿global using Microsoft.Xna.Framework;
global using Point = Microsoft.Xna.Framework.Point;
global using GameObject = Nabunassar.Components.GameObject;

using AssetManagementBase;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Collisions.QuadTree;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;
using Myra;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Content.Compiler;
using Nabunassar.Desktops;
using Nabunassar.ECS;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Monogame.Viewport;
using Nabunassar.Resources;
using Nabunassar.Screens;
using Nabunassar.Screens.Abstract;
using Nabunassar.Struct;
using Nabunassar.Systems;
using System.Reflection;

namespace Nabunassar
{
    internal partial class NabunassarGame : Game
    {
        public static NabunassarGame Game { get; private set; }

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

            ScreenManager = new MonoGame.Extended.Screens.ScreenManager();
            this.Components.Add(this.ScreenManager);
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
            Game = this;
            this.Window.Title = Settings.GameTitle;

            //Window.TextInput += OnTextInput;

            SetMonitor(Settings.MonitorIndex);

            var state = GamePad.GetState(0);
            IsMouseVisible = !state.IsConnected;
            Settings.IsGamePadConnected = state.IsConnected;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice,Resolution.Width, Resolution.Height);
            Camera = new OrthographicCamera(viewportAdapter);

            DataBase=new DataBase(this);

            Random = new FastRandom();
            var quadTreeBounds = new RectangleF(0, 0, Resolution.Width, Resolution.Height);
            CollisionComponent = new CustomCollisionComponent(quadTreeBounds);

            var objectsLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("objects", objectsLayer);

            var groundLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("ground", groundLayer);

            var cursorLayer = new Layer(new QuadTreeSpace(quadTreeBounds));
            CollisionComponent.Add("cursor", cursorLayer);

            CollisionComponent.AddCollisionBetweenLayer(cursorLayer, objectsLayer);

            World = new WorldBuilder()
                .AddSystem(new PlayerControllSystem(this))
                .AddSystem(new RenderSystem(this))
                .AddSystem(new CursorSystem(this))
                .AddSystem(new MoveSystem(this))
                .AddSystem(new MouseControlSystem(this))
                .Build();

            EntityFactory=new Entities.EntityFactory(this);

            base.Initialize();
        }

        private GameObject _myraGameObject;

        public void SwitchScreen<TScreen>(Transition transition = default)
            where TScreen : BaseGameScreen
        {
            var screen = typeof(TScreen).New(this).As<BaseGameScreen>();

            if (transition == default)
                transition = new FadeTransition(GraphicsDevice, Color.Black);

            if (DesktopWidget != null)
                DesktopWidget.Dispose();

            DesktopWidget = screen.GetWidget();
            DesktopWidget.LoadContent();
            Desktop.Root = DesktopWidget.Load();
            _myraGameObject = DesktopWidget.GameObject;

            ScreenManager.LoadScreen(screen, transition);
        }

        public void SwitchDesktop(ScreenWidget widget=null)
        {
            if (widget != null)
            {
                widget.LoadContent();
                Desktop.Root = widget.Load();
                _myraGameObject = widget.GameObject;
            }
            else
            {
                Desktop.Root = null;
            }
        }

        protected override void LoadContent()
        {
            FrameCounter = new FrameCounter();
            ResourceLoader = new ResourceLoader(this);
            base.Content = new NabunassarContentManager(this, ResourceLoader);
            SpriteBatch = new SpriteBatchManager(this, GraphicsDevice, Content);

            MyraEnvironment.Game = this;
            MyraEnvironment.DefaultAssetManager = new AssetManager(new MyraAssetAccessor(ResourceLoader), Settings.PathData);
            Desktop = new Desktop();

            SwitchScreen<MainMenuScreen>();

            Game.InitializeGameState();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
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
        
        // Add this to the Game1.cs file
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

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            //if (Desktop.Root != default && _myraGameObject!=default && _myraGameObject.Bounds.BoundingRectangle.Width==0)
            //{
            //    _myraGameObject.Bounds = new RectangleF(_myraGameObject.Bounds.Position, new SizeF(Desktop.Root.ActualBounds.Width, Desktop.Root.ActualBounds.Height));

            //    if (_myraGameObject.Bounds.BoundingRectangle.Width != 0)
            //        EntityFactory.AddCollistion(_myraGameObject);
            //}

            MouseExtended.Update();
            KeyboardExtended.Update();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FrameCounter.Update(deltaTime, gameTime.IsRunningSlowly);

            const float movementSpeed = 200;
            Camera.Move(MoveCamera() * movementSpeed * gameTime.GetElapsedSeconds());

            var mouseState = Mouse.GetState();
            _mousePosition = new Vector2(mouseState.X, mouseState.Y);
            _worldPosition = Camera.ScreenToWorld(_mousePosition);

            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.X))
                isDrawFPS = !isDrawFPS;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.X))
                isDrawCoords = !isDrawCoords;

            if (keyboardState.IsControlDown() && keyboardState.WasKeyPressed(Keys.B))
                IsDrawBounds = !IsDrawBounds;

            AdjustZoom();

            if (IsGameActive)
            {
                World.Update(gameTime);
                CollisionComponent.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            SpriteBatch.End();

            if (isDrawFPS)
                DrawFPS();

            if (isDrawCoords)
                DrawPositions();
        }

        public void DrawFPS()
        {
            var sb = BeginDraw(false);

            sb.DrawText(Fonts.Retron, 30, FrameCounter.ToString(), new Vector2(1, 1), Color.Yellow);

            sb.End();
        }

        public void DrawPositions()
        {
            var sb = BeginDraw(false);

            sb.DrawText(Fonts.Retron, 20, "World: " + _worldPosition.ToString(), new Vector2(50, 100), Color.PeachPuff);
            sb.DrawText(Fonts.Retron, 20, "Display: " + _mousePosition.ToString(), new Vector2(50, 125), Color.AntiqueWhite);

            sb.End();
        }
    }
}