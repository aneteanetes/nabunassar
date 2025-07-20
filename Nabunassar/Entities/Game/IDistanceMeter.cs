using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabunassar.Entities.Game
{
    internal interface IDistanceMeter
    {
        Result<bool> IsObjectNear(GameObject gameObject);

        RectangleF DistanceMeterRectangle { get; }
    }
}
