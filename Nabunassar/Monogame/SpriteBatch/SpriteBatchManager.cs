﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.Content;
using System.Xml.Linq;

namespace Nabunassar.Monogame.SpriteBatch
{
    internal class SpriteBatchManager : IDisposable
    {
        private NabunassarContentManager _contentManager;
        private GraphicsDevice _graphicsDevice;
        private Matrix? _resolutionMatrix;
        private static readonly RasterizerState antialise = new() { MultiSampleAntiAlias = true };
        private bool isBegined = false;
        public const string NoEffectNameConstant = "#@$NO_%EFF3CT^";
        private NabunassarGame _game;

        private Dictionary<string, SpriteBatchKnowed> SpriteBatches = new Dictionary<string, SpriteBatchKnowed>();

        public SpriteBatchManager(NabunassarGame game, GraphicsDevice graphicsDevice, NabunassarContentManager contentManager)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
        }

        public void Begin(Matrix? resolutionMatrix = default)
        {
            _resolutionMatrix = resolutionMatrix == default ? _game.ResolutionMatrix : resolutionMatrix;
            isBegined = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samplerState"></param>
        /// <param name="alphaBlend"></param>
        /// <param name="isTransformMatrix"></param>
        /// <param name="effect">if null then <see cref="NoEffectNameConstant"/> constant</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SpriteBatchKnowed GetSpriteBatch(SamplerState samplerState = null, bool alphaBlend = false, bool isTransformMatrix = true/*, IEffect effect = default*/)
        {
            if (!isBegined)
                throw new Exception("SpriteBatchManager needs to be Begin() for work with actual resolution matrix!");

            if (samplerState == null)
                samplerState = SamplerState.PointWrap;

            var effectName = "";//effect == null ? NoEffectNameConstant : effect.Name;

            var key = $"{samplerState}{alphaBlend}{isTransformMatrix}{effectName}";
            if (!SpriteBatches.TryGetValue(key, out var spriteBatch))
            {
                SpriteBatches[key] = spriteBatch = new SpriteBatchKnowed(_game,_graphicsDevice);
            }

            if (!spriteBatch.IsOpened)
            {
                spriteBatch.Begin(
                    transformMatrix: _resolutionMatrix,
                    samplerState: samplerState,
                    blendState: alphaBlend ? BlendState.AlphaBlend : BlendState.NonPremultiplied, /*GetBlendState(useLight, alphaBlend, colorInvert),*/
                    //effect: GetEffect(effectName),
                    depthStencilState: spriteBatch.DepthStencilState,
                    rasterizerState: antialise);
            }

            return spriteBatch;
        }

        public void End()
        {
            foreach (var kv in SpriteBatches)
            {
                if (kv.Value.IsOpened)
                    kv.Value.End();
            }
            isBegined = false;
        }

        private Effect GetEffect(string effectName)
        {
            if (effectName == NoEffectNameConstant)
                return null;

            if (!XnaEffectsLoaded.TryGetValue(effectName, out var xnaeff))
            {
                xnaeff = _contentManager.Load<Effect>($"{effectName}");
                XnaEffectsLoaded[effectName] = xnaeff;
            }

            return xnaeff;
        }

        public void Dispose() => SpriteBatches.ForEach(kv => kv.Value.Dispose());

        private static Dictionary<string, Effect> XnaEffectsLoaded = new Dictionary<string, Effect>();
    }
}
