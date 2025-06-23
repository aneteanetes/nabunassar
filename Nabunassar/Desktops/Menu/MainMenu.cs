using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Screens.Game;

namespace Nabunassar.Desktops.Menu;

internal partial class MainMenu : ScreenWidget
{
    public MainMenu(NabunassarGame game) : base(game)
    {
    }

    private FontSystem _font;
    private Texture2D backimgnorm;
    private Texture2D backimgfocus;

    protected override void LoadContent()
    {
        _font = Game.Content.LoadFont("Assets/Fonts/Retron2000.ttf");
        backimgnorm = Game.Content.Load<Texture2D>("Assets/Images/Borders/commonborder.png");
        backimgfocus = Game.Content.Load<Texture2D>("Assets/Images/Borders/commonborderpressed.png");
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAssets(["Assets/Images/Borders/commonborder.png", "Assets/Images/Borders/commonborderpressed.png"]);
        base.UnloadContent();
    }

    protected override Widget InitWidget()
    {
        var panel = new Panel();

        var backNormal = new NinePatchRegion(backimgnorm, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));
        var backPressed = new NinePatchRegion(backimgfocus, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));

        var newGame = new Button()
        {
            Width = 200,
            Height = 50,
            Background = backNormal,
            OverBackground = backNormal,
            PressedBackground = backPressed,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        newGame.MouseEntered += NewGame_MouseEntered;
        newGame.MouseLeft += NewGame_MouseLeft;
        var newgametext = new Label()
        {
            Text = "Новая игра",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(Color.Transparent),
            Font = _font.GetFont(28),
            TextColor = Globals.CommonColor,
        };
        newGame.Click += NewGame_Click;
        newGame.Content = newgametext;
        newGame.Top = -300;

        var load = newGame.Clone().As<Button>();
        var loadtext = newgametext.Clone().As<Label>();
        loadtext.Text = "Загрузить";
        load.Top = -225;
        load.Content = loadtext;
        load.MouseEntered += NewGame_MouseEntered;
        load.MouseLeft += NewGame_MouseLeft;

        var quit = newGame.Clone().As<Button>();
        var quittext = newgametext.Clone().As<Label>();
        quittext.Text = "Выйти";
        quit.Content = quittext;
        quit.Top = -150;
        quit.Click += _menuQuit_Selected;
        quit.MouseEntered += NewGame_MouseEntered;
        quit.MouseLeft += NewGame_MouseLeft;

        panel.Widgets.Add(newGame);
        panel.Widgets.Add(load);
        panel.Widgets.Add(quit);

        return panel;
    }

    private void NewGame_Click(object sender, EventArgs e)
    {
        Game.SwitchScreen<MainGameScreen>();
    }

    private void NewGame_MouseLeft(object sender, EventArgs e)
    {
        sender.As<Button>().Content.As<Label>().TextColor = Globals.CommonColor;
    }

    private void NewGame_MouseEntered(object sender, EventArgs e)
    {
        sender.As<Button>().Content.As<Label>().TextColor = Globals.CommonColorLight;
    }

    private void _menuQuit_Selected(object sender, EventArgs e)
    {
        Game.Exit();
    }


    //public void LoadContent() { }

    //public void UnloadContent()
    //{
    //    _game.Content.Load<Texture2D>("Assets/Images/Borders/commonborder.png");
    //    _game.Content.Load<Texture2D>("Assets/Images/Borders/commonborderpressed.png");
    //}
}