using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike.Controllings
{
    public class KeyTileset
    {
        public Keys Key { get; set; }

        public MouseButton Mouse { get; set; }

        public CustomKeys Custom { get; set; }

        public string Tileset { get; set; }

        public int TileId { get; set; }

        internal Texture2DRegion Region(GameHost game)
        {
            return game.GameState.Tilesets[Tileset].GetRegion(TileId);
        }
    }
}
