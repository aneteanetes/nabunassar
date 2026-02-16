using ioi.Entities.Game;
using ioi.Entities.Struct.FixedCollections.Octas;
using ioi.Struct;
using System.Diagnostics;

namespace ioi.Components
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
