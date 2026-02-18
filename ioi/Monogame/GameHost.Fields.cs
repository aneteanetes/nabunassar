using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using Myra.Graphics2D.UI;
using ioi.Content;
using ioi.Entities;
using ioi.Entities.Data;
using ioi.Localization;
using ioi.Monogame.Content;
using ioi.Monogame.Extended;
using ioi.Monogame.Interfaces;
using ioi.Monogame.Settings;
using ioi.Monogame.SpriteBatch;
using ioi.Monogame.Viewports;
using ioi.Resources;
using ioi.Struct;
using ioi.Widgets;
using Penumbra;
using System.Runtime.InteropServices;
using ioi.Monogame.Cameras;

namespace ioi
{
    internal partial class GameHost
    {
        public HashSet<IFeatured> FeatureValues { get; internal set; } = new();

        public static GameLoopFeatureValue<bool> IsMakingScreenShot;

        public RenderTarget2D _screenShotTarget = null;
        public RenderTarget2D _shareTarget = null;

        public Texture2D PixelTexture { get; private set; }
        public OSPlatform OSPlatform { get; private set; }

        private bool _screenLoaded = false;

        public RenderTarget2D _backBuffer;

        public LocalizedStrings Strings { get; set; }

        public WidgetFactory WidgetFactory { get; set; }

        public PenumbraComponent Penumbra { get; set; }

        public GameLoopFeatureValue<bool> IsMouseMoveAvailable { get; internal set; }

        public bool IsGameActive { get; set; } = true;

        public DataBase DataBase { get; set; }

        public CustomCollisionComponent CollisionComponent { get; private set; }

        private static FastRandom _random;

        public static FastRandom Randoms
        {
            get
            {
                if (_random == null)
                {
                    try
                    {
                        _random = new FastRandom(Guid.NewGuid().GetHashCode());
                    }
                    catch
                    {
                        //safe, but ineffective (and less random) way
                        _random = new FastRandom(int.Parse(new string(DateTime.UtcNow.Ticks.ToString().Take(8).ToArray())));
                    }
                }

                return _random;
            }
        }

        public FastRandom Random => Randoms;

        public GameState GameState { get; private set; }

        public Vector2 _worldPosition;
        public Vector2 _mousePosition;

        private bool isDrawFPS = false;
        private bool isDrawCoords = false;

        public FrameCounter FrameCounter;

        //public ECS.ESCWorld World { get; private set; }

        public MapEntityFactory EntityFactoryMap { get; set; }

        public BattleEntityFactory EntityFactoryBattle { get; set; }

        public World WorldMap { get; private set; }

        public World WorldBattle { get; private set; }

        public OrthographicCameraCustom CameraMain { get; private set; }

        public OrthographicCameraCustom CameraMap { get; private set; }

        public Desktop MyraDesktop { get; private set; }

        public AudioOptions Audio { get; set; } = new AudioOptions();

        internal GraphicsDeviceManager graphics;

		public GameSettings Settings { get; private set; }

        public Matrix ResolutionMatrix;
        private Point originSize { get; set; }

        public Microsoft.Xna.Framework.Vector3 Scale { get; private set; }

        public PossibleResolution Resolution { get; set; }

        public int ViewportProportion { get; set; }

        public static System.Numerics.Matrix4x4 ResolutionScaleMatrix { get; set; }
        public static Action<PossibleResolution> ChangeResolution { get; set; }

        public bool _isControlsBlocked { get; set; }

        public new GameContentManager Content => base.Content.As<GameContentManager>();

        public ResourceLoader ResourceLoader { get; set; }

        public SpriteBatchManager SpriteBatch { get; private set; }

        public readonly CustomScreenManager ScreenManager;
    }
}
