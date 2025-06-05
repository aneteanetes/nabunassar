using AssetManagementBase;
using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace Nabunassar.Interface.Menu;

internal partial class MainMenu : Panel
{
	private NabunassarGame _game;

	public MainMenu(NabunassarGame game)
	{
		_game = game;

        var background = new Image
        {
            Renderable = MyraEnvironment.DefaultAssetManager.LoadTextureRegion("Assets/Images/Backgrounds/Main.png"),
            HorizontalAlignment = HorizontalAlignment.Center,
            ResizeMode = ImageResizeMode.KeepAspectRatio
        };

        Widgets.Add(background);

        var font = game.Content.Load("Assets/Fonts/Retron2000.ttf");

        // Ordinary DynamicSpriteFont
        var fontSettings = new FontSystemSettings()
        {
            FontResolutionFactor = 2,
            
        };
        var retronFont = new FontSystem(fontSettings);
        retronFont.AddFont(font);
        //_label1.Font = ordinaryFontSystem.GetFont(32);

        var backimgnorm = game.Content.Load<Texture2D>("Assets/Images/Borders/commonborder.png");
        var backimgfocus = game.Content.Load<Texture2D>("Assets/Images/Borders/commonborderpressed.png");
        var backNormal = new NinePatchRegion(backimgnorm, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));
        var backPressed = new NinePatchRegion(backimgfocus, new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));


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
            Font = retronFont.GetFont(28),
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

        Widgets.Add(newGame);
        Widgets.Add(load);
        Widgets.Add(quit);
    }

    private void NewGame_Click(object sender, EventArgs e)
    {
        //_game.Desktop.Dispose();
        //_game.Desktop = new Desktop();
        _game.Desktop.Root = new Panel();
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
		_game.Exit();
    }
}