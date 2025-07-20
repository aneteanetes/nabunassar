using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Entities.Map
{
    internal class Minimap
    {
        public RenderTarget2D Texture { get; set; }

        public Vector2 OriginSize { get; private set; }

        public Vector2 MapSize { get; private set; }

        public Minimap(Vector2 originSize, Vector2 mapSize)
        {
            OriginSize = originSize;
            MapSize = mapSize;
        }

        public void MovePlayer(Vector2 playerPosition)
        {
            var localPosition = TransformPosition(playerPosition);
            PlayerOnMinimap.Position = localPosition;
        }

        public bool IsPresentedOnMinimap(int entityId) => PresentedEntitites.Contains(entityId);

        public void Move(Vector2 position, int entityId)
        {
            var point = Points.FirstOrDefault(x => x.EntityId == entityId);
            if (point != null)
            {
                var localPosition = TransformPosition(position);
                point.Position = localPosition;
            }
        }

        public Vector2 TransformPosition(Vector2 position)
        {
            return new Vector2(
                position.X / OriginSize.X * MapSize.X,
                position.Y / OriginSize.Y * MapSize.Y);
        }

        public string AreaName { get; set; }

        public MinimapPoint PlayerOnMinimap { get; private set; }

        public List<MinimapPoint> Points { get; set; } = new();

        public HashSet<int> PresentedEntitites { get; private set; } = new();

        public void Add(MinimapPoint point)
        {
            Points.Add(point);
            PresentedEntitites.Add(point.EntityId);
        }

        public void AddPlayer(Vector2 playerPosition, int entityId)
        {
            var playerOnMinimap = new MinimapPoint()
            {
                EntityId = entityId,
                Name = "Вы",
                ObjectType = Nabunassar.Struct.ObjectType.Player,
                Position = TransformPosition(playerPosition),
            };

            PlayerOnMinimap = playerOnMinimap;
            PresentedEntitites.Add(entityId);
            Points.Add(playerOnMinimap);
        }
    }
}
