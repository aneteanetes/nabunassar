//-----------------------------------------------------------------------------
// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Shaders
{  
    internal class GaussianBlur : PostProcessShader
    {
        private int _BLUR_RADIUS = 7;
        private float _BLUR_AMOUNT = 2.0f;

        private static RenderTarget2D _renderTarget1;
        private static RenderTarget2D _renderTarget2;

        public GaussianBlur(NabunassarGame game, float blurAmount = 2.0f) : base(game, "Assets/Shaders/GaussianBlur.fx")
        {
            if (blurAmount != _BLUR_AMOUNT)
            {
                _BLUR_AMOUNT = blurAmount;
                ComputeKernel(_BLUR_RADIUS, _BLUR_AMOUNT);
            }
        }

        public override void LoadContent()
        {
            if (_renderTarget1 == null)
            {
                var renderTargetWidth = Game.Resolution.Width / 2;
                var renderTargetHeight = Game.Resolution.Height / 2;

                _renderTarget1 = new RenderTarget2D(Game.GraphicsDevice,
                    renderTargetWidth, renderTargetHeight, false,
                    Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None);

                _renderTarget2 = new RenderTarget2D(Game.GraphicsDevice,
                    renderTargetWidth, renderTargetHeight, false,
                    Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.None);

                // The texture offsets used by the Gaussian blur shader depends
                // on the dimensions of the render targets. The offsets need to be
                // recalculated whenever the render targets are recreated.

                ComputeOffsets(Game.Resolution.ToVector2());
                ComputeKernel(_BLUR_RADIUS, _BLUR_AMOUNT);
            }
        }

        private int _radius;
        private float _amount;
        private float _sigma;
        private float[] _kernel;
        private Vector2[] _offsetsHoriz;
        private Vector2[] _offsetsVert;

        /// <summary>
        /// Returns the radius of the Gaussian blur filter kernel in pixels.
        /// </summary>
        public int Radius
        {
            get { return _radius; }
        }

        /// <summary>
        /// Returns the blur amount. This value is used to calculate the
        /// Gaussian blur filter kernel's sigma value. Good values for this
        /// property are 2 and 3. 2 will give a more blurred result whilst 3
        /// will give a less blurred result with sharper details.
        /// </summary>
        public float Amount
        {
            get { return _amount; }
        }

        /// <summary>
        /// Returns the Gaussian blur filter's standard deviation.
        /// </summary>
        public float Sigma
        {
            get { return _sigma; }
        }

        /// <summary>
        /// Returns the Gaussian blur filter kernel matrix. Note that the
        /// kernel returned is for a 1D Gaussian blur filter kernel matrix
        /// intended to be used in a two pass Gaussian blur operation.
        /// </summary>
        public float[] Kernel
        {
            get { return _kernel; }
        }

        /// <summary>
        /// Returns the texture offsets used for the horizontal Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsX
        {
            get { return _offsetsHoriz; }
        }

        /// <summary>
        /// Returns the texture offsets used for the vertical Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsY
        {
            get { return _offsetsVert; }
        }

        /// <summary>
        /// Calculates the Gaussian blur filter kernel. This implementation is
        /// ported from the original Java code appearing in chapter 16 of
        /// "Filthy Rich Clients: Developing Animated and Graphical Effects for
        /// Desktop Java".
        /// </summary>
        /// <param name="blurRadius">The blur radius in pixels.</param>
        /// <param name="blurAmount">Used to calculate sigma.</param>
        public void ComputeKernel(int blurRadius, float blurAmount)
        {
            _radius = blurRadius;
            _amount = blurAmount;

            _kernel = null;
            _kernel = new float[_radius * 2 + 1];
            _sigma = _radius / _amount;

            float twoSigmaSquare = 2.0f * _sigma * _sigma;
            float sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            float total = 0.0f;
            float distance = 0.0f;
            int index = 0;

            for (int i = -_radius; i <= _radius; ++i)
            {
                distance = i * i;
                index = i + _radius;
                _kernel[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += _kernel[index];
            }

            for (int i = 0; i < _kernel.Length; ++i)
                _kernel[i] /= total;
        }

        /// <summary>
        /// Calculates the texture coordinate offsets corresponding to the
        /// calculated Gaussian blur filter kernel. Each of these offset values
        /// are added to the current pixel's texture coordinates in order to
        /// obtain the neighboring texture coordinates that are affected by the
        /// Gaussian blur filter kernel. This implementation has been adapted
        /// from chapter 17 of "Filthy Rich Clients: Developing Animated and
        /// Graphical Effects for Desktop Java".
        /// </summary>
        /// <param name="textureWidth">The texture width in pixels.</param>
        /// <param name="textureHeight">The texture height in pixels.</param>
        public void ComputeOffsets(Vector2 resolution)
        {
            _offsetsHoriz = null;
            _offsetsHoriz = new Vector2[_radius * 2 + 1];

            _offsetsVert = null;
            _offsetsVert = new Vector2[_radius * 2 + 1];

            int index = 0;
            float xOffset = 1.0f / resolution.X;
            float yOffset = 1.0f / resolution.Y;

            for (int i = -_radius; i <= _radius; ++i)
            {
                index = i + _radius;
                _offsetsHoriz[index] = new Vector2(i * xOffset, 0.0f);
                _offsetsVert[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        /// <summary>
        /// Performs the Gaussian blur operation on the source texture image.
        /// The Gaussian blur is performed in two passes: a horizontal blur
        /// pass followed by a vertical blur pass. The output from the first
        /// pass is rendered to renderTarget1. The output from the second pass
        /// is rendered to renderTarget2. The dimensions of the blurred texture
        /// is therefore equal to the dimensions of renderTarget2.
        /// </summary>
        /// <param name="srcTexture">The source image to blur.</param>
        /// <param name="renderTarget1">Stores the output from the horizontal blur pass.</param>
        /// <param name="renderTarget2">Stores the output from the vertical blur pass.</param>
        /// <param name="spriteBatch">Used to draw quads for the blur passes.</param>
        /// <returns>The resulting Gaussian blurred image.</returns>
        public Texture2D PerformGaussianBlur(Texture2D srcTexture,
                                             RenderTarget2D renderTarget1,
                                             RenderTarget2D renderTarget2,
                                             SpriteBatch spriteBatch)
        {
            if (Effect == null)
                throw new InvalidOperationException("GaussianBlur.fx effect not loaded.");

            Texture2D outputTexture = null;
            Rectangle srcRect = new Rectangle(0, 0, srcTexture.Width, srcTexture.Height);
            Rectangle destRect1 = new Rectangle(0, 0, renderTarget1.Width, renderTarget1.Height);
            Rectangle destRect2 = new Rectangle(0, 0, renderTarget2.Width, renderTarget2.Height);

            // Perform horizontal Gaussian blur.

            Game.GraphicsDevice.SetRenderTarget(renderTarget1);

            Effect.CurrentTechnique = Effect.Techniques["GaussianBlur"];
            Effect.Parameters["weights"].SetValue(_kernel);
            Effect.Parameters["colorMap+colorMapTexture"].SetValue(srcTexture);
            Effect.Parameters["offsets"].SetValue(_offsetsHoriz);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, Effect);
            spriteBatch.Draw(srcTexture, destRect1, Color.White);
            spriteBatch.End();

            // Perform vertical Gaussian blur.

            Game.GraphicsDevice.SetRenderTarget(renderTarget2);
            outputTexture = (Texture2D)renderTarget1;

            Effect.Parameters["colorMap+colorMapTexture"].SetValue(outputTexture);
            Effect.Parameters["offsets"].SetValue(_offsetsVert);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, Effect);
            spriteBatch.Draw(outputTexture, destRect2, Color.White);
            spriteBatch.End();

            // Return the Gaussian blurred texture.

            Game.GraphicsDevice.SetRenderTarget(null);
            outputTexture = (Texture2D)renderTarget2;

            return outputTexture;
        }

        public override void Draw(GameTime gameTime, bool isLast = true)
        {
            var sb = Game.BeginDraw();
            sb.End();

            var backBuffer = Game.GetBackBuffer();

            var result = PerformGaussianBlur(backBuffer, _renderTarget1, _renderTarget2, sb);

            sb.Begin();
            sb.Draw(result, Game.Resolution, Color.White);
            sb.End();

            base.Draw(gameTime,isLast);
        }
    }
}