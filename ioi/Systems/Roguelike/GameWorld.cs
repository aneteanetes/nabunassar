using MoonSharp.Interpreter;
using System.Collections;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class GameWorld
    {
        public GameHost Game { get; }

        public GameWorld(GameHost game)
        {
            Game = game;
        }

        public ObjectMapSystem MapSystem { get; set; }

        public PlayerControlSystem PlayerControlSystem { get; set; }

        public PathfindSystem PathfindSystem { get; set; }

        public SpawnSystem SpawnSystem { get; set; }

        public LogSystem LogSystem { get; set; }

        public CombatSystem CombatSystem { get; set; }

        public LoadingSystem LoadingSystem { get; set; }

        public ItemRandomSystem ItemRandomSystem { get; set; }

        public BorderSystem BorderSystem { get; internal set; }

        internal IEnumerator LoadContent()
        {
            LoadingSystem.LoadContent();
            yield return 0;

            CombatSystem.LoadContent();
            yield return 0;

            yield return BorderSystem.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (!Game.IsGameActive)
                return;

            PlayerControlSystem?.Update(gameTime);
            MapSystem?.Update(gameTime);
            CombatSystem?.Update(gameTime);
            LoadingSystem?.Update(gameTime);
            LogSystem?.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            MapSystem?.Draw(gameTime);
            PlayerControlSystem?.Draw(gameTime);
            BorderSystem?.Draw(gameTime);
        }

        public void Dispose()
        {
            MapSystem?.Dispose();
            PlayerControlSystem?.Dispose();
            PathfindSystem?.Dispose();
            LogSystem?.Dispose();
            LoadingSystem?.Dispose();
            CombatSystem?.Dispose();

            MapSystem = null;
            PlayerControlSystem = null;
            PathfindSystem = null;
            SpawnSystem = null;
            LogSystem = null;
        }
    }
}