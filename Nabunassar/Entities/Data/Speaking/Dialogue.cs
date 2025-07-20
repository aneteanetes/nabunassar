namespace Nabunassar.Entities.Data.Speaking
{
    internal class Dialogue
    {
        public Dialogue(NabunassarGame game, DialogueModel model)
        {
            foreach (var block in model.graph)
            {
                var speakerToken = model.meta.npcs[block.Value.npcIndex].name;

                var round = new DialogueRound()
                {
                    Name = block.Key,
                    Text = block.Value.q,
                    Speaker = game.Strings["ObjectNames"][speakerToken]
                };

                foreach (var answ in block.Value.o)
                {
                    round.Replics.Add(new DialogueReplica(this)
                    {
                        Text = answ.a,
                        NextRoundName = answ.next
                    });
                }

                Rounds.Add(round);
            }

            foreach (var replica in Rounds.SelectMany(r=>r.Replics))
            {
                if(replica.NextRoundName!=default)
                {
                    replica.NextRound = Rounds.FirstOrDefault(x => x.Name == replica.NextRoundName);
                }
            }

            InitialRound = Rounds.FirstOrDefault();
            CurrentRound = InitialRound;

            EndDialogueReplica = new DialogueReplica(this)
            {
                Text = game.Strings["UI"]["ExitDialogue"]
            };
        }

        public List<DialogueRound> Rounds { get; set; } = new();

        public DialogueRound CurrentRound { get; set; }

        public DialogueReplica EndDialogueReplica { get; set; }

        public DialogueRound InitialRound { get; set; }
    }
}