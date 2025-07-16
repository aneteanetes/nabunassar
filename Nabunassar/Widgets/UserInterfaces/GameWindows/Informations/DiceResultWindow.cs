using Cyotek.Drawing.BitmapFont;
using FontStashSharp;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using System.IO.IsolatedStorage;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Informations
{
    internal class DiceResultWindow : InformationWindow
    {
        private FontSystem fontBitter;

        public DiceResultWindow(NabunassarGame game, GameObject gameObject) : base(game, gameObject)
        {
        }

        protected override string Portrait => "Assets/Images/Objects/dices.jpg";

        protected override int FillInformationWindow(VerticalStackPanel informationpanel)
        {
            var rollResult = this.GameObject.RollResult;

            var textResult = rollResult.IsSuccess
                ? Game.Strings["UI"]["Success"].ToString()
                : Game.Strings["UI"]["Failure"].ToString();

            AddRollResult(informationpanel, rollResult.Complexity, Game.Strings["UI"]["Difficult"]);
            AddRollResult(informationpanel, rollResult.Roll, Game.Strings["UI"]["Throw"],true);
            AddTitle(informationpanel, $"{Game.Strings["UI"]["Result"]}: {textResult}", new Thickness(0,15,0,0));

            return base.FillInformationWindow(informationpanel);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            fontBitter = Content.LoadFont(Fonts.BitterSemiBold);
        }

        int titleFontSize = 26;

        private void AddRollResult(VerticalStackPanel informationpanel, DiceResult diceResult, string title, bool isOffset=false)
        {
            var textFontSize = 18;

            var padding = new Myra.Graphics2D.Thickness(10);

            AddTitle(informationpanel, title);

            var complexityFormula = new Label()
            {
                Text = $"{Game.Strings["UI"]["Formula"]}: {diceResult.ToFormula()}",
                Font = fontBitter.GetFont(textFontSize),
                Wrap = true,
                Padding = padding,
            };
            informationpanel.Widgets.Add(complexityFormula);

            var complexityResult = new Label()
            {
                Text = $"{Game.Strings["UI"]["Result"]}: {diceResult.ToString()}",
                Font = fontBitter.GetFont(textFontSize),
                Wrap = true,
                Padding = padding,
            };
            informationpanel.Widgets.Add(complexityResult);
        }

        private void AddTitle(VerticalStackPanel informationpanel, string title, Thickness padding = default)
        {
            var complexityTitle = new Label()
            {
                Text = title,
                Font = fontBitter.GetFont(titleFontSize),
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding=padding,
            };
            informationpanel.Widgets.Add(complexityTitle);
        }

        protected override void InitWindow(Window window)
        {
            base.InitWindow(window);
            window.Title = Game.Strings["UI"]["ThrowResult"];
        }
    }
}
