using FontStashSharp;
using FontStashSharp.RichText;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Content;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Speaking;
using Nabunassar.Entities.Json;
using Nabunassar.Entities.Struct.ImageRegions;
using Nabunassar.Tiled.Map;
using Newtonsoft.Json;

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

            InitJsonSerializer();

            RichTextDefaults.FontResolver = fontName_ttf =>
            {
                var dotIndex = fontName_ttf.IndexOf(",");
                var font = fontName_ttf.Substring(0, dotIndex);
                var size = int.Parse(fontName_ttf.Substring(dotIndex+1).TrimStart());

                return this.LoadFont(font).GetFont(size);
            };

            RichTextDefaults.ImageResolver = imageName =>
            {
                var texture = this.Load<Texture2D>(imageName);
                return new TextureFragment(texture);
            };
        }

        private void InitJsonSerializer()
        {
            var settings = new JsonSerializerSettings();

            settings.Converters.Add(new DiceJsonConverter());
            settings.Converters.Add(new RankJsonConverter());
            settings.Converters.Add(new RankDiceJsonConverter());
            settings.Converters.Add(new ImageRegionJsonConverter());
            settings.Converters.Add(new MoneyJsonConverter());

            _jsonSerializer = JsonSerializer.CreateDefault(settings);
        }

        public override T Load<T>(string assetName)
        {
            if (LoadedAssets.TryGetValue(assetName, out var value) && value is T)
            {
                return (T)value;
            }

            if (typeof(T) == typeof(Texture2D))
            {
                return LoadTextureInternal<T>(assetName);
            }

            if (typeof(T) == typeof(TiledMap))
            {
                var stream = OpenStream(assetName);
                var map = TiledMap.Load(stream,assetName);
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

            if(typeof(T)==typeof(string))
            {
                var stream = OpenStream(assetName);
                var @string = stream.AsString();
                LoadedAssets[assetName] = @string;
                return @string.As<T>();
            }    

            return base.Load<T>(assetName);
        }

        private T LoadTextureInternal<T>(string assetName)
        {
            bool isFound = false;

            var originalAssetName = assetName;

            if (!_resourceLoader.IsExists(assetName))
            {
                assetName = Path.Combine(Path.GetDirectoryName(assetName), Path.GetFileNameWithoutExtension(assetName)).Replace("\\", "/");

                if (_resourceLoader.IsExists(assetName + ".png"))
                {
                    assetName += ".png";
                    isFound = true;
                }

                if (_resourceLoader.IsExists(assetName + ".jpg"))
                {
                    assetName += ".jpg";
                    isFound = true;
                }

                if (_resourceLoader.IsExists(assetName + ".jpeg"))
                {
                    assetName += ".jpeg";
                    isFound = true;
                }
            }
            else
            {
                isFound = true;
            }

            if(!isFound)
            {
                throw new ContentLoadException($"File asset not found: {originalAssetName}");
            }

            var stream = OpenStream(assetName);
            var texture = Texture2D.FromStream(_game.GraphicsDevice, stream, DefaultColorProcessors.PremultiplyAlpha);
            LoadedAssets[assetName] = texture;
            return texture.As<T>();
        }

        public Texture2D LoadTexture(string assetName) => Load<Texture2D>(assetName);

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
                var fontSystem = new FontSystem(/*fontSettings*/);
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
            => _resourceLoader.GetStream(assetName.Replace("\\", "/"));

        protected override void Dispose(bool disposing)
        {
            _resourceLoader?.Dispose();
            _fonts.ForEach(f => f.Value.Dispose());
            _fonts.Clear();
            base.Dispose(disposing);
        }
    }
}