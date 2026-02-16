using MonoGame.Extended;

namespace ioi.Entities.Game
{
    internal interface IDistanceMeter
    {
        Result<bool> IsObjectNear(GameObject gameObject);

        RectangleF DistanceMeterRectangle { get; }
    }
}
