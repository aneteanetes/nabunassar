using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Dices;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class DiceIcon : Image
    {
        private Texture2D _texture;

        public DiceIcon(NabunassarGame game, Dice dice, bool isFilled = true)
        {
            _texture = game.Content.Load<Texture2D>("Assets/Tilesets/boardIconsDefault.png");
            Renderable = GetRegion(dice);
            Color = Globals.BaseColor;
        }

        private TextureRegion GetRegion(Dice dice)
        {
            Point location = dice.Edges switch
            {
                2 => new Point(128, 0),
                4 => new Point(128, 128),
                6 => new Point(128, 192),
                8 => new Point(0, 0),
                10 => new Point(0, 64),
                12 => new Point(0, 128),
                20 => new Point(0, 192),
                _ => new Point(704, 64),
            };
            return new TextureRegion(_texture, new Rectangle(location, new Point(64)));
        }

        private int _size;
        public int Size
        {
            get => _size;
            set
            {
                _size = value;
                Width = _size;
                Height = _size;
            }
        }
    }
}