using System.Collections;

namespace ioi.Systems.Roguelike
{
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

        public BorderLayersSystem BorderLayersSystem { get; internal set; }

        public void Update(GameTime gameTime)
        {
            if (!Game.IsGameActive)
                return;

            MapSystem?.Update(gameTime);
            PlayerControlSystem?.Update(gameTime);
            CombatSystem?.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            MapSystem?.Draw(gameTime);
            PlayerControlSystem?.Draw(gameTime);
            BorderLayersSystem?.Draw(gameTime);
        }

        public void Dispose()
        {
            MapSystem?.Dispose();
            PlayerControlSystem?.Dispose();
            PathfindSystem?.Dispose();
            LogSystem?.Dispose();
            CombatSystem?.Dispose();

            MapSystem = null;
            PlayerControlSystem = null;
            PathfindSystem = null;
            SpawnSystem = null;
            LogSystem = null;
        }

        internal IEnumerator LoadContent()
        {
            yield return BorderLayersSystem.LoadContent();
        }
    }
}