using Microsoft.Xna.Framework;

namespace Nabunassar.Monogame.Viewport
{
    public class PossibleResolution
    {
        public PossibleResolution() { }

        public PossibleResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public double CenterH(double width)
        {
            var w = (double)this.Width;
            return w / 2 - width / 2;
        }

        public double CenterV(double height)
        {
            var h = (double)this.Height;
            return h / 2 - height / 2;
        }

        public Vector2 Origin()
        {
            return new Vector2(this.Width / 2, this.Height / 2);
        }

        public int OriginPixel()
        {
            var pos = Origin();
            return Width * ((int)pos.Y) + ((int)pos.X);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PossibleResolution res))
                return false;

            return this.Width == res.Width && this.Height == res.Height;
        }

        public override int GetHashCode()
        {
            return this.Width + this.Height;
        }

        public override string ToString() => $"{Width}x{Height}";

        internal Vector2 ToVector2()
        {
            return new Vector2(Width, Height);
        }

        public static implicit operator Rectangle(PossibleResolution res) => new Rectangle(0, 0, res.Width, res.Height);
    }
}