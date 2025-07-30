using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Affects;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Informations;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace Nabunassar.Entities.Data
{
    internal class Party : Quad<Hero>, IDistanceMeter
    {
        public ActionQueue ActionQueue { get; set; } = new();

        public Entity Entity { get; set; }

        public Money Money { get; set; } = new Money(0,0,0);

        public MapObject MapObject { get; set; }

        public GameObject GameObject => new GameObject()
        {
            MapObject = MapObject,
            Entity = Entity,
            ObjectType = ObjectType.Player            
        };

        public Direction ViewDirection { get; set; } = Direction.Right;

        public RenderComponent DirectionRender { get; internal set; }

        private NabunassarGame _game;

        public Party(NabunassarGame game)
        {
            _game = game;
            First = new Hero(game);
            Second = new Hero(game);
            Third = new Hero(game);
            Fourth = new Hero(game);
        }

        public Inventory Inventory { get; set; } = new();

        public void OnCollision(CollisionEventArgs collisionInfo, MapObject host, MapObject other)
        {

        }

        public void Select(QuadPosition position)
        {
            _game.GameState.Log($"Select hero at {position} position");

            var current = this[position];
                while (First != current)
                    Rotate();
        }

        public void Rotate()
        {
            _game.GameState.Log($"Rotate party");

            var first = First;
            var firstOrder = first.Order;

            var second = Second;
            var secondOrder = second.Order;

            var third = Third;
            var thirdOrder = third.Order;

            var fourth = Fourth;
            var fourthOrder = fourth.Order;

            First = second;
            First.Order = firstOrder;

            Second = third;
            Second.Order = secondOrder;

            Third = fourth;
            Third.Order = thirdOrder;

            Fourth = first;
            Fourth.Order = fourthOrder;

            if (ViewDirection == Direction.Left)
            {
                SetPositionOnAllPartyLeft();
            }

            if (ViewDirection == Direction.Right)
            {
                SetPositionOnAllPartyRight();
            }
        }

        internal void SetPositionOnAllPartyLeft()
        {
            SetPositionOn(First, QuadPosition.First);
            SetPositionOn(Second, QuadPosition.Second);
            SetPositionOn(Third, QuadPosition.Thrid);
            SetPositionOn(Fourth, QuadPosition.Fourth);
        }

        internal void SetPositionOnAllPartyRight()
        {
            SetPositionOn(First, QuadPosition.Fourth);
            SetPositionOn(Second, QuadPosition.Thrid);
            SetPositionOn(Third, QuadPosition.Second);
            SetPositionOn(Fourth, QuadPosition.First);
        }

        internal void ChangeDirection(Direction direction)
        {
            var gameobject = First.GameObject;

            if (!gameobject.IsMoving)
            {
                if (ViewDirection != direction)
                {
                    ViewDirection = direction;

                    if (ViewDirection == Direction.Left)
                    {
                        SetPositionOnAllPartyLeft();
                    }

                    if (ViewDirection == Direction.Right)
                    {
                        SetPositionOnAllPartyRight();
                    }
                }
            }
        }

        private void SetPositionOn(Hero hero, QuadPosition position)
        {
            var y = -12;
            switch (position)
            {
                case QuadPosition.First:
                    hero.GameObject.SetAbsolutePosition(-4,y);
                    break;
                case QuadPosition.Second:
                    hero.GameObject.SetAbsolutePosition(4, y);
                    break;
                case QuadPosition.Thrid:
                    hero.GameObject.SetAbsolutePosition(12, y);
                    break;
                case QuadPosition.Fourth:
                    hero.GameObject.SetAbsolutePosition(20, y);
                    break;
                default:
                    break;
            }
        }

        public void MoveTo(Vector2 to, GameObject gameObject=null, Vector2 mouseScreenPosition=default)
        {
            if (gameObject == null)
            {
                MoveParty(to);
            }
            else
            { 
                if (IsObjectNear(gameObject))
                {
                    Interact(gameObject, mouseScreenPosition);
                }
                else
                {
                    MoveParty(to);

                    MapObject.BoundsTries = 15;

                    ActionQueue.Enqueue(() =>
                    {
                        Interact(gameObject, mouseScreenPosition);
                    });
                }
            }
        }

        private void MoveParty(Vector2 to)
        {
            MapObject.BoundsTries = 75;

            MapObject.MoveToPosition(MapObject.BoundsOrigin, to);

            DirectionRender.Sprite.IsVisible = true;
            DirectionRender.Position = to;
        }

        public void Interact(GameObject gameObject, Vector2 mouseScreenPosition)
        {
            switch (gameObject.ObjectType)
            {
                case ObjectType.Object:
                    RadialMenu.Open(this._game, gameObject, mouseScreenPosition);
                    break;
                case ObjectType.NPC:
                    SpeakTo(gameObject);
                    break;
                case ObjectType.Container:
                    ScreenWidgetWindow.Open(new ItemContainerWindow(_game, gameObject));
                    break;
                default:
                    InformationWindow.Open(NabunassarGame.Game, gameObject);
                    break;
            }
        }


        public RectangleF DistanceMeterRectangle => this.MapObject.Bounds.BoundingRectangle.Multiple(3);

        public int Weight { get; internal set; } = 95;

        public Result<bool> IsObjectNear(GameObject gameObject)
        {
            if (gameObject == null)
                return new Result<bool>(false,NabunassarGame.Game.Strings["GameTexts"]["NoTarget"]);


            return DistanceMeterRectangle.Intersects(gameObject.MapObject.Bounds.BoundingRectangle);
        }

        public void SpeakTo(GameObject gameObject, Vector2 talkingTargetPosition=default)
        {
            if (gameObject.MapObject != null)
            {
                var visualBounds = this.MapObject.Bounds.BoundingRectangle.Multiple(2);

                if (visualBounds.Intersects(gameObject.MapObject.Bounds.BoundingRectangle))
                {
                    var direction = MapObject.Position.DetectDirection(gameObject.MapObject.Position)
                        .ToLeftRight();

                    ChangeViewDirection(direction);
                    gameObject.MapObject.ViewDirection = direction.Opposite();

                    _game.WidgetFactory.OpenDialogue(gameObject);
                }
                //else
                //{
                //    MoveTo(talkingTargetPosition, gameObject);
                //}
            }
        }

        public void ChangeViewDirection(Direction direction)
        {
            var dir = direction.ToLeftRight();

            foreach (var hero in this)
            {
                var render = hero.Entity.Get<RenderComponent>();
                render.Sprite.Effect = dir == Direction.Left ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally : Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            }
        }

        public void OnStopMoving()
        {
            foreach (var hero in this)
            {
                var animatedSprite = hero.Entity.Get<AnimatedSprite>();
                if (animatedSprite.CurrentAnimation != "idle")
                {
                    animatedSprite.SetAnimation("idle");
                }
            }

            this.MapObject.BoundsTries = 75;

            Action dequeued = ActionQueue.Dequeue(); 
            dequeued?.Invoke();

            while (dequeued != default)
            {
                dequeued = ActionQueue.Dequeue();
                dequeued?.Invoke();
                this.MapObject.BoundsTries = 50;
            }
        }

        public List<BaseWorldAbility> GetWorldAbilities(GameObject gameObject)
        {
            List<BaseWorldAbility> worldAbilities = new();

            foreach (var hero in this)
            {
                if (hero.Creature != default)
                {
                    foreach (var abil in hero.Creature.WorldAbilities)
                    {
                        if (abil != null && abil.IsApplicable(gameObject))
                        {
                            worldAbilities.Add(abil);
                        }
                    }
                }
            }

            return worldAbilities;
        }
    }
}