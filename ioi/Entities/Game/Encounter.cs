using MonoGame.Extended.ECS;
using ioi.Entities.Struct.FixedCollections.Octas;
using ioi.Resources;

namespace ioi.Entities.Game
{
    internal class Encounter : IClonable<Encounter>
    {
        public Encounter Clone(Encounter instance = null)
        {
            var obj = instance ?? new Encounter();

            obj.EncounterId= EncounterId;
            obj.Targets = Targets;

            return obj;
        }

        public Guid EncounterId { get; set; }

        public List<Guid> Targets { get; set; }

        public Octa<Creature> Creatures { get; set; } = new();

        public Entity Entity { get; set; }

        internal void Init(DataBase dataBase)
        {
            var creatures = dataBase.Get<List<Creature>>("Data/Battlers/CreatureRegistry.json").Where(x => Targets.Contains(x.ObjectId));

            for (int i = 0; i < Targets.Count; i++)
            {
                var target = Targets[i];
                if (target == default)
                    continue;

                var creature = creatures.FirstOrDefault(x => x.ObjectId == target);
                Creatures[i] = creature.Clone();
            }
        }
    }
}
