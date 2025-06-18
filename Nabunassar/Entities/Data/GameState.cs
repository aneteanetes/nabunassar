using GoRogue.Pathing;
using Nabunassar.Entities.Game;
using Roy_T.AStar.Grids;
using SadRogue.Primitives.GridViews;

namespace Nabunassar.Entities.Data
{
    internal class GameState
    {
        public Party Party { get; set; }

        public Character ActiveCharacter => Party?.Characters?.FirstOrDefault();

        public Cursor Cursor { get; set; } = new();
    }
}
