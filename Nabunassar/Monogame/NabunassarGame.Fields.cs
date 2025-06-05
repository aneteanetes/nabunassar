using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Nabunassar.Content;
using Nabunassar.Monogame.Settings;
using Nabunassar.Monogame.Viewport;
using System.Numerics;

namespace Nabunassar
{
    internal partial class NabunassarGame
	{
        public Desktop Desktop = null;

        public AudioOptions Audio { get; set; } = new AudioOptions();

        internal GraphicsDeviceManager graphics;

		public GameSettings Settings { get; private set; }

        public Matrix ResolutionMatrix;
        private Point originSize { get; set; }

        private Microsoft.Xna.Framework.Vector3 Scale = default;

        public PossibleResolution Resolution { get; set; }

        public static Matrix4x4 ResolutionScaleMatrix { get; set; }
        public static Action<PossibleResolution> ChangeResolution { get; set; }

        public bool _isControlsBlocked { get; set; }

        public new NabunassarContentManager Content => base.Content.As<NabunassarContentManager>();

        public ResourceLoader ResourceLoader { get; set; }
    }
}
