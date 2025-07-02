using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class SpeakToRadialAction : RadialMenuAction
    {
        public SpeakToRadialAction(RadialMenu menu) : base(menu, Direction.Right, "speak")
        {
        }

        public override void OnClick()
        {
            Menu.Close();
            var speakerWorldPosition = Game.Camera.ScreenToWorld(Menu.Position);
            Game.GameState.Party.SpeakTo(Menu.GameObject, speakerWorldPosition);
        }
    }
}
