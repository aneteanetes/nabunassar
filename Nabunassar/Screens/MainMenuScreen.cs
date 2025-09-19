using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Screens.Abstract;
using Nabunassar.Widgets.Menu;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.MonoGame.Sprites;
using SpriterDotNet.Providers;

namespace Nabunassar.Screens
{
    internal class MainMenuScreen : BaseScreen
    {
        private readonly List<MonoGameAnimator> animators = new();
        private MonoGameAnimator animator;
        public static readonly Config SpriterConfig = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = true,
            VarsEnabled = true,
            SoundsEnabled = false
        };
        Spriter spriter;

        private bool _isAnimationVisible = false;



        private Texture2D background;

        public MainMenuScreen(NabunassarGame game) : base(game) { }

        public override void LoadContent()
        {
            Game.Content.LoadFont(Fonts.Retron);
            background = Game.Content.Load<Texture2D>("Assets/Images/Backgrounds/logo.png");
            Game.AddDesktopWidget(new MainMenu(Game));
            Game.InitGameWorld();
            Game.InitializeGameState();
            Game.IsGameActive = true;

            LoadContentSpriter();
        }

        private void LoadContentSpriter()
        {
            var sdata = Content.Load<string>("Assets/Sprites/Creatures/Wrath3/Animations.scml");
            spriter = SpriterReader.Default.Read(sdata);

            DefaultProviderFactory<ISprite, SoundEffect> factory = new DefaultProviderFactory<ISprite, SoundEffect>(SpriterConfig, true);
            if (spriter != null)
            {
                foreach (SpriterFolder folder in spriter.Folders)
                {
                    if (spriter.Atlases != null && spriter.Atlases.Length > 0)
                    {
                        // AddAtlasFolder(folder, factory, spriter);  // I don't use SpriteAtlas so you have to write your own function here
                    }
                    else
                    {
                        AddRegularFolder(folder, factory, spriter);
                    }
                }
            }


            Stack<SpriteDrawInfo> drawInfoPool = new Stack<SpriteDrawInfo>();

            foreach (SpriterEntity entity in spriter.Entities)
            {
                var animator = new MonoGameAnimator(entity, factory, drawInfoPool);
                animators.Add(animator);
                animator.Position = new Vector2(100, 400);
            }

            animator = animators.First();
            animator.Play("Idle");
            animator.Position = new Vector2(400, 400);
        }

        private void AddRegularFolder(SpriterFolder folder, DefaultProviderFactory<ISprite, SoundEffect> factory, Spriter spriter)
        {
            foreach (SpriterFile file in folder.Files)
            {
                if (file.Type == SpriterFileType.Sound)
                {
                }
                else
                {
                    Texture2D texture = Content.LoadTexture("Assets/Sprites/Creatures/Wrath3/" + file.Name);
                    TextureSprite sprite = new TextureSprite(texture);
                    //file.Name = file.Name.Substring(0, file.Name.LastIndexOf(".") - 1);
                    factory.SetSprite(spriter, folder, file, sprite);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            sb.Draw(background, Game.Resolution, new Rectangle(0, 0, 2560, 1440), Color.White);
            sb.End();

            if (_isAnimationVisible)
            {
                sb = Game.BeginDraw();
                animator.Draw(sb);
                sb.End();
            }

            base.Draw(gameTime);
        }

        public override void UnloadContent()
        {
            Game.RemoveDesktopWidgets(true);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerMillisecond;
            animator.Update(deltaTime);
            
            SkeletonAnimationTest();
        }

        private void SkeletonAnimationTest()
        {
            var keyboard = KeyboardExtended.GetState();

            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V))
            {
                _isAnimationVisible = !_isAnimationVisible;
            }

            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                animator.Position = new Vector2(animator.Position.X - .1f, animator.Position.Y);
            }
            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                animator.Position = new Vector2(animator.Position.X + .1f, animator.Position.Y);
            }
            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                animator.Position = new Vector2(animator.Position.X, animator.Position.Y - .1f);
            }
            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                animator.Position = new Vector2(animator.Position.X, animator.Position.Y + .1f);
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D1))
            {
                animator.Play("Idle");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D2))
            {
                animator.Play("Idle Blinking");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D3))
            {
                animator.Play("Taunt");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D4))
            {
                animator.Play("Moving Forward");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D5))
            {
                animator.Play("Attack");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D6))
            {
                animator.Play("Casting Spells");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D7))
            {
                animator.Play("Hurt");
            }

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.D8))
            {
                animator.Play("Dying");
            }
        }
    }
}