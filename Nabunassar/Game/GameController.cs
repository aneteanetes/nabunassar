using Nabunassar.Screens.Game;

namespace Nabunassar
{
    internal static class GameController
    {
        public static void StartNewGame(NabunassarGame game)
        {
            game.SwitchScreen(new MainGameScreen(game, true), LoadNewGame(game));
        }

        private static Func<Task> LoadNewGame(NabunassarGame game)
        {
            return () =>
            {
                return Task.CompletedTask;
            };
        }
    }
}
