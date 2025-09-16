using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct.ImageRegions;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal abstract class StatLine : HorizontalStackPanel, IFeatured
    {
        protected HorizontalProgressBar bar;
        protected NabunassarGame Game;
        protected Creature Creature;

        public StatLine(NabunassarGame game, Creature creature, int length=75)
        {
            Game = game;
            Creature = creature;

            bar = new HorizontalProgressBar();
            bar.Filler = new SolidBrush(Color);
            bar.Maximum = GetMax();
            bar.Minimum = 0;
            bar.Value = bar.Maximum;
            bar.Width = length;
            bar.Height = 8;
            bar.VerticalAlignment= VerticalAlignment.Center;

            var texture = game.Content.LoadTexture(TextureAsset);

            var icon = new Image()
            {
                Renderable = new TextureRegion(texture, TextureRegion),
                Color = Color,
                Width = 16,
                Height = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(3, 0, 0, 0)
            };

            Widgets.Add(icon);
            Widgets.Add(bar);
        }

        public abstract string TextureAsset { get; }

        public abstract Rectangle TextureRegion { get; }

        public abstract Color Color { get; }

        public abstract int GetMax();

        public abstract int GetCurrent();

        public abstract string GetTooltip();

        public void Update(GameTime gameTime)
        {
            var now = GetCurrent();
            var max = GetMax();

            bar.Maximum = max;
            bar.Value = now;

            this.Tooltip = $"{GetTooltip()}: {now}/{max}";
        }
    }
}