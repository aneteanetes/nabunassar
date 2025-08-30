using Nabunassar.Entities.Struct;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace Nabunassar.Entities.Data.Descriptions
{
    internal class Description : IEnumerable<DescriptionRow>
    {
        public List<DescriptionRow> Rows { get; set; } = [];

        public float ProportionLeft { get; internal set; }

        public float ProportionRight { get; internal set; }


        public static DescriptionBuilder Create(string name, Color color = default)
        {
            var titleRow = new DescriptionRow()
            {
                Left = new DescriptionPart()
                {
                    Text = name,
                    Color = color == default ? Globals.BaseColorLight : color
                }
            };
            return new DescriptionBuilder(new Description(), titleRow);
        }

        public IEnumerator<DescriptionRow> GetEnumerator() => Rows.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class DescriptionRow : IEnumerable<DescriptionPart>
    {
        public DescriptionPart Left { get; set; }

        public DescriptionPart Right { get; set; }

        public IEnumerator<DescriptionPart> GetEnumerator()
        {
            if (Left != null)
                yield return Left;

            if (Right != null)
                yield return Right;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class DescriptionPart
    {
        public DescriptionPart(string text = null)
        {
            Text = text;
        }

        public string Text { get; set; }

        public Color Color { get; set; } = Globals.BaseColor;

        public int Size { get; set; }

        public DescriptionPosition Position { get; set; }
    }

    public enum DescriptionPosition
    {
        Left,
        Right,
        Center
    }

    internal class DescriptionBuilder
    {
        private Description _description;
        private DescriptionRow _currentRow;

        public int TextSizeTitle { get; private set; }

        public int TextSizeDefault { get; private set; }

        public DescriptionBuilder(Description description, DescriptionRow currentRow, int titleFontSize = 26, int textFontSize = 18)
        {
            _description = description;
            _currentRow = currentRow;
            TextSizeTitle = titleFontSize;
            TextSizeDefault = textFontSize;

            if (_currentRow.Left.Size == default)
                _currentRow.Left.Size = titleFontSize;
        }

        public DescriptionBuilder Append(DescriptionPosition position, string text, Color color = default, int size = -1)
            => Append(CreatePart(position, text, color, size));

        public DescriptionBuilder Append(DescriptionPosition position, DrawText drawText)
            => Append(CreatePart(position, drawText.ToString()));

        public DescriptionBuilder Append(DescriptionPart descriptionPart)
        {
            if (descriptionPart.Position == DescriptionPosition.Center)
                return AppendLine(descriptionPart);

            var isNeedCreateNewRow = true;

            if (descriptionPart.Position == DescriptionPosition.Left && _currentRow.Left == null)
                isNeedCreateNewRow = false;

            if (descriptionPart.Position == DescriptionPosition.Right && _currentRow.Right == null)
                isNeedCreateNewRow = false;

            if (isNeedCreateNewRow)
            {
                AppendCurrentRow();
            }

            AddDescription(descriptionPart);

            return this;
        }

        public DescriptionBuilder SetProportion(double left, double right)
        {
            _description.ProportionLeft = ((float)left);
            _description.ProportionRight = ((float)right);

            return this;
        }

        public DescriptionBuilder AppendLine(DescriptionPosition position, string text, Color color = default, int size = -1)
            => AppendLine(CreatePart(position,text, color, size));

        public DescriptionBuilder AppendLine(DescriptionPosition position, DrawText drawText)
            => AppendLine(CreatePart(position, drawText.ToString()));

        public DescriptionBuilder AppendLine(DescriptionPart descriptionPart)
        {
            AppendCurrentRow();
            AddDescription(descriptionPart);
            AppendCurrentRow();

            return this;
        }

        private DescriptionPart CreatePart(DescriptionPosition position, string text, Color color = default, int size = -1)
        {
            return new DescriptionPart()
            {
                Text = text,
                Color = color == default ? Globals.BaseColorLight : color,
                Size = size < 1 ? TextSizeDefault : size,
                Position = position
            };
        }

        private void AppendCurrentRow()
        {
            CheckIsBuilded();

            if (_currentRow.Left == null && _currentRow.Right == null)
                return;

            _description.Rows.Add(_currentRow);
            _currentRow = new DescriptionRow();
        }

        private void AddDescription(DescriptionPart descriptionPart)
        {
            CheckIsBuilded();

            if (descriptionPart.Position == DescriptionPosition.Right)
                _currentRow.Right = descriptionPart;
            else
                _currentRow.Left = descriptionPart;

        }

        private void CheckIsBuilded()
        {
            if (_isBuilded)
                throw new InvalidOperationException("Can't append row in already bulided description!");
        }

        private bool _isBuilded = false;

        public Description Build()
        {
            AppendCurrentRow();
            _isBuilded = true;
            return _description;
        }

        public static implicit operator Description(DescriptionBuilder builder) => builder.Build();
    }
}