using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using Myra.Events;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Screens;
using Nabunassar.Screens.Game;
using Nabunassar.Shaders;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.Menu;

internal partial class MainMenu : ScreenWidget
{
    bool _isInGame;

    public override void BindWidgetBlockMouse(Widget widget, bool withDispose = true, bool twoSideBlock = false)
    {
        if (_isInGame)
            base.BindWidgetBlockMouse(widget, withDispose);
    }

    public MainMenu(NabunassarGame game, bool isInGame = false) : base(game)
    {
        _isInGame = isInGame;
    }

    public override bool IsModal => true;

    private FontSystem _font;
    private Texture2D backimgnorm;
    private Texture2D backimgfocus;

    public override void LoadContent()
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

    protected override Widget CreateWidget()
    {
        var container = new Panel();
        container.HorizontalAlignment = HorizontalAlignment.Stretch;
        container.VerticalAlignment = VerticalAlignment.Stretch;

        var panel = new VerticalStackPanel();
        panel.Spacing = 25;
        panel.Margin = new Myra.Graphics2D.Thickness(0, 0, 0, 15);
        panel.HorizontalAlignment = HorizontalAlignment.Center;
        panel.VerticalAlignment = VerticalAlignment.Bottom;

        var backNormal = new NinePatchRegion(backimgnorm, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));
        var backPressed = new NinePatchRegion(backimgfocus, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));

        //var top = -300;

        var newGame = new Button()
        {
            Width = 200,
            Height = 50,
            Background = backNormal,
            OverBackground = backNormal,
            PressedBackground = backPressed
        };

        newGame.MouseEntered += NewGame_MouseEntered;
        newGame.MouseLeft += NewGame_MouseLeft;
        var newgametext = new Label()
        {
            Text = Game.Strings["UI"]["NewGame"],
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(Color.Transparent),
            Font = _font.GetFont(28),
            TextColor = Globals.CommonColor,
        };
        newGame.Click += NewGame_Click;
        newGame.Content = newgametext;
        //newGame.Top = top;

        if (_isInGame)
        {
            var back = newGame.Clone().As<Button>();
            var backtext = newgametext.Clone().As<Label>();
            backtext.Text = Game.Strings["UI"]["Back"];
            back.Content = backtext;
            //back.Top = top += 75;
            back.Click += Back_Click;
            back.MouseEntered += NewGame_MouseEntered;
            back.MouseLeft += NewGame_MouseLeft;
            panel.Widgets.Add(back);
        }
        else
        {
            panel.Widgets.Add(newGame);
        }

        var load = newGame.Clone().As<Button>();
        var loadtext = newgametext.Clone().As<Label>();
        loadtext.Text = Game.Strings["UI"]["Load"];
        //load.Top = top+=75;
        load.Content = loadtext;
        load.MouseEntered += NewGame_MouseEntered;
        load.MouseLeft += NewGame_MouseLeft;
        panel.Widgets.Add(load);

        if (_isInGame)
        {
            var backtoMenu = newGame.Clone().As<Button>();
            var backtoMenutext = newgametext.Clone().As<Label>();
            backtoMenutext.Text = Game.Strings["UI"]["Back to main menu"];
            backtoMenu.Content = backtoMenutext;
            //quit.Top = top += 75;
            backtoMenu.Click += BacktoMenu_Click; ;
            backtoMenu.MouseEntered += NewGame_MouseEntered;
            backtoMenu.MouseLeft += NewGame_MouseLeft;
            panel.Widgets.Add(backtoMenu);
        }

        var quit = newGame.Clone().As<Button>();
        var quittext = newgametext.Clone().As<Label>();
        quittext.Text = Game.Strings["UI"]["Exit"];
        quit.Content = quittext;
        //quit.Top = top += 75;
        quit.Click += _menuQuit_Selected;
        quit.MouseEntered += NewGame_MouseEntered;
        quit.MouseLeft += NewGame_MouseLeft;
        panel.Widgets.Add(quit);

        if (_isInGame)
            container.IsModal = true;

        container.Widgets.Add(panel);


        Grid.SetRow(newGame, 0);
        Grid.SetRow(load, 1);
        Grid.SetRow(quit, 2);

        return container;
    }

    private void BacktoMenu_Click(object sender, MyraEventArgs e)
    {
        Game.GameState.InGame = false;
        Game.SwitchScreen<MainMenuScreen>(GameController.UnloadGame());
    }

    public override void OnAfterAddedWidget(Widget widget)
    {
        widget.BringToFront();
    }

    private void Back_Click(object sender, MyraEventArgs e)
    {
        MainGameScreen.GlobalBlurShader.Disable();
        Game.RemoveDesktopWidgets<MainMenu>();
        Game.ChangeGameActive();
    }

    private void NewGame_Click(object sender, MyraEventArgs e)
    {
        GameController.StartNewGame(Game);
    }

    private void NewGame_MouseLeft(object sender, MyraEventArgs e)
    {
        sender.As<Button>().Content.As<Label>().TextColor = Globals.CommonColor;
    }

    private void NewGame_MouseEntered(object sender, MyraEventArgs e)
    {
        sender.As<Button>().Content.As<Label>().TextColor = Globals.CommonColorLight;
    }

    private void _menuQuit_Selected(object sender, MyraEventArgs e)
    {
        GameController.Exit();
    }
}