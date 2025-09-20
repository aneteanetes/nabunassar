namespace Nabunassar.Entities.Data.Animations
{
    internal class AnimationInfo
    {
        public string Name { get; set; }

        public bool IsLoop { get; set; }

        public List<AnimationFrameInfo> Frames { get; set; }
    }
}
