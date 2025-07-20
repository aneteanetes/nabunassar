using MonoGame.Extended;

namespace Nabunassar
{
    internal static class OrthographCameraExtensions
    {
        internal static void ZoomToPoint(this OrthographicCamera camera, Vector2 worldPoint, float deltaZoom)
        {
            float pastZoom = camera.Zoom;
            camera.Zoom = MathHelper.Clamp(camera.Zoom + deltaZoom, 0.1f, 10f); // Example zoom limits

            // Calculate the difference between zoom center and camera's origin
            Vector2 offset = worldPoint - camera.Origin;

            // Calculate the zoom delta
            float zoomDifference = camera.Zoom - pastZoom;

            // Adjust the camera's position
            prevCameraZoomPosition= camera.Position;
            camera.Position += offset * (zoomDifference / camera.Zoom);
        }

        private static Vector2 prevCameraZoomPosition;

        internal static void ZoomOut(this OrthographicCamera camera, Vector2 fromPoint, float deltaZoom)
        {
            camera.Zoom = MathHelper.Clamp(camera.Zoom - deltaZoom, 0.1f, 10f);
            camera.Position = prevCameraZoomPosition;
        }

        private static Vector2 prevCameraViewPosition;

        internal static void ViewOn(this OrthographicCamera camera, Vector2 worldPosition, Vector2 leftBounds = default, Vector2 rightBounds = default)
        {
            prevCameraViewPosition = camera.Position;

            var newPos = new Vector2(
                worldPosition.X / 2 - prevCameraViewPosition.X / 2,
                worldPosition.Y / 2 + prevCameraViewPosition.Y / 2);

            if (leftBounds == default)
                leftBounds = _leftBounds;

            if (rightBounds == default)
                rightBounds = _rightBounds;

            newPos.X = Math.Max(newPos.X, leftBounds.X);
            newPos.Y = Math.Max(newPos.Y, leftBounds.Y);

            newPos.X = Math.Min(newPos.X, rightBounds.X);
            newPos.Y = Math.Min(newPos.Y, rightBounds.Y);

            camera.Position = newPos;
        }

        internal static void ViewReset(this OrthographicCamera camera)
        {
            camera.Position= prevCameraViewPosition;
        }

        private static Vector2 _leftBounds;
        private static Vector2 _rightBounds;

        internal static void SetBounds(this OrthographicCamera camera, Vector2 left, Vector2 right)
        {
            _leftBounds = left;
            _rightBounds = right;
        }
    }
}
