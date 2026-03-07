using Assimp;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike.Controllings
{
    public class KeyToTilesetMap
    {
        private Dictionary<Keys, KeyTileset> keys = new();

        private Dictionary<MouseButton, KeyTileset> mbtns = new();

        private Dictionary<CustomKeys, KeyTileset> custom = new();

        public KeyTileset GetKeyTileset(Keys key) => keys[key];

        public KeyTileset GetKeyTileset(MouseButton mbtn) => mbtns[mbtn];

        public KeyTileset GetKeyTileset(CustomKeys ckey) => custom[ckey];

        private void Add(Keys key, string tileset, int tileid)
        {
            keys[key] = new KeyTileset()
            {
                TileId = tileid,
                Tileset = tileset,
                Key = key
            };
        }

        private void Add(MouseButton key, string tileset, int tileid)
        {
            mbtns[key] = new KeyTileset()
            {
                TileId = tileid,
                Tileset = tileset,
                Mouse = key
            };
        }

        private void Add(CustomKeys key, string tileset, int tileid)
        {
            custom[key] = new KeyTileset()
            {
                TileId = tileid,
                Tileset = tileset,
                Custom = key
            };
        }

        public static KeyToTilesetMap KeyboardMap()
        {
            KeyToTilesetMap map = new();

            var ktile = "keyboard_mouse";

            map.Add(Keys.A,ktile, 229);
            map.Add(Keys.D1, ktile, 243);
            map.Add(Keys.D2, ktile, 245);
            map.Add(Keys.D3, ktile, 247);
            map.Add(Keys.D4, ktile, 249);
            map.Add(Keys.D5, ktile, 251);
            map.Add(Keys.D6, ktile, 253);
            map.Add(Keys.D7, ktile, 255);
            map.Add(Keys.D8, ktile, 225);
            map.Add(Keys.Q, ktile, 95);

            map.Add(MouseButton.Left, ktile, 19);
            map.Add(MouseButton.Right, ktile, 23);

            map.Add(Keys.W, ktile, 39);
            map.Add(Keys.A, ktile, 229);
            map.Add(Keys.S, ktile, 73);
            map.Add(Keys.D, ktile, 166);

            map.Add(Keys.F1, ktile, 152);
            map.Add(Keys.F2, ktile, 128);
            map.Add(Keys.F3, ktile, 130);
            map.Add(Keys.F4, ktile, 132);
            map.Add(Keys.F5, ktile, 134);
            map.Add(Keys.F6, ktile, 136);

            map.Add(Keys.E, ktile, 170);
            map.Add(Keys.C, ktile, 182);
            map.Add(Keys.I, ktile, 120);
            map.Add(Keys.M, ktile, 98);
            map.Add(Keys.R, ktile, 69);

            map.Add(CustomKeys.Arrows, ktile, 213);

            map.Add(Keys.F, ktile, 150);
            map.Add(Keys.D, ktile, 166);

            map.Add(Keys.Escape, ktile, 146);

            map.Add(Keys.Up, ktile, 193);
            map.Add(Keys.Down, ktile, 216);
            map.Add(Keys.Left, ktile, 220);
            map.Add(Keys.Right, ktile, 223);

            return map;
        }

    }
}
