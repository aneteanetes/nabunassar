using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class MoveToRadialAction : RadialMenuAction
    {
        public MoveToRadialAction(RadialMenu menu) : base(menu, Direction.Right, "moveto")
        {
            IsEnabled = true;
        }

        public override void OnClick()
        {
            Game.GameState.Party.MoveTo(Game.Camera.ScreenToWorld(Menu.Position));
            base.Close();
        }
    }
}