using ioi.Components;
using ioi.Widgets.UserInterfaces.Roguelike;
using MonoGame.Extended.Input;

namespace ioi.Systems.Roguelike
{
    internal class CombatSystem : IDisposable
    {
        public GameHost Game { get; }

        EntityWidget enemyWidget;

        public CombatSystem(GameHost game)
        {
            Game = game;
        }

        public void Update(GameTime gameTime)
        {
            var key = KeyboardExtended.GetState();
            if(key.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                EndCombat();
            }
        }

        public void StartCombat(GameEntity enemy)
        {
            Game.GameWorld.BorderLayersSystem["LeftPanel"] = true;
            Game.GameWorld.BorderLayersSystem["Center"] = true;
            Game.GameWorld.MapSystem.Pause();
            Game.GameWorld.PlayerControlSystem.Combat();
            enemyWidget = Game.AddDesktopWidget(new EntityWidget(Game, enemy, Struct.Side.Left), Game.MyraDesktopIngame); 
        }

        public void EndCombat()
        {
            Game.GameWorld.BorderLayersSystem["LeftPanel"] = false;
            Game.GameWorld.BorderLayersSystem["Center"] = false;
            Game.GameWorld.MapSystem.Resume();
            Game.GameWorld.PlayerControlSystem.Map();
            Game.RemoveDesktopWidget(enemyWidget,Game.MyraDesktopIngame);
        }

        public void Dispose()
        {
        }
    }
}
