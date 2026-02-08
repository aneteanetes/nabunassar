using MonoGame.Extended.ECS;
using Nabunassar.Components;

namespace Nabunassar.Entities
{
    internal class BaseEntityFactory
    {
        protected NabunassarGame Game { get; private set; }

        public World FactoryWorld { get; private set; }

        public BaseEntityFactory(NabunassarGame game, World factoryWorld)
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
