using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal class HPLine : HorizontalStackPanel, IFeatured
    {
        private HorizontalProgressBar _bar;
        private Creature _creature;

        public HPLine(NabunassarGame game, Creature creature)
        {
            _creature = creature;

            _bar = new HorizontalProgressBar();
            _bar.Filler = new SolidBrush(Color.Red);
            _bar.Maximum = _creature.HPMax;
            _bar.Minimum = 0;
            _bar.Value = _bar.Maximum;
            _bar.Width = 75;
            _bar.Height = 8;
            _bar.VerticalAlignment= VerticalAlignment.Center;

            var icons = game.Content.LoadTexture("Assets/Tilesets/transparent_packed.png");

            var icon = new Image()
            {
                Renderable = new TextureRegion(icons, new Rectangle(624, 160, 16, 16)),
                Color = Color.Red,
                Width = 24,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(3, 0, 0, 0)
            };

            Widgets.Add(icon);
            Widgets.Add(_bar);
        }

        public void Update(GameTime gameTime)
        {
            _bar.Maximum = _creature.HPMax;
            _bar.Value = _creature.HPNow;
        }
    }
}