using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using ioi.Components;
using ioi.Entities.Game;
using ioi.Entities.Struct.FixedCollections.Octas;
using ioi.Screens.Abstract;
using ioi.Struct;
using ioi.Widgets.UserInterfaces;
using ioi.Widgets.UserInterfaces.Combat.SquadView;

namespace ioi.Screens.Game
{
    internal class CombatGameScreen : BaseGameScreen
    {
        private Texture2D _background;
        private Octa<Creature> _playerSquad;
        private Octa<Creature> _enemySquad;
        private Encounter _encounter;
        private Side _playerSide;
        private Entity playerSquadEntity;
        private SquadInBattle _playerSquadComponent;
        private SquadInBattle _encounterSquadComponent;

        public CombatGameScreen(GameHost game, GameObject gameobjectWithEncounter, Side playerSize) : base(game)
        {
            _encounter = gameobjectWithEncounter.Encounter;
            _playerSide = playerSize;
        }

        public override void LoadContent()
        {
            GameController.SetCameraToWorld();

            Game.InitBattleWorld();

            _background = Content.LoadTexture("Assets/Images/Backgrounds/Combat/underdead3");

            _playerSquad = Game.GameState.Party.ToSquad();
            _enemySquad = _encounter.Creatures;

            InitBattlers();
            InitGameUI();

            base.LoadContent();
        }

        public void InitBattlers()
        {
            playerSquadEntity = Game.EntityFactoryBattle.CreateEntity("player squad in map");
            playerSquadEntity.Attach(_playerSquadComponent = new SquadInBattle("player squad",_playerSquad, _playerSide)
            {
                IsPlayerSquad = true,
            });

            foreach (var hero in Game.GameState.Party)
            {
                if (hero.Sprite.CurrentAnimation != "idle")
                    hero.Sprite.SetAnimation("idle");

                if (_playerSide == Side.Right)
                    hero.Sprite.Effect = SpriteEffects.FlipHorizontally;
            }

            _encounter.Entity = Game.EntityFactoryBattle.CreateEntity("enemy squad in map");
            _encounter.Entity.Attach(_encounterSquadComponent = new SquadInBattle("enemy squad", _enemySquad, _playerSide.Opposite()));
            Game.EntityFactoryBattle.FullfillEncounter(_encounter);
        }

        public void InitGameUI()
        {
            Game.AddDesktopWidget(new ControlPanel(Game,false));
            Game.AddDesktopWidget(new GameDateTime(Game));
            Game.AddDesktopWidget(new PartyConditions(Game));

            var leftFramePos = new Vector2(0, 130);
            var rightFramePos = new Vector2(Game.MainViewport.Width - SquadPlate.SquadPlateWidth, 130);

            Game.AddDesktopWidget(new SquadPlate(Game, _playerSquadComponent, _playerSide)
            {
                Position = _playerSide == Side.Left ? leftFramePos : rightFramePos
            });
            Game.AddDesktopWidget(new SquadPlate(Game, _encounterSquadComponent, _playerSide.Opposite())
            {
                Position = _playerSide == Side.Left ? rightFramePos: leftFramePos
            });
        }

        public override void Update(GameTime gameTime)
        {
            GameController.GlobalMenuWidget();

            if (!Game.IsGameActive)
                return;

            Game.OldECSBattle?.Update(gameTime);
        }

        protected override void DrawInternal(GameTime gameTime)
        {
            //var sb = Game.BeginDraw();
            //sb.Draw(_background, new Rectangle(Point.Zero, (Game.MainViewport.ToVector2() / Game.CameraMain.Zoom).ToPoint()), Color.White);
            //sb.End();

            Game.OldECSBattle?.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        public override void Dispose()
        {
            Game.DisposeBattleWorld();
        }
    }
}