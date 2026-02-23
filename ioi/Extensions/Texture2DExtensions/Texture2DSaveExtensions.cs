using Microsoft.Xna.Framework.Graphics;

namespace ioi.Extensions.Texture2DExtensions
{
    internal static class Texture2DSaveExtensions
    {
        internal static string SaveAsScreenshot(this Texture2D texure)
        {
            var screenshotDir = "Screenshots";

            if (!Directory.Exists(screenshotDir))
                Directory.CreateDirectory(screenshotDir);

            var dt = DateTime.Now;
            var path = Path.Combine(screenshotDir, $"ScreenShot_{dt:dd.MM.yyyy_HHmmss}.jpg");
            using var file = File.Create(path);
            texure.SaveAsJpeg(file, texure.Width, texure.Height);

            return path;
        }
    }
}