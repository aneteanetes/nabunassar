using ioi.Monogame.Cameras;
using ioi.Monogame.Viewports;
using Microsoft.Xna.Framework.Graphics;

namespace ioi
{
    internal partial class GameHost : Game
    {
        public Viewport MainViewport { get; set; }
        public Viewport MapViewport { get; set; }

        public Vector2 CellSize { get; private set; } = new Vector2(16, 34);

        private void InitializeCameras()
        {
            viewportAdapter = new BoxingViewportAdapterCustom(this,Window, GraphicsDevice, MainViewport, Settings.OriginWidthPixel, Settings.OriginHeightPixel);
            CameraMain = new OrthographicCameraCustom(viewportAdapter);
            viewportAdapter.Reset();
            
            var mapWidth = GraphicsDevice.Viewport.Width * MapViewportAdapter.WidthPercent;
            var mapHeight = GraphicsDevice.Viewport.Height * MapViewportAdapter.HeightPercent;

            MapViewport = new Viewport(((int)CellSize.X), ((int)CellSize.Y), (int)mapWidth, (int)mapHeight);

            var mapWidthOrigin = 1920 * MapViewportAdapter.WidthPercent;
            var mapHeightOrigin = 1080 * MapViewportAdapter.HeightPercent;

            var mapAdapter = new MapViewportAdapter(this, Window, MapViewport, (int)mapWidthOrigin, (int)mapHeightOrigin);
            mapAdapter.Reset();
            CameraMap = new OrthographicCameraCustom(mapAdapter);
        }
    }    
}