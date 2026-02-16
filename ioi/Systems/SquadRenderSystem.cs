using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using ioi.Components;
using ioi.Entities.Data;
using ioi.Entities.Game;
using ioi.Entities.Struct.FixedCollections.Octas;
using ioi.Monogame.SpriteBatch;
using ioi.Struct;
using ioi.Widgets.UserInterfaces.Combat.SquadView;

namespace ioi.Systems
{
    internal class SquadRenderSystem : BaseSystem
    {
        private ComponentMapper<SquadInBattle> _squadMapper;
        private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;

        public SquadRenderSystem(GameHost game) : base(game, Aspect.All(typeof(SquadInBattle)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _squadMapper = mapperService.GetMapper<SquadInBattle>();
            _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
        }

        public override void Update(GameTime gameTime, bool isSystem)
        {
            foreach (var squadEntityId in ActiveEntities)
            {
                var squadInMap = _squadMapper.Get(squadEntityId);
                foreach (var battler in squadInMap.Squad)
                {
                    if (battler == null)
                        continue;

                    var animatedSprite = GetAnimatedSprite(squadInMap, battler);
                    animatedSprite.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime, bool isSystem)
        {
            var sb = Game.BeginDraw();
            foreach (var squadEntityId in ActiveEntities)
            {
                var squad = _squadMapper.Get(squadEntityId);
                RenderSquad(squad, sb);
            }
            sb.End();
        }

        private Vector2 SlotSize = new Vector2(160, 80);

        private Vector2 LeftSixPos = new Vector2(340, 440);
        private Vector2 RightSixPos = new Vector2(875, 795);

        private Vector2 LeftFifthPos = new Vector2(635, 360);
        private Vector2 RightFifthPos = new Vector2(1110, 715);

        private Vector2 LeftFourthPos = new Vector2(875, 275);
        private Vector2 RightFourthPos = new Vector2(1355, 635);

        private Vector2 LeftThirdPos = new Vector2(555, 560);
        private Vector2 RightThirdPos = new Vector2(695, 675);

        private Vector2 LeftSecondPos = new Vector2(795, 480);
        private Vector2 RightSecondPos = new Vector2(950, 600);

        private Vector2 LeftFirstPos = new Vector2(1035, 400);
        private Vector2 RightFirstPos = new Vector2(1200, 520);

        private Vector2 GetPositionByContext(OctaPosition octaPosition, Side side)
        {
            var context = PosSide(octaPosition,side);

            // no match on calculated values

            if (PosSide(OctaPosition.Sixth, Side.Left) == context)
            {
                return LeftSixPos;
            }
            if (PosSide(OctaPosition.Sixth, Side.Right) == context)
            {
                return RightSixPos;
            }

            if (PosSide(OctaPosition.Fifth, Side.Left) == context)
            {
                return LeftFifthPos;
            }
            if (PosSide(OctaPosition.Fifth, Side.Right) == context)
            {
                return RightFifthPos;
            }

            if (PosSide(OctaPosition.Fourth, Side.Left) == context)
            {
                return LeftFourthPos;
            }
            if (PosSide(OctaPosition.Fourth, Side.Right) == context)
            {
                return RightFourthPos;
            }

            if (PosSide(OctaPosition.Third, Side.Left) == context)
            {
                return LeftThirdPos;
            }
            if (PosSide(OctaPosition.Third, Side.Right) == context)
            {
                return RightThirdPos;
            }

            if (PosSide(OctaPosition.Second, Side.Left) == context)
            {
                return LeftSecondPos;
            }
            if (PosSide(OctaPosition.Second, Side.Right) == context)
            {
                return RightSecondPos;
            }

            if (PosSide(OctaPosition.First, Side.Left) == context)
            {
                return LeftFirstPos;
            }
            if (PosSide(OctaPosition.First, Side.Right) == context)
            {
                return RightFirstPos;
            }

            return Vector2.Zero;
        }

        private string PosSide(OctaPosition octaPosition, Side side) => $"{octaPosition}{side}";

        private void RenderSquad(SquadInBattle squadInMap, SpriteBatchKnowed sb)
        {
            var side = squadInMap.Side;

            var octa = squadInMap.Squad;
            var battlers = squadInMap.Squad.ToList();

            if (side == Side.Right)
                battlers.Reverse();

            foreach (var battler in battlers)
            {
                if (battler == null)
                    continue;

                var animatedSprite = GetAnimatedSprite(squadInMap, battler);

                var octaPos = octa.GetOctaPosition(battler);
                var slot = GetPositionByContext(octaPos, side);

                var slotScaled= Game.CameraMain.ScreenToWorld(slot);

                var pos = new Vector2((slot.X + (SlotSize.X / 2)), (slot.Y + (SlotSize.Y / 2)));
                pos = Game.CameraMain.ScreenToWorld(pos);

                pos = new Vector2(pos.X - (animatedSprite.TextureRegion.Width / 2), pos.Y - (animatedSprite.TextureRegion.Height / 2));

                sb.Draw(animatedSprite, pos, 0, Vector2.One);

                if (Game.IsDrawBounds)
                {
                    sb.DrawRectangle(new RectangleF()
                    {
                        X = pos.X,
                        Y = pos.Y,
                        Width = animatedSprite.TextureRegion.Width,
                        Height = animatedSprite.TextureRegion.Height
                    }, Color.MediumVioletRed);

                    var slotSizeScaled = Game.CameraMain.ScreenToWorld(SlotSize);
                    sb.DrawRectangle(new RectangleF()
                    {
                        X = slotScaled.X,
                        Y = slotScaled.Y,
                        Width = slotSizeScaled.X,
                        Height = slotSizeScaled.Y
                    }, squadInMap.IsPlayerSquad ? Color.DarkRed : Color.GreenYellow);
                }
            }
        }

        private AnimatedSprite GetAnimatedSprite(SquadInBattle squadInMap, Creature creature) 
            => squadInMap.IsPlayerSquad
                ? creature.HeroLink.Sprite
                : _animatedSpriteMapper.Get(creature.Entity);
    }
}