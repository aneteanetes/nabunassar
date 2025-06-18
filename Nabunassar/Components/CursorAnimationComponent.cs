using Geranium.Reflection;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;

namespace Nabunassar.Components
{
    internal class CursorAnimationComponent
    {
        public CursorAnimationComponent()
        {

            SpriteSheetAnimation anim;

            //var builder = new SpriteSheetAnimationBuilder("cursor", null);
            //var anim = new MonoGame.Extended.Animations.An
        }
    }

    public class CursorAnimation : IAnimation
    {
        public string Name => nameof(CursorAnimation);

        public ReadOnlySpan<IAnimationFrame> Frames => throw new NotImplementedException();

        public int FrameCount => throw new NotImplementedException();

        public bool IsLooping => throw new NotImplementedException();

        public bool IsReversed => throw new NotImplementedException();

        public bool IsPingPong => throw new NotImplementedException();
    }
}
