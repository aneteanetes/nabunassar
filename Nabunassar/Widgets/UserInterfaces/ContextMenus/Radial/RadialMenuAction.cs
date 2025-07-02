using Microsoft.Xna.Framework.Input;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial
{
    internal abstract class RadialMenuAction
    {
        public Direction Position { get; set; }

        public string CodeName { get; set; }

        public RadialMenu Menu { get; protected set; }

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
        }

        public BackRadialAction CreateBackAction()
        {
            return new BackRadialAction(Menu, this);
        }

        public virtual void OnClick()
        {
            //Close();
            //Open(Game, GameObject, Position);
            //Mouse.SetPosition((int)Position.X, (int)Position.Y);
        }
    }
}