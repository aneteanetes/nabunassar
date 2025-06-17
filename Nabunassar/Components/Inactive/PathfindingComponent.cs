namespace Nabunassar.Components.Inactive
{
    internal class PathfindingComponent
    {
        public List<Vector2> Path { get; set; }

        public Roy_T.AStar.Paths.Path RoyToyPath { get; set; }
    }
}
