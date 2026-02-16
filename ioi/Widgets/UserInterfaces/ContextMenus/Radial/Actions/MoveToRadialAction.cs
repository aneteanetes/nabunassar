using ioi.Struct;

namespace ioi.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class MoveToRadialAction : RadialMenuAction
    {
        public MoveToRadialAction(RadialMenu menu) : base(menu, Direction.Right, "moveto")
        {
            IsEnabled = true;
        }

        public override void OnClick()
        {
            Game.GameState.Party.MoveTo(Game.CameraMain.ScreenToWorld(Menu.Position));
            base.Close();
        }
    }
}