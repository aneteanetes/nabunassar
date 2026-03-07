using ioi.Entities.Data;
using ioi.Entities.Data.Abilities.WorldAbilities;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace ioi
{
    internal partial class GameHost
    {
        public void InitializeGameState()
        {
            if (GameState != null)
                GameState.Dispose();

            GameState = new GameState();
            GameState.Init();
            EntityFactoryMap.CreateCursor();
        }

        public void RunGameState()
        {
            var party = GameState.Party = DataBase.CreateRandomParty(this);

            var pos = new Vector2(175, 230);

            EntityFactoryMap.CreateParty(GameState.Party, pos);

            GameState.IsActive = true;

#if DEBUG
            var landscapeModel = DataBase.GetAbility("Landscape");
            party.First.Creature.WorldAbilities.First = new LandscapeAbility(Game, GameState.Party, party.First.Creature, landscapeModel);

            var revealModel = DataBase.GetAbility("Reveal");
            party.Second.Creature.WorldAbilities.First = new RevealAbility(Game, party.Second.Creature, revealModel);

            var teleportModel = DataBase.GetAbility("Teleportation");
            party.Third.Creature.WorldAbilities.First = new TeleportationAbility(Game, party.Third.Creature, teleportModel);

            var prayerModel = DataBase.GetAbility("Prayer");
            party.Fourth.Creature.WorldAbilities.First = new PrayerAbility(Game, party.Fourth.Creature, prayerModel);
#endif
        }

        public void ChangeGameActive()
        {
            IsGameActive = !IsGameActive;
            if (!IsGameActive)
                GameState.Cursor.SetCursor("cursor");
        }

        public IEnumerable<ICollisionActor> QuerySpace(string layerName, IShapeF shape)
        {
            var layer = Game.CollisionComponent.Layers[layerName];
            var actors = layer.Space.Query(shape.BoundingRectangle);

            return actors.Where(x => shape.Intersects(x.Bounds));
        }
    }
}
