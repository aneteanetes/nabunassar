using MonoGame.Extended.ECS;
using ioi.Components;

namespace ioi.Entities
{
    internal class BaseEntityFactory
    {
        protected GameHost Game { get; private set; }

        public World FactoryWorld { get; private set; }

        public BaseEntityFactory(GameHost game, World factoryWorld)
        {
            Game = game;
            FactoryWorld = factoryWorld;
        }

        public Entity CreateEntity(string descriptor = null, int order = 0)
        {
            var entity = FactoryWorld.CreateEntity();

            entity.Attach(new DescriptorComponent(descriptor));
            entity.Attach(new OrderComponent(order));

            return entity;
        }
    }
}
