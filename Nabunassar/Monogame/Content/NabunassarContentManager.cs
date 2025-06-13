using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Content;
using Nabunassar.Tiled.Map;
using Newtonsoft.Json;
using System.IO;

namespace Nabunassar.Monogame.Content
{
    internal class NabunassarContentManager : ContentManager
    {
        private ResourceLoader _resourceLoader;
        private JsonSerializer _jsonSerializer;
        NabunassarGame _game;

        private Dictionary<string, FontSystem> _fonts = new();

        public NabunassarContentManager(NabunassarGame game, ResourceLoader resourceLoader) : base(game.Services)
        {
            _game = game;
            _resourceLoader = resourceLoader;
            _jsonSerializer = JsonSerializer.CreateDefault();
        }

        private bool disposed => this.GetPropValue<bool>("disposed");


        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                var stream = OpenStream(assetName);
                var texture = Texture2D.FromStream(_game.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
                LoadedAssets[assetName] = texture;
                return texture.As<T>();
            }

            if (typeof(T) == typeof(TiledMap))
            {
                var stream = OpenStream(assetName);
                var map = TiledMap.Load(stream);
                return map.As<T>();
            }

            if (assetName.EndsWith(".json"))
            {
                var stream = OpenStream(assetName);
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    return _jsonSerializer.Deserialize<T>(jsonTextReader);
                }
            }

            return base.Load<T>(assetName);
        }

        public FontSystem LoadFont(string assetName)
        {
            if (!_fonts.ContainsKey(assetName))
            {
                var font = Load(assetName);

                var fontSettings = new FontSystemSettings()
                {
                    FontResolutionFactor = 2,

                };
                var fontSystem = new FontSystem(fontSettings);
                fontSystem.AddFont(font);

                _fonts[assetName] = fontSystem;
            }

            return _fonts[assetName];
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
            _fonts.ForEach(f => f.Value.Dispose());
            _fonts.Clear();
            base.Dispose(disposing);
        }
    }
}