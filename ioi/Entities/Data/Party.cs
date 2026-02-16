using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using ioi.Components;
using ioi.Entities.Data.Abilities.WorldAbilities;
using ioi.Entities.Data.Affects;
using ioi.Entities.Game;
using ioi.Entities.Struct.FixedCollections;
using ioi.Entities.Struct.FixedCollections.Octas;
using ioi.Entities.Struct.FixedCollections.Quads;
using ioi.Struct;
using ioi.Widgets.UserInterfaces.ContextMenus.Radial;
using ioi.Widgets.UserInterfaces.GameWindows.Informations;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace ioi.Entities.Data
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

        private GameHost _game;

        public Party(GameHost game)
        {
            _game = game;
        }

        public Inventory Inventory { get; set; } = new();

        public void OnCollision(CollisionEventArgs collisionInfo, MapObject host, MapObject other)
        {
            if (other.ObjectType == ObjectType.Creature)
                GameController.StartCombat(other.GameObject, Side.Right);
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
            SetPositionOn(Third, QuadPosition.Third);
            SetPositionOn(Fourth, QuadPosition.Fourth);
        }

        internal void SetPositionOnAllPartyRight()
        {
            SetPositionOn(First, QuadPosition.Fourth);
            SetPositionOn(Second, QuadPosition.Third);
            SetPositionOn(Third, QuadPosition.Second);
            SetPositionOn(Fourth, QuadPosition.First);
        }

        internal void ChangeDirection(Direction direction)
        {
            var gameobject = First.MapObject;

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
                    hero.MapObject.SetAbsolutePosition(-4,y);
                    break;
                case QuadPosition.Second:
                    hero.MapObject.SetAbsolutePosition(4, y);
                    break;
                case QuadPosition.Third:
                    hero.MapObject.SetAbsolutePosition(12, y);
                    break;
                case QuadPosition.Fourth:
                    hero.MapObject.SetAbsolutePosition(20, y);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toWorldCoords">Screen coords</param>
        /// <param name="gameObject"></param>
        /// <param name="mouseWorldPosition"></param>
        public void MoveTo(Vector2 toWorldCoords, GameObject gameObject=null, Vector2 mouseWorldPosition=default)
        {
            if (gameObject == null)
            {
                MoveParty(toWorldCoords);
            }
            else
            { 
                if (IsObjectNear(gameObject))
                {
                    Interact(gameObject, mouseWorldPosition);
                }
                else
                {
                    MoveParty(toWorldCoords);

                    MapObject.BoundsTries = 15;

                    ActionQueue.Enqueue(() =>
                    {
                        Interact(gameObject, mouseWorldPosition);
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

        public Vector2 GetOrigin()
        {
            return this.MapObject.BoundsOrigin;
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
                    if (IsObjectNear(gameObject))
                        LootWindow.Open(_game, gameObject);
                    break;
                case ObjectType.Creature:
                    if (IsObjectNear(gameObject))
                    {
                        GameController.StartCombat(gameObject, Side.Left);
                    }
                    break;
                default:
                    if (gameObject.ObjectType.IsInteractive() && IsObjectNear(gameObject))
                        InformationWindow.Open(GameHost.Game, gameObject);
                    break;
            }
        }

        public RectangleF DistanceMeterRectangle => this.MapObject.Bounds.BoundingRectangle.Multiple(3).MultipleY(2f);

        public RectangleF Bounds => this.MapObject.Bounds.BoundingRectangle;

        public RectangleF RevealArea => DistanceMeterRectangle;

        public RectangleF PartyMenuRectangle
        {
            get
            {
                var bounds = this.MapObject.Bounds.As<RectangleF>();
                var height = 10;
                var width = 14;
                return new RectangleF(bounds.X-7, bounds.Y-4, bounds.Width+ width, bounds.Height+ height);
            }
        }

        public int Weight { get; internal set; } = 95;

        public Result<bool> IsObjectNear(GameObject gameObject)
        {
            if (gameObject == null)
                return new Result<bool>(false,GameHost.Game.Strings["GameTexts"]["NoTarget"]);


            return DistanceMeterRectangle.Intersects(gameObject.MapObject.Bounds.BoundingRectangle);
        }

        public bool Visible
        {
            set
            {
                foreach (var hero in this)
                {
                    hero.Entity.Get<RenderComponent>().IsVisible = value;
                }
            }
        }

        public void SetPosition(Vector2 position)
        {
            this.MapObject.SetPosition(position);
        }

        public Vector2 GetPosition()
        {
            return this.MapObject.Position;
        }

        public void SpeakTo(GameObject gameObject, Vector2 talkingTargetPosition=default)
        {
            if (gameObject.MapObject != null)
            {
                if (DistanceMeterRectangle.Intersects(gameObject.MapObject.Bounds.BoundingRectangle))
                {
                    var direction = MapObject.Position.DetectDirection(gameObject.MapObject.Position)
                        .ToLeftRight();

                    ChangeViewDirection(direction);
                    gameObject.MapObject.ViewDirection = direction.Opposite();

                    _game.WidgetFactory.OpenDialogue(gameObject);
                }
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

        internal Octa<Creature> ToSquad()
        {
#warning todo party formation
            return new Octa<Creature>()
            {
                First = this.First.Creature,
                Second = this.Second.Creature,
                Third = this.Third.Creature,
                Fourth = this.Fourth.Creature
            };
        }
    }
}