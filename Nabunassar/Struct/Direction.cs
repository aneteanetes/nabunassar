using System;

namespace Nabunassar.Struct
{
    [Flags]
    public enum Direction
    {
        Idle = 0,

        Up = 2,
        Down = 4,
        Left = 8,
        Right = 16,

        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right,

        LeftUp = UpLeft,
        RightUp = UpRight,
        LeftDown = DownLeft,
        RightDown = DownRight,
    }

    public enum Distance
    {
        Low = 0,
        Medium = 1,
        Hard = 1
    }

    public enum SimpleDirection
    {
        Up = 1,
        Down = 20,
        Left = 300,
        Right = 4000,
    }

    public enum Binary
    {
        Zero = 0,
        One = 1
    }
}

namespace Nabunassar
{
    using Microsoft.Xna.Framework;
    using MonoGame.Extended;
    using Nabunassar;
    using Nabunassar.Struct;

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction dir)
        {
            switch (dir)
            {

                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;

                case Direction.UpLeft: return Direction.DownRight;
                case Direction.UpRight: return Direction.DownLeft;
                case Direction.DownLeft: return Direction.UpRight;
                case Direction.DownRight: return Direction.UpLeft;

                default: return Direction.Idle;
            }
        }
        public static string ToStringUpDown(this Direction dir)
        {
            if (dir == Direction.LeftDown)
                return "DownLeft";

            if (dir == Direction.LeftUp)
                return "UpLeft";

            if (dir == Direction.RightUp)
                return "UpRight";

            if (dir == Direction.RightDown)
                return "DownRight";

            return dir.ToString();
        }

        public static Direction OppositeX(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;

                case Direction.Down: return Direction.Down;
                case Direction.Up: return Direction.Up;

                case Direction.UpLeft: return Direction.UpRight;
                case Direction.UpRight: return Direction.UpLeft;
                case Direction.DownLeft: return Direction.DownRight;
                case Direction.DownRight: return Direction.DownLeft;

                default: return Direction.Idle;
            }
        }

        public static bool IsDiagonal(this Direction dir)
        {
            switch (dir)
            {
                case Direction.UpLeft:
                case Direction.UpRight:
                case Direction.DownLeft:
                case Direction.DownRight:
                    return true;

                default: return false;
            }
        }

        public static Direction Rangom(this Direction dir)
        {            
            switch (NabunassarGame.Game.Random.Next(0, 9))
            {
                case 1: return Direction.Up;
                case 2: return Direction.Down;
                case 3: return Direction.Left;
                case 4: return Direction.Right;
                case 5: return Direction.UpLeft;
                case 6: return Direction.UpRight;
                case 7: return Direction.DownLeft;
                case 8: return Direction.DownRight;
                default: return Direction.Idle;
            }
        }

        public static string ToStringX(this Direction direction) => direction switch
        {
            Direction.Up => nameof(Direction.Up),
            Direction.Down => nameof(Direction.Down),
            Direction.Left => nameof(Direction.Left),
            Direction.Right => nameof(Direction.Right),
            Direction.UpLeft => nameof(Direction.UpLeft),
            Direction.UpRight => nameof(Direction.UpRight),
            Direction.DownLeft => nameof(Direction.DownLeft),
            Direction.DownRight => nameof(Direction.DownRight),
            _ => nameof(Direction.Idle),
        };

        public static bool Is(this Direction value, Direction direction) 
            => value.HasFlag(direction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static Direction DetectDirection(this Vector2 from, Vector2 to, double accuracy = 0)
        {
            Direction dirX = Direction.Idle;
            Direction dirY = Direction.Idle;

            if (Compare(true, to.X, from.X, accuracy))
            {
                dirX = Direction.Left;
            }

            if (Compare(false, to.X, from.X, accuracy))
            {
                dirX = Direction.Right;
            }

            if (Compare(false, to.Y, from.Y, accuracy))
            {
                dirY = Direction.Down;
            }

            if (Compare(true, to.Y, from.Y, accuracy))
            {
                dirY = Direction.Up;
            }

            return dirX | dirY;
        }

        public static Vector2 ToDirectionVector(this Direction direction) => direction switch
        {
            Direction.Up => new Vector2(0, -1),
            Direction.Down => new Vector2(0, 1),
            Direction.Left => new Vector2(-1, 0),
            Direction.Right => new Vector2(1, 0),
            Direction.UpLeft => new Vector2(-1, -1),
            Direction.UpRight => new Vector2(1, -1),
            Direction.DownLeft => new Vector2(-1, 1),
            Direction.DownRight => new Vector2(1, 1),
            _ => Vector2.Zero,
        };

        private static bool Compare(bool isLess, double a, double b, double accuracy)
        {
            var diff = Math.Abs(a - b);
            if (diff < accuracy)
                return false;

            static bool less(double x1, double x2) => x1 < x2;
            static bool more(double x1, double x2) => x1 > x2;

            return isLess ? less(a, b) : more(a, b);
        }
    }
}