using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Entities;
using Nabunassar.Entities.Data;
using Nabunassar.Localization;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Monogame.Viewport;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Nabunassar.Widgets;
using Penumbra;
using System.Runtime.InteropServices;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public HashSet<IFeatured> FeatureValues { get; internal set; } = new();

        public static bool? IsMakingScreenShot = null;

        private RenderTarget2D _screenShotTarget = null;

        public Texture2D PixelTexture { get; private set; }
        public OSPlatform OSPlatform { get; private set; }

        private bool _screenLoaded = false;

        private RenderTarget2D _backBuffer;

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

        public EntityFactory EntityFactory { get; set; }

        public World WorldGame { get; private set; }

        public OrthographicCamera Camera { get; private set; }

        public Desktop Desktop { get; private set; }

        public AudioOptions Audio { get; set; } = new AudioOptions();

        internal GraphicsDeviceManager graphics;

		public GameSettings Settings { get; private set; }

        public Matrix ResolutionMatrix;
        private Point originSize { get; set; }

        public Microsoft.Xna.Framework.Vector3 Scale { get; private set; }

        public PossibleResolution Resolution { get; set; }

        public PossibleResolution Viewport {  get; set; }

        public static System.Numerics.Matrix4x4 ResolutionScaleMatrix { get; set; }
        public static Action<PossibleResolution> ChangeResolution { get; set; }

        public bool _isControlsBlocked { get; set; }

        public new NabunassarContentManager Content => base.Content.As<NabunassarContentManager>();

        public ResourceLoader ResourceLoader { get; set; }

        public SpriteBatchManager SpriteBatch { get; private set; }

        public readonly ScreenManager ScreenManager;
    }
}
