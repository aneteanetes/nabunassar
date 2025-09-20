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

                Game.SetRenderTarget(minimap.Texture);
                Game.ClearRenderTarget(Color.Black);
                sb = Game.BeginDraw(false);

                Color dotColor = Color.White;

                foreach (var point in minimap.Points.Where(x => x.IsVisible))
                {
                    dotColor = GetPointColor(dotColor, point);
                    sb.DrawPoint(point.Position, dotColor);
                }

                sb.End();
                
                Game.SetRenderTarget(null);

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
                    dotColor = Color.Blue;
                    break;
                case Struct.ObjectType.Object:
                    dotColor = Color.Gray;
                    break;
                case Struct.ObjectType.Container:
                    dotColor = Color.DarkGreen;
                    break;
                case Struct.ObjectType.Player:
                    dotColor = Color.IndianRed;
                    break;
                case Struct.ObjectType.Creature:
                    dotColor = Color.Red;
                    break;
                case Struct.ObjectType.Ground:
                    {
                        switch (point.GroundType)
                        {
                            case Struct.GroundType.Road:
                                dotColor = "#320e0e".AsColor();
                                break;
                            case Struct.GroundType.Grass:
                                dotColor = "#1a2a1b".AsColor();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    dotColor = Color.White;
                    break;
            }

            return dotColor;
        }
    }
}
