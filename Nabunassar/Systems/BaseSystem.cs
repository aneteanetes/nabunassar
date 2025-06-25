using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;

namespace Nabunassar.Systems
{
    internal abstract class BaseSystem : EntitySystem, IDrawSystem, IUpdateSystem
    {
        protected NabunassarGame Game { get; private set; }

        public BaseSystem(NabunassarGame game, AspectBuilder aspect) : base(aspect)
        {
            Game = game;
        }

        public override void Initialize(IComponentMapperService mapperService) { }


        public void Draw(GameTime gameTime)
        {
            if (Game.DisabledWorldSystems.Contains(this.GetType()))
                return;

            Draw(gameTime, true);
        }

        /// <summary>
        /// Call first for auto-disabling
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (Game.DisabledWorldSystems.Contains(this.GetType()))
                return;

            Update(gameTime, true);
        }

        public virtual void Update(GameTime gameTime, bool isSystem) { }

        public virtual void Draw(GameTime gameTime, bool isSystem) { }
    }
}
