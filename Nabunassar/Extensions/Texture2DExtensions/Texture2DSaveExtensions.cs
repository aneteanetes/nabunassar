using Microsoft.Xna.Framework.Graphics;
using StbImageWriteSharp;

namespace Nabunassar.Extensions.Texture2DExtensions
{
    internal static class Texture2DSaveExtensions
    {
        internal static unsafe string SaveAsScreenshot(this Texture2D texure)
        {
            var screenshotDir = "Screenshots";

            if (!Directory.Exists(screenshotDir))
                Directory.CreateDirectory(screenshotDir);

            var dt = DateTime.Now;
            var path = Path.Combine(screenshotDir, $"Nabunassar_{dt:dd.MM.yyyy_HHmmss}.jpg");
            using var file = File.Create(path);
            texure.SaveAsJpeg(file, texure.Width, texure.Height);

            return path;
        }
    }
}