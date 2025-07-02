namespace Nabunassar.Tiled.Map
{
    public class TiledPolygonObject : TiledBase
    {
        public TiledPolygonObject(IEnumerable<Vector2> dots, Vector2 position=default)
        {
            Vertices = dots.ToArray();

            var left = Vertices.Min(v => v.X);
            var top = Vertices.Min(v => v.Y);

            Position = position != default ? position : new Vector2(left, top);
        }

        public Vector2[] Vertices { get; set; }
    }
}
