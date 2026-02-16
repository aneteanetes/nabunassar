using ioi.Monogame.Viewports;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace ioi.Monogame.Cameras
{
    //
    // Summary:
    //     Represents an orthographic (2D) camera that provides view and projection transformations
    //     for rendering within a 2D world.
    public sealed class OrthographicCameraCustom : Camera<Vector2>, IMovable, IRotatable
    {
        private readonly ViewportAdapterCustom _viewportAdapter;

        private float _maximumZoom = float.MaxValue;

        private float _minimumZoom;

        private float _zoom;

        private float _pitch;

        private float _maximumPitch = float.MaxValue;

        private float _minimumPitch;

        private Vector2 _position;

        private Rectangle _worldBounds;

        private bool _clampZoomToWorldBounds;

        //
        // Remarks:
        //     When MonoGame.Extended.OrthographicCamera.IsClampedToWorldBounds is true, the
        //     camera position is clamped so that its view remains within the defined MonoGame.Extended.OrthographicCamera.WorldBounds.
        public override Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                if (IsClampedToWorldBounds)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        public override float Rotation { get; set; }

        //
        // Remarks:
        //     When MonoGame.Extended.OrthographicCamera.IsClampedToWorldBounds is true, the
        //     camera zoom is clamped so that its view remains within the defined MonoGame.Extended.OrthographicCamera.WorldBounds.
        public override float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                bool flag = CanClampToWorldBounds();
                if (IsZoomClampedToWorldBounds && flag)
                {
                    ClampZoomToWorldBounds();
                }

                _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);
                if (flag)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        public override float MinimumZoom
        {
            get
            {
                return _minimumZoom;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0f, "value");
                _minimumZoom = value;
                bool flag = CanClampToWorldBounds();
                if (IsZoomClampedToWorldBounds && flag)
                {
                    ClampZoomToWorldBounds();
                }

                _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);
                if (flag)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        public override float MaximumZoom
        {
            get
            {
                return _maximumZoom;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0f, "value");
                _maximumZoom = value;
                bool flag = CanClampToWorldBounds();
                if (IsZoomClampedToWorldBounds && flag)
                {
                    ClampZoomToWorldBounds();
                }

                _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);
                if (flag)
                {
                    ClampPositionToWorldBounds();
                }
            }
        }

        [Obsolete("Pitch will be removed in the next major version")]
        public override float Pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                _pitch = MathHelper.Clamp(value, _minimumPitch, _maximumPitch);
            }
        }

        [Obsolete("Pitch will be removed in the next major version")]
        public override float MinimumPitch
        {
            get
            {
                return _minimumPitch;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0f, "value");
                _minimumPitch = value;
                _pitch = MathHelper.Clamp(_pitch, _minimumPitch, _maximumPitch);
            }
        }

        [Obsolete("Pitch will be removed in the next major version")]
        public override float MaximumPitch
        {
            get
            {
                return _maximumPitch;
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0f, "value");
                _maximumPitch = value;
                _pitch = MathHelper.Clamp(_pitch, _minimumPitch, _maximumPitch);
            }
        }

        public override RectangleF BoundingRectangle
        {
            get
            {
                Vector3[] corners = GetBoundingFrustum().GetCorners();
                Vector3 vector = corners[0];
                Vector3 vector2 = corners[2];
                float width = vector2.X - vector.X;
                float height = vector2.Y - vector.Y;
                return new RectangleF(vector.X, vector.Y, width, height);
            }
        }

        public override Vector2 Origin { get; set; }

        public override Vector2 Center => Position + Origin;

        //
        // Summary:
        //     Gets the bounding rectangle that defines the limits of the camera's movement.
        //
        //
        // Remarks:
        //     Use MonoGame.Extended.OrthographicCamera.EnableWorldBounds(Microsoft.Xna.Framework.Rectangle)
        //     to set world bounds and enable constraints, or MonoGame.Extended.OrthographicCamera.DisableWorldBounds
        //     to remove constraints.
        public Rectangle WorldBounds => _worldBounds;

        //
        // Summary:
        //     Gets a value indicating whether the camera is currently constrained within world
        //     bounds.
        //
        // Remarks:
        //     Use MonoGame.Extended.OrthographicCamera.EnableWorldBounds(Microsoft.Xna.Framework.Rectangle)
        //     to enable world bounds constraints, or MonoGame.Extended.OrthographicCamera.DisableWorldBounds
        //     to disable them.
        public bool IsClampedToWorldBounds { get; private set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the camera zoom should be clamped to
        //     world bounds.
        //
        // Remarks:
        //     When true, the camera zoom is constrained so that the view cannot extend beyond
        //     the world bounds. When false, zoom is only constrained by MonoGame.Extended.OrthographicCamera.MinimumZoom
        //     and MonoGame.Extended.OrthographicCamera.MaximumZoom. This property only has
        //     effect when MonoGame.Extended.OrthographicCamera.IsClampedToWorldBounds is true.
        public bool IsZoomClampedToWorldBounds
        {
            get
            {
                return _clampZoomToWorldBounds;
            }
            set
            {
                _clampZoomToWorldBounds = value;
                if (value)
                {
                    ClampZoomToWorldBounds();
                    _zoom = MathHelper.Clamp(_zoom, _minimumZoom, _maximumZoom);
                    ClampPositionToWorldBounds();
                }
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the MonoGame.Extended.OrthographicCamera class
        //     using the specified viewport adapter.
        //
        // Parameters:
        //   viewportAdapter:
        //     The viewport adapter that defines how world and screen coordinates are transformed.
        public OrthographicCameraCustom(ViewportAdapterCustom viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;
            Rotation = 0f;
            Zoom = 1f;
            Pitch = 1f;
            Origin = new Vector2((float)viewportAdapter.VirtualWidth / 2f, (float)viewportAdapter.VirtualHeight / 2f);
            Position = Vector2.Zero;
        }

        public override void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix.CreateRotationZ(0f - Rotation));
        }

        public override void Rotate(float deltaRadians)
        {
            Rotation += deltaRadians;
        }

        public override void ZoomIn(float deltaZoom)
        {
            Zoom += deltaZoom;
        }

        //
        // Summary:
        //     Increases the camera's zoom level while maintaining a specified world position
        //     as the zoom center.
        //
        // Parameters:
        //   deltaZoom:
        //     The amount to increase the zoom by.
        //
        //   zoomCenter:
        //     The world position to use as the zoom center. This point will remain fixed in
        //     screen space as the zoom changes.
        public void ZoomIn(float deltaZoom, Vector2 zoomCenter)
        {
            float zoom = Zoom;
            Zoom += deltaZoom;
            if (Zoom != zoom)
            {
                Position += (zoomCenter - Origin - Position) * ((Zoom - zoom) / Zoom);
            }
        }

        public override void ZoomOut(float deltaZoom)
        {
            Zoom -= deltaZoom;
        }

        //
        // Summary:
        //     Decreases the camera's zoom level while maintaining a specified world position
        //     as the zoom center.
        //
        // Parameters:
        //   deltaZoom:
        //     The amount to decrease the zoom by.
        //
        //   zoomCenter:
        //     The world position to use as the zoom center. This point will remain fixed in
        //     screen space as the zoom changes.
        public void ZoomOut(float deltaZoom, Vector2 zoomCenter)
        {
            float zoom = Zoom;
            Zoom -= deltaZoom;
            if (Zoom != zoom)
            {
                Position += (zoomCenter - Origin - Position) * ((Zoom - zoom) / Zoom);
            }
        }

        [Obsolete("Pitch will be removed in the next major version")]
        public override void PitchUp(float deltaPitch)
        {
            Pitch += deltaPitch;
        }

        [Obsolete("Pitch will be removed in the next major version")]
        public override void PitchDown(float deltaPitch)
        {
            Pitch -= deltaPitch;
        }

        //
        // Remarks:
        //     The camera is positioned so that the specified position appears at the center
        //     of the viewport.
        public override void LookAt(Vector2 position)
        {
            Position = position - new Vector2((float)_viewportAdapter.VirtualWidth / 2f, (float)_viewportAdapter.VirtualHeight / 2f);
        }

        //
        // Summary:
        //     Converts a position from world coordinates to screen coordinates.
        //
        // Parameters:
        //   x:
        //     The x-position in world coordinates.
        //
        //   y:
        //     The y-position in world coordinates.
        //
        // Returns:
        //     The corresponding position in screen coordinates.
        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        public override Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Vector2 result = Vector2.Transform(worldPosition, GetViewMatrix());
            if (_viewportAdapter is ScalingViewportAdapterCustom)
            {
                Viewport viewport = _viewportAdapter.Viewport;
                result += new Vector2(viewport.X, viewport.Y);
            }

            return result;
        }

        //
        // Summary:
        //     Converts a position from screen coordinates to world coordinates.
        //
        // Parameters:
        //   x:
        //     The x-position in screen coordinates.
        //
        //   y:
        //     The y-position in screen coordinates.
        //
        // Returns:
        //     The corresponding position in world coordinates.
        public Vector2 ScreenToWorld(float x, float y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public override Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            if (_viewportAdapter is ScalingViewportAdapterCustom)
            {
                Viewport viewport = _viewportAdapter.Viewport;
                screenPosition -= new Vector2(viewport.X, viewport.Y);
            }

            return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix()));
        }

        //
        // Summary:
        //     Gets the view transformation matrix for the camera, applying a parallax factor.
        //
        //
        // Parameters:
        //   parallaxFactor:
        //     The parallax factor to apply to the camera position. A value of (1,1) applies
        //     no parallax, while values closer to (0,0) create a stronger parallax effect for
        //     background layers.
        //
        // Returns:
        //     A Microsoft.Xna.Framework.Matrix representing the camera's view transformation
        //     with the specified parallax factor applied.
        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            return GetVirtualViewMatrix(parallaxFactor) * _viewportAdapter.GetScaleMatrix();
        }

        private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0f)) * Matrix.CreateTranslation(new Vector3(-Origin, 0f)) * Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom, Zoom * Pitch, 1f) * Matrix.CreateTranslation(new Vector3(Origin, 0f));
        }

        private Matrix GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(Vector2.One);
        }

        public override Matrix GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }

        public override Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            Matrix matrix = Matrix.CreateOrthographicOffCenter(0f, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight, 0f, -1f, 0f);
            Matrix.Multiply(ref viewMatrix, ref matrix, out matrix);
            return matrix;
        }

        public override BoundingFrustum GetBoundingFrustum()
        {
            Matrix virtualViewMatrix = GetVirtualViewMatrix();
            return new BoundingFrustum(GetProjectionMatrix(virtualViewMatrix));
        }

        //
        // Summary:
        //     Determines whether the camera's view contains the specified point.
        //
        // Parameters:
        //   point:
        //     The point to test, in world coordinates.
        //
        // Returns:
        //     A Microsoft.Xna.Framework.ContainmentType indicating whether the point is inside,
        //     outside, or intersects the camera's view.
        public ContainmentType Contains(Point point)
        {
            return Contains(point.ToVector2());
        }

        public override ContainmentType Contains(Vector2 vector2)
        {
            return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0f));
        }

        public override ContainmentType Contains(Rectangle rectangle)
        {
            Vector3 max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            Vector3 min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            BoundingBox box = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(box);
        }

        //
        // Summary:
        //     Enables world bounds constraint for the camera and sets the bounding rectangle.
        //
        //
        // Parameters:
        //   worldBounds:
        //     The bounding rectangle that defines the limits of the camera's movement and zoom.
        //
        //
        // Remarks:
        //     When world bounds are enabled, the camera position and zoom are automatically
        //     clamped to ensure the visible area does not extend beyond the specified bounds.
        //     This only applies when the camera has no rotation and the pitch is 1.0.
        public void EnableWorldBounds(Rectangle worldBounds)
        {
            _worldBounds = worldBounds;
            IsClampedToWorldBounds = true;
            ClampPositionToWorldBounds();
        }

        //
        // Summary:
        //     Disables world bounds constraint for the camera.
        //
        // Remarks:
        //     When world bounds are disabled, the camera can move and zoom freely without any
        //     constraints. The world bounds rectangle is reset to Microsoft.Xna.Framework.Rectangle.Empty.
        public void DisableWorldBounds()
        {
            _worldBounds = Rectangle.Empty;
            IsClampedToWorldBounds = false;
        }

        private void ClampZoomToWorldBounds()
        {
            Vector2 vector = new Vector2(_viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight) / _zoom;
            if (vector.X > (float)_worldBounds.Width || vector.Y > (float)_worldBounds.Height)
            {
                float value = (float)_viewportAdapter.VirtualWidth / (float)_worldBounds.Width;
                float value2 = (float)_viewportAdapter.VirtualHeight / (float)_worldBounds.Height;
                float num = MathHelper.Max(value, value2);
                if (_zoom < num)
                {
                    _zoom = num;
                }
            }
        }

        private void ClampPositionToWorldBounds()
        {
            Vector2 vector = new Vector2(_viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight) / _zoom;
            if ((float)_worldBounds.Width < vector.X || (float)_worldBounds.Height < vector.Y)
            {
                _position = _worldBounds.Center.ToVector2() - Origin;
                return;
            }

            Matrix inverseViewMatrix = GetInverseViewMatrix();
            Vector2 vector2 = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            Vector2 min = new Vector2(_worldBounds.Left, _worldBounds.Top);
            Vector2 vector3 = new Vector2(_worldBounds.Right, _worldBounds.Bottom);
            Vector2 vector4 = _position - vector2;
            _position = Vector2.Clamp(vector2, min, vector3 - vector) + vector4;
        }

        private bool CanClampToWorldBounds()
        {
            if (!IsClampedToWorldBounds || _worldBounds.Width <= 0 || _worldBounds.Height <= 0)
            {
                return false;
            }

            if (MathHelper.Distance(Rotation, 0f) >= 0.001f || MathHelper.Distance(Pitch, 1f) >= 0.001f)
            {
                return false;
            }

            return true;
        }
    }
}
