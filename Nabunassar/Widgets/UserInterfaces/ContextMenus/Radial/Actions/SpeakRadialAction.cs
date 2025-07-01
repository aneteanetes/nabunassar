using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class SpeakRadialAction : RadialMenuAction
    {
        public SpeakRadialAction(RadialMenu menu) : base(menu, Direction.Right, "speak")
        {
        }

        public override void OnClick()
        {
            Menu.Close();
            Game.AddDesktopWidget(new DialogueMenu(Game, GameObject));
        }
    }
}
