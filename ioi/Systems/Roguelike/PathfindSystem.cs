using ioi.Entities.Map;

namespace ioi.Systems.Roguelike;

internal class PathfindSystem
{
    private readonly RogueMap _map;

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

    public List<Vector2> FindPath(Vector2 startVec, Vector2 targetVec)
    {
        // Сразу работаем с целыми числами
        Point start = new Point((int)Math.Round(startVec.X), (int)Math.Round(startVec.Y));
        Point target = new Point((int)Math.Round(targetVec.X), (int)Math.Round(targetVec.Y));

        if (start == target) return new List<Vector2> { startVec };
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
        // Важно: ключ в Dictionary должен совпадать с типом Vector2 (как в твоем RogueMap)
        var key = new Vector2(p.X, p.Y);
        if (_map.ObjectMap.TryGetValue(key, out var objects))
            return !objects.Any(obj => obj.IsBounds);
        return true;
    }

    private List<Vector2> RetracePath(Node node)
    {
        var res = new List<Vector2>();
        while (node != null)
        {
            res.Add(new Vector2(node.Position.X, node.Position.Y));
            node = node.Parent;
        }
        res.Reverse();
        return res;
    }
}

