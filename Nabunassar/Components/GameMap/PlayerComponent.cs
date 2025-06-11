using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Entities;
using Nabunassar.Entities.Interfaces;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Tiled.Renderer;

namespace Nabunassar.Components.GameMap
{
    internal class PlayerComponent : IRenderable, IPositioned, ILoadable, IMoveable
    {
        Party _party;
        Dictionary<Character, AnimatedSprite> _sprites = new();
        List<Texture> textures = new();
        NabunassarGame _game;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public Vector2 Position { get; set; }

        public bool Enabled { get; set; } = true;

        public int UpdateOrder {  get; set; }

        public float Speed { get; set; } = 0.04f;

        public PlayerComponent(NabunassarGame game, Party party)
        {
            _party = party;
            _game = game;
            Position = new(0, 0);
        }

        public void LoadContent()
        {
            foreach (var @char in _party.Characters)
            {
                var name = "SpriteSheet_" + @char.Name;
                var texture = _game.Content.Load<Texture2D>("Assets/Tilesets/" + @char.Tileset);
                textures.Add(texture);
                var atlas = Texture2DAtlas.Create(name+Guid.NewGuid().ToString(), texture, 16, 24);
                SpriteSheet spriteSheet = new SpriteSheet("SpriteSheet_" + @char.Name, atlas);

                spriteSheet.DefineAnimation("idle", builder =>
                {
                    builder.IsLooping(true)
                           .AddFrame(regionIndex: 4, duration: TimeSpan.FromSeconds(0.2))
                           .AddFrame(5, TimeSpan.FromSeconds(0.2))
                           .AddFrame(6, TimeSpan.FromSeconds(0.2))
                           .AddFrame(7, TimeSpan.FromSeconds(0.2));
                });


                spriteSheet.DefineAnimation("run", builder =>
                {
                    builder.IsLooping(true)
                           .AddFrame(regionIndex: 8, duration: TimeSpan.FromSeconds(0.1))
                           .AddFrame(9, TimeSpan.FromSeconds(0.1))
                           .AddFrame(10, TimeSpan.FromSeconds(0.1))
                           .AddFrame(11, TimeSpan.FromSeconds(0.1));
                });


                var _sprite = new AnimatedSprite(spriteSheet, "idle");

                _sprites[@char] = _sprite;
            }
        }

        public void UnloadContent() 
            => Dispose();

        private bool animationPlayed=true;

        public void Update(GameTime gameTime)
        {
            var state = KeyboardExtended.GetState();

            float x = 0, y = 0;

            if (animationPlayed)
            {
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    y += Speed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                {
                    y -= Speed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                {
                    x -= Speed;
                }
                if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    x += Speed;
                }
            }

            var newPos = new Vector2(Position.X + x, Position.Y + y);

            foreach (var _spriteKV in _sprites)
            {
                var _sprite = _spriteKV.Value;
                if (newPos != Position)
                {
                    if (_sprite.CurrentAnimation != "run")
                    {
                        _sprite.SetAnimation("run");
                    }
                    Position = newPos;
                }
                else
                {
                    _sprite.SetAnimation("idle");
                }

                _sprite.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
        {
            Vector2 offset = Vector2.Zero;

            foreach (var _sprite in _sprites)
            {
                spriteBatch.Draw(_sprite.Value, Position, 0, Vector2.One * TiledMapRenderer.TileSizeMultiplier);
            }

            spriteBatch.End();
        }

        public void Dispose()
        {
            _sprites.Clear();
            textures.ForEach(x => x.Dispose());
        }
    }
}