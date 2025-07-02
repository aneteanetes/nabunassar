namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class BackRadialAction : RadialMenuAction
    {
        public RadialMenuAction PreviousAction { get; private set; }

        public BackRadialAction(RadialMenu menu, RadialMenuAction baseAction) : base(menu, baseAction.Position, baseAction.CodeName, baseAction.InnerActions)
        {
            PreviousAction = baseAction;
        }

        public override void OnClick()
        {
            Menu.Fullfill(null, null, Menu.GameObject);
            base.OnClick();
        }
    }
}
