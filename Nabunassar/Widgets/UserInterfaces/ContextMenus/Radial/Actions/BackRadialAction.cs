namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class BackRadialAction : RadialMenuAction
    {
        public RadialMenuAction PreviousAction { get; private set; }

        public BackRadialAction(RadialMenu menu, RadialMenuAction baseAction) : base(menu, baseAction.Position, baseAction.CodeName, baseAction.InnerActions)
        {
            PreviousAction = baseAction;
            IsEnabled = true;
        }

        public override void OnClick()
        {
            Game.RemoveDesktopWidgets<TitleWidget>();
            Menu.Fullfill(null, null, Menu.GameObject);
        }
    }
}
