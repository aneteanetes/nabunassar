namespace Nabunassar.Tiled.Map
{
    public class TiledPolygonObject : TiledBase
    {
        public TiledPolygonObject(IEnumerable<Vector2> dots)
        {
            Vertices = dots.ToArray();
        }

        public Vector2[] Vertices { get; set; }
    }
}
