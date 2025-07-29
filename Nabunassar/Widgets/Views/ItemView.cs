using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Content;

namespace Nabunassar.Widgets.Views
{
    internal class ItemView
    {
        public ItemView(Item item, NabunassarContentManager content, double? merchantPercent=null)
        {
            Item = item;

            Cost = item.Cost * (merchantPercent.HasValue ? merchantPercent.Value : 1);

            var iconRegion = item.IconRegion;
            var iconTexture = content.Load<Texture2D>(item.Icon);

            Icon = new TextureRegion(iconTexture, new Rectangle(iconRegion.X, iconRegion.Y, iconRegion.Width, iconRegion.Height));
            Portrait = new TextureRegion(content.Load<Texture2D>(item.Portrait));
            Image = new TextureRegion(content.Load<Texture2D>(item.Image));
        }

        public Item Item { get; private set; }

        public IImage Icon { get; private set; }

        public IImage Portrait { get; private set; }

        public IImage Image { get; private set; }

        public Money Cost { get; set; }
    }
}
