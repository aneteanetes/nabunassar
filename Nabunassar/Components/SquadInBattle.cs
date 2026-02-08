using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct.FixedCollections.Octas;
using Nabunassar.Struct;
using System.Diagnostics;

namespace Nabunassar.Components
{
    [DebuggerDisplay("{Name}")]
    internal class SquadInBattle
    {
        public string Name { get; }

        public bool IsPlayerSquad { get; set; }

        public Octa<Creature> Squad { get; private set; }

        public Side Side { get; private set; }

        public SquadInBattle(string name, Octa<Creature> squad, Side side)
        {
            Name = name;
            Squad = squad;
            Side = side;
        }
    }
}
