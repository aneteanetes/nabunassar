using ioi.Tiled.Map;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MoonSharp.Interpreter;
using System.Collections;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class BorderLayersSystem
    {
        public GameHost Game { get; }

        private Dictionary<string, Layer> layers = new();

        private List<PolyTile> interfaceSprites;
        public Effect CelshadingGlobal { get; private set; }

        public BorderLayersSystem(GameHost game)
        {
            Game = game;
        }

        public bool this[string layer]
        {
            get => layers[layer].IsVisible;
            set => layers[layer].IsVisible = value;
        }

        public IEnumerator LoadContent()
        {
            CelshadingGlobal = Game.Content.Load<Effect>("Assets/Shaders/CelshadingGlobal.fx");
            var @interface = Game.Content.Load<TiledMap>("Assets/Maps/interface2.tmx");
            LoadTiled(@interface);
            yield return 0;
        }

        private void LoadTiled(TiledMap map)
        {
            foreach (var tileset in map.Tilesets)
            {
                var texture = Game.Content.Load<Texture2D>(tileset.image);
                var _atlas = Texture2DAtlas.Create(tileset.name, texture, tileset.tilewidth, tileset.tileheight);
                tileset.TextureAtlas = _atlas;
            }

            var gray = Color.Wheat;

            foreach (var layer in map.Layers)
            {
                var borderLayer = new Layer()
                {
                    Name = layer.name
                };

                borderLayer.Tiles = layer.Tiles.Where(poly => poly.Gid > 0)
                    .Select(poly =>
                    {
                        var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.Gid - 1);
                        var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                        var position = poly.Position;// new Vector2(poly.Position.X* size.X, poly.Position.Y* size.Y);

                        _sprite.Color = gray;

                        return new PolyTile()
                        {
                            Sprite = _sprite,
                            Position = position,
                        };
                    }).ToList();

                layers[layer.name] = borderLayer;
            }

            //controls from object layer
            var objs = map.Objects.Select(poly =>
            {
                var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.gid - 1);
                var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                var position = poly.Position;// new Vector2(poly.Position.X* size.X, poly.Position.Y* size.Y);

                _sprite.Color = gray;

                return new PolyTile()
                {
                    Sprite = _sprite,
                    Position = position,
                };
            });

            layers["Controls"].Tiles.AddRange(objs);
        }

        public void Draw(GameTime gameTime)
        {
            CelshadingGlobal.Parameters["ScreenSize"].SetValue(new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height));

            var sb = Game.BeginDraw(effect: CelshadingGlobal);

            foreach (var layerinfo in layers)
            {
                if (!layerinfo.Value.IsVisible)
                    continue;

                var layer = layerinfo.Value;

                DrawSprites(sb, layer.Tiles);
            }

            sb.End();
        }

        private void DrawSprites(Monogame.SpriteBatch.SpriteBatchKnowed sb, List<PolyTile> tiles)
        {
            var offset = Vector2.Transform(new Vector2(0, 8), Game.CameraMain.GetViewMatrix());
            foreach (var sprite in tiles)
            {
                sprite.Sprite.Draw(sb, sprite.Position /*- offset*/, 0, Vector2.One);
            }
        }

        private class Layer
        {
            public string Name { get; set; }

            public bool IsVisible { get; set; }

            public List<PolyTile> Tiles { get; set; }
        }

        private class PolyTile
        {
            public Sprite Sprite { get; set; }

            public Vector2 Position { get; set; }
        }
    }
}
