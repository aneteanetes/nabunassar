using Assimp;
using ioi.Monogame.Settings;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike.Controllings
{
    public class ControlScheme
    {
        public ControlSchema Schema { get; internal set; }

        public KeyToTilesetMap TilesetMap { get; set; }

        public static ControlScheme Create(ControlSchema schema) => schema switch
        {
            ControlSchema.Keyboard => DefaultKeyboard(),
            ControlSchema.GamepadPS => DefaultGamepadPS(),
            ControlSchema.GamepadX => DefaultGamepadX(),
            ControlSchema.GamepadN => DefaultGamepadN(),
            ControlSchema.GamepadOther => DefaultGamepadOther(),
            _ => DefaultKeyboard(),
        };

        private ControlSchemeKey Bind(CustomKeys custom, params Keys[] keys) => new ControlSchemeKey
        {
            Scheme = this,
            Keys = [.. keys],
            Tileset = TilesetMap.GetKeyTileset(custom)
        };

        private ControlSchemeKey Bind(Keys key) => new ControlSchemeKey
        {
            Scheme = this,
            Keys = [ key ],
            Tileset = TilesetMap.GetKeyTileset(key)
        };

        private ControlSchemeKey Bind(MouseButton key) => new ControlSchemeKey
        {
            Scheme = this,
            MKey = [key],
            Tileset = TilesetMap.GetKeyTileset(key)
        };

        private static ControlScheme DefaultKeyboard()
        {
            var scheme = new ControlScheme() {
                Schema = ControlSchema.Keyboard,
                TilesetMap = KeyToTilesetMap.KeyboardMap()
            };

            scheme.Ability1 = scheme.Bind(Keys.D1);
            scheme.Ability2 = scheme.Bind(Keys.D2);
            scheme.Ability3 = scheme.Bind(Keys.D3);
            scheme.Ability4 = scheme.Bind(Keys.D4);
            scheme.Attack = scheme.Bind(Keys.A);
            scheme.Info = scheme.Bind(Keys.Q);
            scheme.MouseRightButton = scheme.Bind(MouseButton.Right);
            scheme.MouseLeftButton = scheme.Bind(MouseButton.Left);
            scheme.MoveUp = scheme.Bind(Keys.W);
            scheme.MoveDown = scheme.Bind(Keys.S);
            scheme.MoveLeft = scheme.Bind(Keys.A);
            scheme.MoveRight = scheme.Bind(Keys.D);
            scheme.Party1 = scheme.Bind(Keys.F1);
            scheme.Party2 = scheme.Bind(Keys.F2);
            scheme.Party3 = scheme.Bind(Keys.F3);
            scheme.Party4 = scheme.Bind(Keys.F4);
            scheme.Party5 = scheme.Bind(Keys.F5);
            scheme.Party6 = scheme.Bind(Keys.F6);
            scheme.Use = scheme.Bind(Keys.E);
            scheme.Skill1 = scheme.Bind(Keys.D5);
            scheme.Skill2 = scheme.Bind(Keys.D6);
            scheme.Skill3 = scheme.Bind(Keys.D7);
            scheme.Skill4 = scheme.Bind(Keys.D8);
            scheme.CharInfo = scheme.Bind(Keys.C);
            scheme.Inventory = scheme.Bind(Keys.I);
            scheme.Map = scheme.Bind(Keys.M);
            scheme.Camera = scheme.Bind(CustomKeys.Arrows, Keys.Left, Keys.Up, Keys.Down, Keys.Right);
            scheme.Ability1Combat = scheme.Bind(Keys.Q);
            scheme.Ability2Combat = scheme.Bind(Keys.W);
            scheme.Ability3Combat = scheme.Bind(Keys.E);
            scheme.Ability4Combat = scheme.Bind(Keys.R);
            scheme.Flee = scheme.Bind(Keys.S);
            scheme.Defence = scheme.Bind(Keys.D);
            scheme.Waiting = scheme.Bind(Keys.F);
            scheme.Back = scheme.Bind(Keys.Escape);
            scheme.CameraUp = scheme.Bind(Keys.Up);
            scheme.CameraDown = scheme.Bind(Keys.Down);
            scheme.CameraLeft = scheme.Bind(Keys.Left);
            scheme.CameraRight = scheme.Bind(Keys.Right);

            return scheme;
        }

        private static ControlScheme DefaultGamepadPS() => new ControlScheme()
        {
            Schema = ControlSchema.GamepadPS
        };

        private static ControlScheme DefaultGamepadX() => new ControlScheme()
        {
            Schema = ControlSchema.GamepadX
        };

        private static ControlScheme DefaultGamepadN() => new ControlScheme()
        {
            Schema = ControlSchema.GamepadN
        };

        private static ControlScheme DefaultGamepadOther() => new ControlScheme()
        {
            Schema = ControlSchema.GamepadOther
        };

        public ControlSchemeKey Attack { get; set; }

        public ControlSchemeKey Defence { get; set; }        

        public ControlSchemeKey Info { get; set; }
        
        public ControlSchemeKey Back { get; set; }

        public ControlSchemeKey MouseRightButton { get; set; }

        public ControlSchemeKey MouseLeftButton { get; set; }

        public ControlSchemeKey MoveUp { get; set; }

        public ControlSchemeKey MoveDown { get; set; }

        public ControlSchemeKey MoveLeft { get; set; }

        public ControlSchemeKey MoveRight { get; set; }

        public ControlSchemeKey CameraUp { get; set; }

        public ControlSchemeKey CameraDown { get; set; }

        public ControlSchemeKey CameraLeft { get; set; }

        public ControlSchemeKey CameraRight { get; set; }

        public ControlSchemeKey Ability1 { get; set; }

        public ControlSchemeKey Ability2 { get; set; }

        public ControlSchemeKey Ability3 { get; set; }

        public ControlSchemeKey Ability4 { get; set; }

        public ControlSchemeKey Ability1Combat { get; set; }

        public ControlSchemeKey Ability2Combat { get; set; }

        public ControlSchemeKey Ability3Combat { get; set; }

        public ControlSchemeKey Ability4Combat { get; set; }

        public ControlSchemeKey Party1 { get; set; }

        public ControlSchemeKey Party2 { get; set; }

        public ControlSchemeKey Party3 { get; set; }

        public ControlSchemeKey Party4 { get; set; }

        public ControlSchemeKey Party5 { get; set; }

        public ControlSchemeKey Party6 { get; set; }

        public ControlSchemeKey Use { get; set; }

        public ControlSchemeKey Skill1 { get; set; }

        public ControlSchemeKey Skill2 { get; set; }

        public ControlSchemeKey Skill3 { get; set; }

        public ControlSchemeKey Skill4 { get; set; }

        public ControlSchemeKey CharInfo { get; set; }

        public ControlSchemeKey Inventory { get; set; }

        public ControlSchemeKey Waiting { get; set; }
        
        public ControlSchemeKey Map { get; set; }

        public ControlSchemeKey Camera { get; set; }

        public ControlSchemeKey Flee { get; set; }

    }
}
