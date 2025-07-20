namespace Nabunassar.Entities.Data.Speaking
{
    internal class DialogueRound
    {
        public string Name { get; set; }

        public string Speaker { get; set; }

        public string Text { get; set; }

        public List<DialogueReplica> Replics { get; set; } = new();
    }
}
