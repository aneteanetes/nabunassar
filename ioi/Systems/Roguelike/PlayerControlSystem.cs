using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike
{
    internal class PlayerControlSystem
    {
        public GameHost Game { get; }
        private TimeSpan _total;

        public PlayerControlSystem(GameHost game)
        {
            Game = game;
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - _total < TimeSpan.FromMilliseconds(75))
                return;

            _total = gameTime.TotalGameTime;

            var keyboard = KeyboardExtended.GetState();

            var player = Game.GameState.Player;
            float x = player.Coords.X, y = player.Coords.Y;

            if (keyboard.IsKeyDown(Keys.S))
            {
                y += 1;
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                y -= 1;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                x -= 1;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                x += 1;
            }

            var coords = new Vector2(x, y);

            var objs = Game.GameState.Map.Collide(coords);

            if (objs.Count > 0)
            {
                player.ProcessCollision(objs);
                return;
            }

            Game.GameState.Map.Move(player,coords);
        }
    }
}
