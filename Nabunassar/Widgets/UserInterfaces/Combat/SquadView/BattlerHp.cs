using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Widgets.UserInterfaces.Combat.SquadView
{
    internal class BattlerHp : Panel, IFeatured
    {
        private Label _text;
        private Creature _creature;

        public BattlerHp(NabunassarGame game, Creature creature)
        {
            _creature = creature;

            this.Width = 130;
            this.Height = 20;

            if (creature != null)
            {
                _text = new Label()
                {
                    Text = GetHp(),
                    Font = game.Content.LoadFont(Fonts.Retron).GetFont(18),
                    HorizontalAlignment= HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                this.Widgets.Add(_text);
            }
        }

        private string GetHp() => $"{_creature.HPNow} / {_creature.HPMax}";

        public void Update(GameTime gameTime)
        {
            if (_creature == null)
                return;

            _text.Text = GetHp();
        }
    }
}