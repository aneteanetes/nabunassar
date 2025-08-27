using Myra.Graphics2D.TextureAtlases;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class IconButton
    {
        public string Title { get; protected set; }

        public TextureRegion Icon { get; protected set; }

        public IconButton(string title, TextureRegion icon)
        {
            Title = title;
            Icon = icon;
        }

        public virtual bool IsReactOnClick => true;

        public virtual void OnClick() { }
    }
}
