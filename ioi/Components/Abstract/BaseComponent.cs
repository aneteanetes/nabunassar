namespace ioi.Components.Abstract
{
    internal abstract class BaseComponent
    {
        protected GameHost Game;

        public BaseComponent(GameHost game)
        {
            Game = game;
        }
    }
}
