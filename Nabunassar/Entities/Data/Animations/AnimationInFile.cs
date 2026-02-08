namespace Nabunassar.Entities.Data.Animations
{
    internal class AnimationInFile
    {
        public string Name { get; set; }

        public bool IsLoop { get; set; }

        public List<AnimationInFileFrame> Frames { get; set; }
    }
}
