using GoRogue.GameFramework;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets
{
    internal class WidgetFactory
    {
        NabunassarGame Game { get; }

        public WidgetFactory(NabunassarGame game)
        {
            Game = game;
        }
    }
}
