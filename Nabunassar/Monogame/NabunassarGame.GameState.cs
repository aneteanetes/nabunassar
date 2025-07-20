using Nabunassar.Entities;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Game;

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
            var party = GameState.Party = new Party(this);

            party.First = new Hero(this)
            {
                Tileset = "warrior.png",
                Creature = new Creature()
            };

            party.Second = new Hero(this) { Tileset = "rogue.png" };
            party.Third = new Hero(this) { Tileset = "wizard.png" };
            party.Fourth = new Hero(this) { Tileset = "priest.png" };

            var pos = new Vector2(175, 230);

            EntityFactory.CreateParty(GameState.Party, pos);

            party.First.Creature.WorldAbilities.First = new LandscapeAbility(this, party, party.First.Creature);
        }

        public void ChangeGameActive()
        {
            IsGameActive = !IsGameActive;
            if (!IsGameActive)
                GameState.Cursor.SetCursor("cursor");
        }
    }
}
