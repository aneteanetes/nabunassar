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
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Monogame.Viewport;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Nabunassar.Widgets;
using Penumbra;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        private RenderTarget2D _backBuffer;
        private RenderTarget2D _grayscaleMapBuffer;
        private Effect _grayscaleMapShader;

        public LocalizedStrings Strings { get; set; }

        public WidgetFactory WidgetFactory { get; set; }

        public PenumbraComponent Penumbra { get; set; }

        public bool IsMouseMoveAvailable { get; internal set; } = true;

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

        public Desktop Desktop = null;

        public AudioOptions Audio { get; set; } = new AudioOptions();

        internal GraphicsDeviceManager graphics;

		public GameSettings Settings { get; private set; }

        public Matrix ResolutionMatrix;
        private Point originSize { get; set; }

        private Microsoft.Xna.Framework.Vector3 Scale = default;

        public PossibleResolution Resolution { get; set; }

        public static System.Numerics.Matrix4x4 ResolutionScaleMatrix { get; set; }
        public static Action<PossibleResolution> ChangeResolution { get; set; }

        public bool _isControlsBlocked { get; set; }

        public new NabunassarContentManager Content => base.Content.As<NabunassarContentManager>();

        public ResourceLoader ResourceLoader { get; set; }

        public SpriteBatchManager SpriteBatch { get; private set; }

        public readonly ScreenManager ScreenManager;
    }
}
