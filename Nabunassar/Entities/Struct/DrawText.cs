using FontStashSharp.RichText;
using info.lundin.math;
using Nabunassar.Resources;
using static System.Net.Mime.MediaTypeNames;

namespace Nabunassar.Entities.Struct
{
    internal class DrawText
    {
        private string _text;

        private DrawText(string text)
        {
            _text = Escape(text);
        }

        private string Escape(string text)
        {
            return text.Replace("/", "//");
        }

        public static DrawText Create(string text)
        {
            var dt = new DrawText(text);
            return dt;
        }

        public DrawText Color(Color color)
        {
            _text += $"/c[{color.ToHexString()}]";
            return this;
        }

        private int fontSize = 10;
        private string fontName = Fonts.Retron;

        public DrawText Font(string fontName_ttf,int size)
        {
            fontName = fontName_ttf;
            fontSize = size;

            _text+= $"/f[{fontName_ttf}, {size}]";
            return this;
        }

        public DrawText Font(string fontName_ttf) => Font(fontName_ttf,fontSize);

        public DrawText Size(int size) => Font(fontName, size);

        public DrawText Blurry(float blurry)
        {
            _text += $"/eb[{blurry}]";
            return this;
        }

        public DrawText Stroke(float blurry)
        {
            _text += $"/es[{blurry}]";
            return this;
        }

        public DrawText Offset(int offsetInPixels)
        {
            _text += $"/v[{offsetInPixels}]";
            return this;
        }

        public DrawText Append(string text)
        {
            _text += text;
            return this;
        }

        public DrawText AppendSpace()
        {
            _text += " ";
            return this;
        }

        public DrawText Append(DrawText text)
        {
            _text += text._text;
            return this;
        }

        public DrawText AppendLine(DrawText text = default)
        {
            _text += "/n" + text == default ? text._text : "";
            return this;
        }

        public DrawText AppendSpace(int sizeInPixels)
        {
            _text += $"/s[{sizeInPixels}]";
            return this;
        }

        public DrawText Underline()
        {
            _text += "/tu";
            return this;
        }

        public DrawText StrikeThrough()
        {
            _text += "/ts";
            return this;
        }

        public DrawText ResetStyle()
        {
            _text += "/td";
            return this;
        }

        public DrawText ResetOffset()
        {
            _text += "/vd";
            return this;
        }

        public DrawText ResetEffects()
        {
            _text += "/ed";
            return this;
        }

        public DrawText ResetFont()
        {
            _text += "/fd";
            return this;
        }

        public DrawText ResetColor()
        {
            _text += "/cd";
            return this;
        }

        public DrawText Image(string imagepath)
        {
            _text += $"/i[{imagepath}]";
            return this;
        }

        public override string ToString()
        {
            return _text;
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _text.Equals(obj);
        }
    }
}
