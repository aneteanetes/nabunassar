using ioi.Monogame.Cameras;
using ioi.Monogame.Viewports;
using Microsoft.Xna.Framework.Graphics;

namespace ioi
{
    internal partial class GameHost : Game
    {
        public Viewport mainViewport;
        public Viewport mapViewport;

        public Vector2 CellSize { get; private set; } = new Vector2(16, 34);

        private void InitializeCameras()
        {
            viewportAdapter = new BoxingViewportAdapterCustom(this,Window, GraphicsDevice, mainViewport, Settings.OriginWidthPixel, Settings.OriginHeightPixel);
            CameraMain = new OrthographicCameraCustom(viewportAdapter);
            viewportAdapter.Reset();
            
            var mapWidth = GraphicsDevice.Viewport.Width * MapViewportAdapter.WidthPercent;
            var mapHeight = GraphicsDevice.Viewport.Height * MapViewportAdapter.HeightPercent;

            mapViewport = new Viewport(((int)CellSize.X), ((int)CellSize.Y), (int)mapWidth, (int)mapHeight);

            var mapWidthOrigin = 1920 * MapViewportAdapter.WidthPercent;
            var mapHeightOrigin = 1080 * MapViewportAdapter.HeightPercent;

            var mapAdapter = new MapViewportAdapter(this, Window, mapViewport, (int)mapWidthOrigin, (int)mapHeightOrigin);
            mapAdapter.Reset();
            CameraMap = new OrthographicCameraCustom(mapAdapter);
        }
    }    
}