using MonoGame.Extended;

namespace Nabunassar.Entities.Game
{
    internal interface IDistanceMeter
    {
        Result<bool> IsObjectNear(GameObject gameObject);

        RectangleF DistanceMeterRectangle { get; }
    }
}
