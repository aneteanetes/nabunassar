using Geranium.Reflection;
using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Entities;
using Nabunassar.Entities.Data;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Monogame.Viewport;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Penumbra;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets;
using Nabunassar.Localization;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public LocalizedStrings Strings { get; set; }

        public WidgetFactory Dialogues { get; set; }

        public PenumbraComponent Penumbra { get; set; }
        public bool IsMouseActive { get; internal set; } = true;

        public bool IsGameActive { get; set; } = true;

        public DataBase DataBase { get; set; }

        public CustomCollisionComponent CollisionComponent { get; private set; }

        public FastRandom Random { get; private set; }

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

        public Desktop DesktopContainer = null;

        public Panel Desktop = null;

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
