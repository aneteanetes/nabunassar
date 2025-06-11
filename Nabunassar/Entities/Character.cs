namespace Nabunassar.Entities
{
    internal class Character
    {
        public string Tileset { get; set; }

        public string Name { get; set; } = Guid.NewGuid().ToString();
    }
}
