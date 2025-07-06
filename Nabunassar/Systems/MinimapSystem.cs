using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using Nabunassar.Entities.Map;

namespace Nabunassar.Systems
{
    internal class MinimapSystem : BaseSystem
    {
        private ComponentMapper<Minimap> minimapMapper;

        public MinimapSystem(NabunassarGame game) : base(game, Aspect.One(typeof(Minimap)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            minimapMapper = mapperService.GetMapper<Minimap>();
        }

        public override void Draw(GameTime gameTime, bool isSystem)
        {
            var sb = Game.BeginDraw(false);
            if (sb.IsOpened)
                sb.End();

            foreach (var entity in ActiveEntities)
            {
                var minimap = minimapMapper.Get(entity);

                Game.GraphicsDevice.SetRenderTarget(minimap.Texture);
                Game.GraphicsDevice.Clear(Color.Black);
                sb = Game.BeginDraw(false);

                Color dotColor = Color.White;

                foreach (var point in minimap.Points)
                {
                    dotColor = GetPointColor(dotColor, point);
                    sb.DrawPoint(point.Position, dotColor);
                }

                sb.End();
                
                Game.GraphicsDevice.SetRenderTarget(null);

                //debug draw
                //sb = Game.BeginDraw();
                //sb.Draw(minimap.Texture, new Vector2(50), Color.White);
            }

            base.Draw(gameTime, isSystem);
        }

        private static Color GetPointColor(Color dotColor, MinimapPoint point)
        {
            switch (point.ObjectType)
            {
                case Struct.ObjectType.NPC:
                    dotColor = Color.LightBlue;
                    break;
                case Struct.ObjectType.Object:
                    dotColor = Color.Gray;
                    break;
                case Struct.ObjectType.Player:
                    dotColor = Color.Red;
                    break;
                default:
                    dotColor = Color.White;
                    break;
            }

            return dotColor;
        }
    }
}
