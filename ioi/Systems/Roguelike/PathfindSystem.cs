using ioi.Entities.Map;
using MoonSharp.Interpreter;

namespace ioi.Systems.Roguelike;

[MoonSharpUserData]
internal class PathfindSystem : IDisposable
{
    private RogueMap _map;

    public PathfindSystem(RogueMap map) => _map = map;

    private class Node
    {
        public Point Position;
        public float G; // Реальная стоимость пути
        public float H; // Предполагаемая стоимость (эвристика)
        public float F => G + H;
        public Node Parent;
        public Node(Point pos) => Position = pos;
    }

    public List<Point> FindPath(Point start, Point target)
    {
        var targetX = Math.Clamp(target.X,0,_map.Width-1);
        var targetY = Math.Clamp(target.Y, 0, _map.Height-1);

        target = new Point(targetX, targetY);

        if (start == target) return new List<Point> { start };
        if (!IsWalkable(target)) return null;

        // Очередь с приоритетом: всегда отдает узел с минимальным F
        var openQueue = new PriorityQueue<Node, float>();
        var openNodes = new Dictionary<Point, Node>(); // Для быстрого поиска узла в очереди
        var closedSet = new HashSet<Point>();

        var startNode = new Node(start) { G = 0, H = GetHeuristic(start, target) };
        openQueue.Enqueue(startNode, startNode.F);
        openNodes.Add(start, startNode);

        while (openQueue.Count > 0)
        {
            var current = openQueue.Dequeue();

            if (current.Position == target)
                return RetracePath(current);

            openNodes.Remove(current.Position);
            closedSet.Add(current.Position);

            foreach (var neighborPos in GetNeighbors(current.Position))
            {
                if (closedSet.Contains(neighborPos)) continue;

                // Стоимость шага: 1 для прямых, 1.4 для диагоналей
                float dist = (current.Position.X != neighborPos.X && current.Position.Y != neighborPos.Y) ? 1.4f : 1.0f;
                float tentativeG = current.G + dist;

                if (!openNodes.TryGetValue(neighborPos, out var neighborNode) || tentativeG < neighborNode.G)
                {
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighborPos);
                        neighborNode.H = GetHeuristic(neighborPos, target);
                        openNodes.Add(neighborPos, neighborNode);
                    }

                    neighborNode.Parent = current;
                    neighborNode.G = tentativeG;

                    // Переподаем в очередь с обновленным приоритетом
                    openQueue.Enqueue(neighborNode, neighborNode.F);
                }
            }
        }
        return null; // Путь не найден
    }

    private float GetHeuristic(Point a, Point b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        // Расстояние Чебышёва для 8 направлений + небольшой множитель (1.001) 
        // чтобы алгоритм меньше "раздумывал" на пустых участках
        return Math.Max(dx, dy) * 1.001f;
    }

    private IEnumerable<Point> GetNeighbors(Point p)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Point n = new Point(p.X + x, p.Y + y);

                // Просто проверяем: свободна ли сама клетка, куда мы хотим шагнуть
                if (IsWalkable(n))
                {
                    yield return n;
                }
            }
        }
    }

    private bool IsWalkable(Point p)
    {
        var key = new Vector2(p.X, p.Y);

        var boundsExists = _map.ObjectMap[p.X, p.Y].Objects.Any(x => x.IsBounds);

        return !boundsExists;
    }

    private List<Point> RetracePath(Node node)
    {
        var res = new List<Point>();
        while (node != null)
        {
            res.Add(node.Position);
            node = node.Parent;
        }
        res.Reverse();
        return res;
    }

    public void Dispose()
    {
        _map = null;
    }
}

