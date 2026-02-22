using ioi.Screens.Abstract;
using ioi.Tiled.Map;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace ioi.Screens
{
    internal class RoguelikeScreen : BaseGameScreen
    {
        public RoguelikeScreen(GameHost game) : base(game) { }

        private List<PolyTile> interfaceSprites;
        private Sprite positionSprite;
        private Sprite crossSprite;

        public Effect Celshading { get; private set; }

        public Effect CelshadingGlobal { get; private set; }

        public override void LoadContent()
        {
            Celshading = Game.Content.Load<Effect>("Assets/Shaders/Celshading.fx");
            CelshadingGlobal = Game.Content.Load<Effect>("Assets/Shaders/CelshadingGlobal.fx");
            var @interface = Game.Content.Load<TiledMap>("Assets/Maps/interface2.tmx");
            interfaceSprites = LoadTiled(@interface);

            var cursorTileset = Game.Content.LoadTexture("Assets/Tilesets/cursor_tilemap_packed.png");
            positionSprite = new Sprite(new Texture2DRegion(cursorTileset, 272, 32, 16, 16)) { Color= Color.AntiqueWhite };
            crossSprite = new Sprite(new Texture2DRegion(cursorTileset, 272, 0, 16, 16)) { Color = Color.Red, };
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

            var gray = Color.Wheat;

            return map.Layers.SelectMany(layer => layer.Tiles)
                .Where(poly => poly.Gid > 0)
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
        }

        private class PolyTile
        {
            public Sprite Sprite { get; set; }

            public Vector2 Position { get; set; }
        }

        public override void Update(GameTime gameTime)
        {
            GameController.GlobalMenuWidget();
            Game.PlayerControlSystem.Update(gameTime);
            Game.MapSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            CelshadingGlobal.Parameters["ScreenSize"].SetValue(new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height));

            var sb = Game.BeginDraw(effect: CelshadingGlobal);

            DrawSprites(sb, interfaceSprites);

            sb.End();

            Game.MapSystem.Draw(gameTime, Celshading);
            Game.PlayerControlSystem.Draw(gameTime);

            sb = Game.BeginDraw(effect: Celshading,camera:Game.CameraMap);

            if (Game.GameState.Temp.ClickPosition.HasValue)
            {
                var sprite = Game.GameState.Temp.ClickSprite == Entities.Data.Temporary.ClickSprite.Position
                    ? positionSprite
                    : crossSprite;

                var drawPos = Game.GameState.Temp.ClickPosition.Value;// - new Vector2(16, 16);

                sb.Draw(sprite, drawPos, 0, new Vector2(2, 2));
            }

            var hp = Game.GameState.Player["stats.hp"];
            var mhp = Game.GameState.Player["stats.mhp"];
            sb.DrawText(Fonts.Consolas, 25, $"Здоровье: {hp}/{mhp}", new Vector2(1600, 200), Color.Red);

            sb.End();

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
    }
}