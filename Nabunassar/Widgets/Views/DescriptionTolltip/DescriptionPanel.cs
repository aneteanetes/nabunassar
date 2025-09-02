using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Resources;

namespace Nabunassar.Widgets.Views.DescriptionTolltip
{
    internal class DescriptionPanel : Grid
    {
        public DescriptionPanel(NabunassarGame game, Description description)
        {
            var texture = game.Content.LoadTexture("Assets/Images/Borders/descriptionpanel1.png");
            Background = texture.NinePatch(16);

            Width = 300;
            VerticalAlignment = VerticalAlignment.Top;
            Padding = new Myra.Graphics2D.Thickness(15);

            var _retron = game.Content.LoadFont(Fonts.Retron);
            var _bitterFont = game.Content.LoadFont(Fonts.BitterBold);

            if (description.ProportionLeft == default)
            {
                ColumnsProportions.Add(new Proportion(ProportionType.Part, 7));
                ColumnsProportions.Add(new Proportion(ProportionType.Part, 3));
            }
            else
            {
                ColumnsProportions.Add(new Proportion(ProportionType.Part, description.ProportionLeft));
                ColumnsProportions.Add(new Proportion(ProportionType.Part, description.ProportionRight));
            }

            RowSpacing = 5;
            DefaultRowProportion = new Proportion(ProportionType.Auto, 1);

            int rowCounter = 0;

            foreach (var row in description)
            {
                foreach (var part in row)
                {
                    var label = new Label()
                    {
                        Font = _retron.GetFont(part.Size),
                        Text = part.Text,
                        TextColor = part.Color,
                        Wrap = true,
                        //Width = Width-Padding.Width,
                        HorizontalAlignment = part.Position == DescriptionPosition.Right ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                    };

                    Grid.SetRow(label, rowCounter);

                    if (part.Position != DescriptionPosition.Center)
                    {
                        Grid.SetColumn(label, part.Position == DescriptionPosition.Left ? 0 : 1);
                    }
                    else
                    {
                        Grid.SetColumnSpan(label, 2);
                    }

                    //RowsProportions.Add(new Proportion(ProportionType.Auto, rowCounter));
                    Widgets.Add(label);
                }
                rowCounter++;
            }

            //this.Widgets.Add(grid);

            this.TouchDown += DescriptionPanel_TouchDown;
        }

        private void DescriptionPanel_TouchDown(object sender, MyraEventArgs e)
        {
            e.StopPropagation();
        }

        protected override void InternalArrange()
        {
            base.InternalArrange();
        }
    }
}