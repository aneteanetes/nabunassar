using ioi.Components;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using Myra.Graphics2D.UI;

namespace ioi.Entities.Map
{
    internal class RogueMap
    {
        public string NameToken { get; set; }

        public RogueMapCell[,] ObjectMap { get; set; }

        public int Width { get; }

        public int Height { get; }

        public List<Area> Areas { get; set; } = new();

        public Area CurrentArea { get; set; }

        public Dictionary<string, Texture2DAtlas> Tilesets { get; internal set; } = new();

        public List<ObjectMap> Updatable { get; set; } = new();

        public List<ObjectMap> Drawable { get; set; } = new();

        public RogueMap(int width, int height)
        {
            Width = width;
            Height = height;

            ObjectMap = new RogueMapCell[Width, Height];

            // Инициализируем списки сразу, чтобы избежать null-checks
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    ObjectMap[x, y].Objects = new List<ObjectMap>();
        }

        public void Add(ObjectMap obj)
        {
            var key = obj.KeyCoords();

            var cell = ObjectMap[key.X, key.Y];
            cell.Objects.Add(obj);

            if (obj.IsBounds)
            {
                ObjectMap[key.X, key.Y].Flags = 1;
            }

            if(obj.IsUpdatable)
                Updatable.Add(obj);

            Drawable.Add(obj);
        }

        public void Remove(ObjectMap obj)
        {
            if (obj.IsUpdatable)
                Updatable.Remove(obj);

            Drawable.Remove(obj);

            var cell = ObjectMap[obj.Coords.X, obj.Coords.Y];
            cell.Objects.Remove(obj);

            var isBlocked = cell.Objects.Any(x => x.IsBounds);
            ObjectMap[obj.Coords.X, obj.Coords.Y].Flags = (byte)(isBlocked ? 1 : 0);
        }

        internal bool Move(ObjectMap obj, Point coords)
        {
            var x = Math.Clamp(coords.X, 0, Width - 1);
            var y = Math.Clamp(coords.Y, 0, Height - 1);

            var cell = ObjectMap[x,y];

            if (cell.Flags == 1)
            {
                obj.StopMove();
                obj.MoveStopRequest = false;
                return false;
            }

            obj.ProcessCollision(cell.Objects);

            // move stopped
            if (obj.MoveStopRequest)
            {
                obj.MoveStopRequest = false;
                return false;
            }

            ObjectMap[obj.Coords.X, obj.Coords.Y].Objects.Remove(obj);

            obj.Coords = coords;

            ObjectMap[x, y].Objects.Add(obj);

            obj.IsMoving = true;
            obj.TargetPosition = obj.GetPositionFromCoords();

            return true;
        }
    }
}
