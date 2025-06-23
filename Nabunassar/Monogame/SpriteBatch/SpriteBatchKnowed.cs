using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Monogame.SpriteBatch
{
    internal class SpriteBatchKnowed : Microsoft.Xna.Framework.Graphics.SpriteBatch
    {
        public bool IsOpened { get; private set; }

        public NabunassarGame Game { get; set; }

        public SpriteBatchKnowed(NabunassarGame game, GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            Game = game;
        }

        public DepthStencilState DepthStencilState;

        public SpriteSortMode? spriteSortMode;

        public new void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            if (!IsOpened)
            {
                IsOpened = true;
                base.Begin(spriteSortMode ?? sortMode, blendState, samplerState, depthStencilState ?? DepthStencilState, rasterizerState, effect, transformMatrix);
            }
        }

        public void DrawText(string fontAsset, float size, string text, Vector2 position, Color color, float rotation = 0f, Vector2 origin = default(Vector2), Vector2? scale = null, float layerDepth = 0f, float characterSpacing = 0f, float lineSpacing = 0f, TextStyle textStyle = TextStyle.None, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0)
        {
            var fontSystem = Game.Content.LoadFont(fontAsset);
            var font = fontSystem.GetFont(size);

            font.DrawText(this, text, position, color, rotation, origin, scale, layerDepth, characterSpacing, lineSpacing, textStyle, effect, effectAmount);
        }

        public new void End()
        {
            if (IsOpened)
                base.End();

            IsOpened = false;
        }

        /// <summary>
        /// End then Begin with  params
        /// </summary>
        public void Flush()
        {
        }
    }
}