using Geranium.Reflection;
using Nabunassar.Resources;

namespace Nabunassar.Localization
{
    internal struct LocalizedStrings
    {
        NabunassarGame _game;
        private string _fileName;

        internal string KeyChain { get; set; }

        public string NotFound => DataBase.NotFoundStringConstant;

        public LocalizedStrings(NabunassarGame game, string fileName=null)
        {
            _game = game;
            _fileName = fileName;
        }

        private void ExpandChain(string key)
        {
            if (KeyChain.IsNotEmpty())
            {
                KeyChain += "." + key;
            }
            else
                KeyChain = key;
        }

        public LocalizedStrings this[string @const]
        {
            get
            {
                if (_fileName == null)
                {
                    return new LocalizedStrings(_game, @const);
                }
                else
                {
                    ExpandChain(@const);
                }
                return this;
            }
        }

        public LocalizedStrings this[object @const]
        {
            get
            {
                if (_fileName == null)
                    throw new ArgumentNullException("string file is not binded!");

                string key = @const.ToString();
                var type = @const.GetType();

                if (type.IsEnum)
                {
                    key = $"{type.Name}.{key}";
                }

                ExpandChain(key);
                return this;
            }
        }

        public static implicit operator string(LocalizedStrings chain)
        {
            return chain.ToString();
        }

        public override string ToString()
        {
            return _game.DataBase.GetString(_fileName, KeyChain);
        }
    }
}