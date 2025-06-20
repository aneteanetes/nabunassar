using Geranium.Reflection;
using Monogame.Extended;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Screens;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Desktops;
using Nabunassar.Entities;
using Nabunassar.Entities.Data;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Monogame.Viewport;
using Nabunassar.Resources;
using Nabunassar.Struct;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
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

        public World World { get; private set; }


        public OrthographicCamera Camera { get; private set; }

        public Desktop Desktop = null;

        public ScreenWidget DesktopWidget= null;    

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
