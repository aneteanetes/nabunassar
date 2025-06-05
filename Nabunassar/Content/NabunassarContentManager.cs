using Geranium.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Nabunassar.Content
{
    internal class NabunassarContentManager : ContentManager
    {
        private ResourceLoader _resourceLoader;
        NabunassarGame _game;

        public NabunassarContentManager(NabunassarGame game, ResourceLoader resourceLoader) : base(game.Services)
        {
            _game = game;
            _resourceLoader = resourceLoader;
        }

        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                var stream = OpenStream(assetName);
                var texture = Texture2D.FromStream(_game.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
                LoadedAssets[assetName] = texture;
                return texture.As<T>();
            }

            return base.Load<T>(assetName);
        }

        public Stream Load(string assetName)
        {
            var stream = _resourceLoader.GetStream(assetName);
            LoadedAssets[assetName] = stream;
            return stream;
        }

        protected override Stream OpenStream(string assetName)
            => _resourceLoader.GetStream(assetName);

        protected override void Dispose(bool disposing)
        {
            _resourceLoader?.Dispose();
            base.Dispose(disposing);
        }
    }
}