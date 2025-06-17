namespace Nabunassar.Components.Abstract
{
    internal abstract class BaseComponent
    {
        protected NabunassarGame Game;

        public BaseComponent(NabunassarGame game)
        {
            Game = game;
        }
    }
}
