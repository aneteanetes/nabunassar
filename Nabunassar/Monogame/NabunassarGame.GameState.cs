using Nabunassar.Entities;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;

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

#if DEBUG
            var revealModel = DataBase.GetAbility("Reveal");
            party.Third.Creature.WorldAbilities.First = new RevealAbility(Game, party.Third.Creature, revealModel);
#endif
        }

        public void ChangeGameActive()
        {
            IsGameActive = !IsGameActive;
            if (!IsGameActive)
                GameState.Cursor.SetCursor("cursor");
        }
    }
}
