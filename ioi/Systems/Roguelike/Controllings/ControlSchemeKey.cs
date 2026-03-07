using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike.Controllings
{
    public class ControlSchemeKey
    {
        public ControlScheme Scheme { get; set; }

        public KeyTileset Tileset { get; set; }

        public List<Keys> Keys { get; set; } = new();

        public List<MouseButton> MKey { get; set; } = new();

        internal Texture2DRegion GetRegion(GameHost game) => Tileset.Region(game);

        public bool WasPressed()
        {
            var stateKey = KeyboardExtended.GetState();
            foreach (var key in Keys)
            {
                if (!stateKey.WasKeyPressed(key))
                    return false;
            }

            var stateMouse = MouseExtended.GetState();
            foreach (var mkey in MKey)
            {
                if (!stateMouse.WasButtonPressed(mkey))
                    return false;
            }

            return true;
        }

        public bool IsDown()
        {
            var stateKey = KeyboardExtended.GetState();
            foreach (var key in Keys)
            {
                if (!stateKey.IsKeyDown(key))
                    return false;
            }

            var stateMouse = MouseExtended.GetState();
            foreach (var mkey in MKey)
            {
                if (!stateMouse.WasButtonPressed(mkey))
                    return false;
            }

            return true;
        }
    }
}
