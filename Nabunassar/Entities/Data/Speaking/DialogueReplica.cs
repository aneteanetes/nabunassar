namespace Nabunassar.Entities.Data.Speaking
{
    internal class DialogueReplica
    {
        private Dialogue _dialogue;

        public DialogueReplica(Dialogue dialogue)
        {
            _dialogue=dialogue;
        }

        public string Text { get; set; }

        public string NextRoundName { get; set; }

        public DialogueRound NextRound { get; set; }

        public DialogueRound Select()
        {
            return _dialogue.CurrentRound = NextRound;
        }
    }
}
