using Nabunassar.Components.GameMap;
using Nabunassar.Entities;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public void InitializeGameState()
        {
            GameState = new GameState();
            GameState.Party = new Party();
            GameState.Party.Characters.Add(new Character()
            {
                Tileset = "player.png"
            });

            World.Add(new PlayerComponent(this, GameState.Party)
            {
                Position=new Microsoft.Xna.Framework.Vector2(700,930)
            });
        }
    }
}
