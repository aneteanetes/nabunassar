using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Nabunassar.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabunassar.Systems
{
    internal class BoundRenderPositionSystem : EntityUpdateSystem
    {
        private ComponentMapper<BoundsComponent> _boundsComponentMapper;
        private ComponentMapper<RenderComponent> _renderComponentMapper;
        private ComponentMapper<BoundRenderPositionComponent> _boundRenderComponentMapper;

        public BoundRenderPositionSystem():base(Aspect.All(typeof(BoundRenderPositionComponent), typeof(BoundsComponent),typeof(RenderComponent)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _boundsComponentMapper = mapperService.GetMapper<BoundsComponent>();
            _renderComponentMapper = mapperService.GetMapper<RenderComponent>();
            _boundRenderComponentMapper = mapperService.GetMapper<BoundRenderPositionComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var bounds = _boundsComponentMapper.Get(entityId);
                var render = _renderComponentMapper.Get(entityId);
                var boundRender = _boundRenderComponentMapper.Get(entityId);

                var boundPos = bounds.Position;

                render.Position = new Vector2(boundPos.X-boundRender.RenderOffset.X/4,boundPos.Y-boundRender.RenderOffset.Y);
            }
        }
    }
}
