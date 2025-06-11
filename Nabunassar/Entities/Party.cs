namespace Nabunassar.Entities
{
    internal class Party
    {
        public Party()
        {
        }

        public List<Character> Characters { get; set; } = new();

        public Character GetLeadCharacter() => Characters.FirstOrDefault();
    }
}
