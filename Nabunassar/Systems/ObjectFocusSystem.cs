using Geranium.Reflection;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Game;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Systems
{
    internal class ObjectFocusSystem : BaseSystem
    {
        private ComponentMapper<FocusComponent> focusMapper;
        private ComponentMapper<FocusWidgetComponent> widgetMapper;

        public ObjectFocusSystem(NabunassarGame game) : base(game, Aspect.One(typeof(FocusComponent),typeof(FocusWidgetComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            focusMapper = mapperService.GetMapper<FocusComponent>();
            widgetMapper = mapperService.GetMapper<FocusWidgetComponent>();
        }

        public override void Update(GameTime gameTime, bool isSystem)
        {
            if (Cursor.FocusEvents.IsEmpty())
                return;

            List<FocusComponent> focusComponents = new();
            List<FocusWidgetComponent> widgetComponents = new();
            foreach (var entity in ActiveEntities)
            {
                var focus = focusMapper.Get(entity);
                if(focus != null)
                    focusComponents.Add(focus);

                //var widget = widgetMapper.Get(entity);
                //if(widget != null)
                //    widgetComponents.Add(widget);
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

                //var objectWidgets = widgetComponents.Where(x => x.GameObject == @event.Object);
                //foreach (var objectWidget in objectWidgets)
                //{
                //    if (@event.IsFocused)
                //    {
                //        var screenWidget = objectWidget.WidgetFactory?.Invoke(@event);
                //        objectWidget.CurrentScreenWidget = screenWidget;
                //        Game.AddDesktopWidget(screenWidget);
                //    }
                //    else
                //    {
                //        Game.RemoveDesktopWidgets<TitleWidget>();
                //    }
                //}
            }
        }
    }
}