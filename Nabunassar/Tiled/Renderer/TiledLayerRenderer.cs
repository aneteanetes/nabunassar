using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Tiled.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabunassar.Tiled.Renderer
{
    internal class TiledLayerRenderer : Renderable
    {
        TiledLayer _layer;
        List<TiledTileRenderer> _tiles = new();

        public TiledLayerRenderer(NabunassarGame game, TiledLayer layer) : base(game)
        {
            _layer = layer;
        }

        public override void LoadContent()
        {
            foreach (var tile in _layer.Tiles)
            {
                if (tile.Gid == 0)
                    continue;

                var tileRenderer = new TiledTileRenderer(Game, tile);
                tileRenderer.LoadContent();
                this._tiles.Add(tileRenderer);
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var tile in _tiles)
            {
                tile.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
        {
            foreach (var tile in _tiles)
            {
                tile.Draw(gameTime, spriteBatch);
            }
        }
    }
}
