using Microsoft.Xna.Framework;
using Nabunassar.Entities;
using Nabunassar.Entities.Data;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public void InitializeGameState()
        {
            GameState = new GameState();
            EntityFactory.CreateCursor();
        }

        public void RunGameState()
        {
            GameState.Party = new Party();
            var character = new Entities.Data.Character(this)
            {
                Tileset = "player.png"
            };
            GameState.Party.Characters.Add(character);

            EntityFactory.CreateCharacter(character, new Vector2(175, 230));
        }
    }
}
