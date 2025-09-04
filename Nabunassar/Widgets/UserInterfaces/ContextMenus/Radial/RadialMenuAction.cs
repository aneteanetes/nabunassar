using Geranium.Reflection;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial
{
    internal abstract class RadialMenuAction
    {
        public Direction Position { get; set; }

        public string CodeName { get; set; }

        public string Name { get; set; }

        public TextureRegion Icon { get; set; }

        public RadialMenu Menu { get; protected set; }

        public bool IsEnabled { get; set; } = true;

        public NabunassarGame Game => Menu.Game;

        public GameObject GameObject => Menu.GameObject;

        public IEnumerable<RadialMenuAction> InnerActions { get; set; }

        public RadialMenuAction(RadialMenu menu, Direction position, string codeName, IEnumerable<RadialMenuAction> innerActions = null)
        {
            this.Menu = menu;

            Position = position;
            CodeName = codeName;
            InnerActions = innerActions;
            Menu = menu;
            IsEnabled = menu.Game.GameState.Party.IsObjectNear(menu.GameObject);
        }

        public BackRadialAction CreateBackAction()
        {
            return new BackRadialAction(Menu, this);
        }

        public void Close()
        {
            Menu.Close();
            Game.RemoveDesktopWidgets<TitleWidget>();
        }

        public virtual void OnClick()
        {
            Game.RemoveDesktopWidgets<TitleWidget>();
            if (InnerActions.IsNotEmpty())
                Menu.Fullfill(InnerActions, this);
            //Close();
            //Open(Game, GameObject, Position);
            //Mouse.SetPosition((int)Position.X, (int)Position.Y);
        }
    }
}