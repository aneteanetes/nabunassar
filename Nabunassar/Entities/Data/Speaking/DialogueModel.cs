namespace Nabunassar.Entities.Data.Speaking
{
    public class DialogueModel
    {
        public string DialogueFile { get; set; }

        public DialogueMeta meta { get; set; }

        public Dictionary<string, DialogueBlock> graph { get; set; }
    }

    public class DialogueMeta
    {
        public string name { get; set; }

        public List<DialogueNpc> npcs { get; set; }
    }

    public class DialogueNpc
    {
        public string name { get; set; }
    }

    public class DialogueBlock
    {
        public string q { get; set; }

        public List<DialogueAnswer> o { get; set; }

        public int npcIndex { get; set; }
    }

    public class DialogueAnswer
    {
        public string a { get; set; }

        public string next { get; set; }
    }
}