using ioi.Screens.Abstract;
using ioi.Tiled.Map;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;

namespace ioi.Screens
{
    internal class RoguelikeScreen : BaseGameScreen
    {
        public RoguelikeScreen(GameHost game) : base(game) { }

        private TiledMap _tiledMap;
        private List<PolyTile> interfaceSprites;

        public override void LoadContent()
        {
            var @interface = Game.Content.Load<TiledMap>("Assets/Maps/interface2.tmx");
            interfaceSprites = LoadTiled(@interface);
        }

        private List<PolyTile> LoadTiled(TiledMap map)
        {
            foreach (var tileset in map.Tilesets)
            {
                if (tileset.name == "Hulls")
                    continue;

                var texture = Game.Content.Load<Texture2D>(tileset.image.Replace("colored-", ""));
                var _atlas = Texture2DAtlas.Create(tileset.name, texture, tileset.tilewidth, tileset.tileheight);
                tileset.TextureAtlas = _atlas;
            }

            //Game.viewportAdapter.Reset();

            return map.Layers.SelectMany(layer => layer.Tiles)
                .Where(poly => poly.Gid > 0)
                .Select(poly =>
                {
                    var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.Gid - 1);
                    var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                    var position = poly.Position;// new Vector2(poly.Position.X* size.X, poly.Position.Y* size.Y);

                    return new PolyTile()
                    {
                        Sprite = _sprite,
                        Position = position,
                    };
                }).ToList();
        }

        private class PolyTile
        {
            public Sprite Sprite { get; set; }

            public Vector2 Position { get; set; }
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw(samplerState: SamplerState.LinearWrap);
            DrawSprites(sb, interfaceSprites);
            sb.End();

            Game.MapSystem.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void DrawInternal(GameTime gameTime)
        {
            Game.WorldMap.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.Penumbra.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        private void DrawSprites(Monogame.SpriteBatch.SpriteBatchKnowed sb, List<PolyTile> tiles)
        {
            var offset = Vector2.Transform(new Vector2(0, 8), Game.CameraMain.GetViewMatrix());
            foreach (var sprite in tiles)
            {
                sprite.Sprite.Draw(sb, sprite.Position /*- offset*/, 0, Vector2.One);
            }
        }

        public override void UnloadContent()
        {
            Game.RemoveDesktopWidgets(true);
        }

        public override void Update(GameTime gameTime)
        {
            GameController.GlobalMenuWidget();
            Game.PlayerControlSystem.Update(gameTime);
            Game.MapSystem.Update(gameTime);
        }
    }
}