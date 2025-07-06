using FontStashSharp;
using Geranium.Reflection;
using info.lundin.math;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nabunassar.Content;
using Nabunassar.Entities.Data.Speaking;
using Nabunassar.Tiled.Map;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public override T Load<T>(string assetName)
        {
            if (LoadedAssets.TryGetValue(assetName, out var value) && value is T)
            {
                return (T)value;
            }

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
                LoadedAssets[assetName] = map;
                return map.As<T>();
            }

            if (assetName.EndsWith(".json"))
            {
                T jsonDeserialized = default;
                var stream = OpenStream(assetName);
                jsonDeserialized = ParseJsonStream<T>(stream);
                LoadedAssets[assetName] = jsonDeserialized;
                return jsonDeserialized;
            }

            if (typeof(T) == typeof(Effect))
            {
                var stream = OpenStream(assetName).As<MemoryStream>();
                var effect = new Effect(_game.GraphicsDevice, stream.ToArray()).As<T>();
                LoadedAssets[assetName]=effect;

                return effect;
            }

            return base.Load<T>(assetName);
        }

        private T ParseJsonStream<T>(Stream stream)
        {
            T jsonDeserialized;
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                jsonDeserialized = _jsonSerializer.Deserialize<T>(jsonTextReader);
            }

            return jsonDeserialized;
        }

        public Dialogue LoadDialogue(string dialoguePathName)
        {
            string assetName = dialoguePathName;

            if (!assetName.Contains("Data"))
                assetName = $"Data/Localization/{_game.Settings.LanguageCode}/Dialogues/" + dialoguePathName;

            var stream = OpenStream(assetName);
            var model = ParseJsonStream<DialogueModel>(stream);
            model.DialogueFile = dialoguePathName;

            return new Dialogue(_game,model);
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