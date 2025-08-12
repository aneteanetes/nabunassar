using Microsoft.Xna.Framework.Graphics;
using StbImageWriteSharp;

namespace Nabunassar.Extensions.Texture2DExtensions
{
    internal static class Texture2DSaveExtensions
    {
        internal static unsafe string MakeScreenshot(this Texture2D texure)
        {
            var screenshotDir = "Screenshots";

            if (!Directory.Exists(screenshotDir))
                Directory.CreateDirectory(screenshotDir);

            var path = Path.Combine(screenshotDir, $"Screenshot {DateTime.Now:dd.MM.yyyy HH mm ss}.jpg");
            using var file = File.Create(path);

            int colorDataLength = texure.Width * texure.Height;
            var screenshotData = new Color[colorDataLength];
            texure.GetData(0, null, screenshotData, 0, colorDataLength);

            var writer = new ImageWriter();
            fixed (Color* ptr = &screenshotData[0])
            {
                writer.WriteJpg(ptr, texure.Width, texure.Height, ColorComponents.RedGreenBlueAlpha, file, 95);
            }

            return path;
        }
    }
}