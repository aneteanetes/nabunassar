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
            var party = GameState.Party = DataBase.CreateRandomParty(this);

            var pos = new Vector2(175, 230);

            EntityFactory.CreateParty(GameState.Party, pos);

            //party.First.Creature.WorldAbilities.First = new LandscapeAbility(this, party, party.First.Creature);
        }

        public void ChangeGameActive()
        {
            IsGameActive = !IsGameActive;
            if (!IsGameActive)
                GameState.Cursor.SetCursor("cursor");
        }
    }
}
