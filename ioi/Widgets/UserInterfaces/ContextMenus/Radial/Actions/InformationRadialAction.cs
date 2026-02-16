using ioi.Entities.Game;
using ioi.Struct;
using ioi.Widgets.UserInterfaces.GameWindows.Informations;

namespace ioi.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class InformationRadialAction : RadialMenuAction
    {
        public InformationRadialAction(RadialMenu menu) : base(menu, Direction.Up,"info")
        {
            IsEnabled = true;
        }

        public override void OnClick()
        {
            Menu.Close();
            var party = Game.GameState.Party;
            if (party.IsObjectNear(this.Menu.GameObject))
            {
                InformationWindow.Open(Game, this.Menu.GameObject);
            }
            else
            {
                var worldPos = Game.CameraMain.ScreenToWorld(Menu.Position);
                party.MoveTo(worldPos, Menu.GameObject, worldPos);
            }
        }
    }
}
