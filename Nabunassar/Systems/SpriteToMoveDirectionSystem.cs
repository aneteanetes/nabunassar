using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class SpriteToMoveDirectionSystem : EntityUpdateSystem
    {
        ComponentMapper<AnimatedSprite> _animSpriteComponentMapper;

        public SpriteToMoveDirectionSystem() : base(Aspect.All(typeof(AnimatedPerson), typeof(AnimatedSprite), typeof(MoveComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _animSpriteComponentMapper = mapperService.GetMapper<AnimatedSprite>();
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
