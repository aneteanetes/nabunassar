using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using Nabunassar.ECS;
using Nabunassar.Systems;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        protected void InitGameWorlds()
        {
            WorldGame = new WorldBuilderProxy(this)
                .AddSystem(new PlayerControllSystem(this))
                .AddSystem(new CursorSystem(this))
                .AddSystem(new RenderSystem(this))
                .AddSystem(new FlickeringSystem(this))
                .AddSystem(new MoveSystem(this))
                .AddSystem(new MouseControlSystem(this))
                .AddSystem(new MouseMapObjectFocusSystem(this))
                .AddSystem(new ObjectFocusSystem(this))
                .AddSystem(new LightSystem(this))
                .Build();

            EntityFactory = new Entities.EntityFactory(this);
        }

        public HashSet<Type> DisabledWorldSystems = new();
        public HashSet<Type> RegisteredSystems = new();


        public void DisableSystems(params Type[] types)
        {
            types.ForEach(x => DisabledWorldSystems.Add(x));
        }

        public void DisableMouseSystems()
        {
            Game.DisableSystems(typeof(PlayerControllSystem), typeof(ObjectFocusSystem), typeof(MouseMapObjectFocusSystem));
        }

        public void DisableSystemsExcept(params Type[] types)
        {
            var exept = RegisteredSystems.Except(types);
            DisabledWorldSystems = exept.ToHashSet();
        }

        public void EnableSystems()
        {
            DisabledWorldSystems.Clear();
        }

        private class WorldBuilderProxy
        {
            private readonly WorldBuilder _worldBuilder;
            private readonly NabunassarGame _game;

            public WorldBuilderProxy(NabunassarGame game, WorldBuilder worldBuilder=default)
            {
                _game = game;
                _worldBuilder = worldBuilder ?? new WorldBuilder();
            }

            public WorldBuilderProxy AddSystem(BaseSystem system)
            {
                _game.RegisteredSystems.Add(system.GetType());
                _worldBuilder.AddSystem(system);
                return this;
            }

            public World Build() => _worldBuilder.Build();
        }
    }    
}
