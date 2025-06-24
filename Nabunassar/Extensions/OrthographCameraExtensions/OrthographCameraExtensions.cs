using Assimp;

namespace Nabunassar.Extensions.OrthographCameraExtensions
{
    internal static class OrthographCameraExtensions
    {
        internal static void ZoomToPoint(this NabunassarGame game, Vector2 worldPoint, float deltaZoom)
        {
            var camera = game.Camera;
            float pastZoom = camera.Zoom;
            camera.Zoom = MathHelper.Clamp(camera.Zoom + deltaZoom, 0.1f, 10f); // Example zoom limits

            // Calculate the difference between zoom center and camera's origin
            Vector2 offset = worldPoint - camera.Origin;

            // Calculate the zoom delta
            float zoomDifference = camera.Zoom - pastZoom;

            // Adjust the camera's position
            prevCameraPosition= camera.Position;
            camera.Position += offset * (zoomDifference / camera.Zoom);
        }

        private static Vector2 prevCameraPosition;

        internal static void ZoomOut(this NabunassarGame game, Vector2 fromPoint, float deltaZoom)
        {
            var camera = game.Camera;

            camera.Zoom = MathHelper.Clamp(camera.Zoom - deltaZoom, 0.1f, 10f);
            camera.Position = prevCameraPosition;
        }
    }
}
