using FontStashSharp;
using ioi.Components;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.InfoList
{
    internal class ObjectInfoRow : HorizontalStackPanel
    {
        public InfoWidget Info { get; }

        public GameHost Game { get; }

        public GameEntity Entity { get; }

        public DynamicSpriteFont Font { get; }

        public ObjectInfoRow(InfoWidget info, GameHost game, GameEntity entity, DynamicSpriteFont font)
        {
            Info = info;
            Game = game;
            Entity = entity;
            Font = font;

            this.MouseEntered += ObjectInfoRow_MouseEntered;
            this.MouseLeft += ObjectInfoRow_MouseLeft;
        }

        private void ObjectInfoRow_MouseLeft(object sender, MyraEventArgs e)
            => Unselect();

        private void ObjectInfoRow_MouseEntered(object sender, MyraEventArgs e)
            => Select();

        public void Select()
        {
            Background = new SolidBrush(new Color(25, 25, 25));
            Info.UnselectAll(this);
        }

        public void Unselect()
        {
            Background = new SolidBrush(Color.Black);
        }
    }
}