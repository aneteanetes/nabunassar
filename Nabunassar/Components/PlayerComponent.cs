using Nabunassar.Entities.Data;

namespace Nabunassar.Components
{
    internal class PlayerComponent
    {
        NabunassarGame _game;
        public Character Character { get; private set; }

        public PlayerComponent(NabunassarGame game, Character character)
        {
            _game = game;
            Character = character;
        }
    }
}
