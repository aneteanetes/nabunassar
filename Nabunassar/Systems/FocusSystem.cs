using Geranium.Reflection;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Game;

namespace Nabunassar.Systems
{
    internal class FocusSystem : BaseSystem
    {
        private ComponentMapper<FocusComponent> focusMapper;

        public FocusSystem(NabunassarGame game) : base(game, Aspect.One(typeof(FocusComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            focusMapper = mapperService.GetMapper<FocusComponent>();
        }

        public override void Update(GameTime gameTime, bool isSystem)
        {
            if (Cursor.FocusEvents.IsEmpty())
                return;

            List<FocusComponent> focusComponents = new();
            foreach (var entity in ActiveEntities)
            {
                var focus = focusMapper.Get(entity);
                if(focus != null)
                    focusComponents.Add(focus);
            }

            while (Cursor.FocusEvents.IsNotEmpty())
            {
                var @event = Cursor.FocusEvents.Dequeue();
                foreach (var focusComponent in focusComponents)
                {
                    if (@event.IsFocused)
                        focusComponent.OnFocus?.Invoke(@event.Object);
                    else
                        focusComponent.OnUnfocus?.Invoke(@event.Object);
                }
            }
        }
    }
}