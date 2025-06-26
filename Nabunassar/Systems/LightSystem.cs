using MonoGame.Extended;
using MonoGame.Extended.ECS;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class LightSystem(NabunassarGame game) : BaseSystem(game, Aspect.One(typeof(LightComponent),typeof(HullComponent)))
    {
        private ComponentMapper<LightComponent> _lightMapper;
        private ComponentMapper<HullComponent> _hullMapper;
        private ComponentMapper<MapObject> _mapObjMapper;

        public override void Initialize(IComponentMapperService mapperService)
        {
            _lightMapper = mapperService.GetMapper<LightComponent>();
            _hullMapper = mapperService.GetMapper<HullComponent>();
            _mapObjMapper = mapperService.GetMapper<MapObject>();
        }

        public override void Update(GameTime gameTime, bool isSystem)
        {
            foreach (var entity in ActiveEntities)
            {
                var lightComp = _lightMapper.Get(entity);
                var hullComp = _hullMapper.Get(entity);

                RuntimeInitialization(lightComp, hullComp);

                var mapObj = _mapObjMapper.Get(entity);
                if (mapObj != null)
                {
                    if (lightComp != null)
                        lightComp.Move(mapObj.Origin);

                    if (hullComp != null)
                        hullComp.Move(mapObj.Position);
                }
            }

            base.Update(gameTime, isSystem);
        }

        private void RuntimeInitialization(LightComponent lightComp, HullComponent hullComp)
        {
            if (lightComp!=default && !lightComp.IsAdded)
            {
                foreach (var light in lightComp.Lights)
                {
                    if (!Game.Penumbra.Lights.Contains(light))
                        Game.Penumbra.Lights.Add(light);
                }

                lightComp.IsAdded = true;
            }

            if (hullComp != default && !hullComp.IsAdded)
            {
                foreach (var hull in hullComp.Hulls)
                {
                    if (!Game.Penumbra.Hulls.Contains(hull))
                        Game.Penumbra.Hulls.Add(hull);
                }

                hullComp.IsAdded = true;
            }
        }

        public override void Draw(GameTime gameTime, bool isSystem)
        {
            if (Game.Penumbra.Debug)
            {
                var sb = Game.BeginDraw();

                foreach (var entity in ActiveEntities)
                {
                    var hullComp = _hullMapper.Get(entity);

                    if (hullComp != null)
                    {
                        foreach (var hull in hullComp.Hulls)
                        {
                            sb.DrawPolygon(hull.Position, new MonoGame.Extended.Shapes.Polygon(hull.Points), Color.Cyan);
                        }
                    }
                }
            }

            base.Draw(gameTime, isSystem);
        }

    }
}