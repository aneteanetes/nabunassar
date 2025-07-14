using Geranium.Reflection;
using Nabunassar.Resources;

namespace Nabunassar.Localization
{
    internal struct LocalizedStrings
    {
        NabunassarGame _game;
        private string _fileName;
        private bool fineTuningMode = false;

        internal string KeyChain { get; set; }

        public string NotFound => DataBase.NotFoundStringConstant;

        public LocalizedStrings(NabunassarGame game, string fileName=null)
        {
            _game = game;
            _fileName = fileName;
        }

        private bool _toLower = false;
        private bool _toCAPS = false;

        private void ExpandChain(string key)
        {
            if (fineTuningMode)
            {
                var IsCaps = key.All(char.IsUpper);
                if (IsCaps)
                {
                    _toCAPS = true;
                    key = NormalizeString(key);
                }

                var value = key.ToLowerInvariant();

                if (!IsCaps && value == key)
                {
                    _toLower = true;
                    key = NormalizeString(key);
                }
            }

            if (KeyChain.IsNotEmpty())
            {
                KeyChain += "." + key;
            }
            else
                KeyChain = key;
        }

        private string NormalizeString(string key)
        {
            var chars = new List<char>();
            chars.Add(char.ToUpperInvariant(key[0]));
            chars.AddRange(key.Skip(1).Select(c => char.ToLowerInvariant(c)));

            return new string(chars.ToArray());
        }

        public LocalizedStrings this[string @const]
        {
            get
            {
                if (_fileName == null)
                {
                    return new LocalizedStrings(_game, @const);
                }
                else if (fineTuningMode)
                {
                    var tunedString = new LocalizedStrings(_game, this._fileName)
                    {
                        fineTuningMode = true
                    };
                    tunedString.ExpandChain(@const);
                    return tunedString;
                }
                else
                {
                    ExpandChain(@const);
                }
                return this;
            }
        }

        public static implicit operator string(LocalizedStrings chain)
        {
            return chain.ToString();
        }


        public LocalizedStrings FineTuning()
        {
            return new LocalizedStrings(this._game, this._fileName)
            {
                fineTuningMode = true
            };
        }

        public string ToLower()
        {
            var str = this.ToString();
            return str.ToLowerInvariant();
        }

        public override string ToString()
        {
            var str = _game.DataBase.GetString(_fileName, KeyChain);

            if(_toLower)
                str = str.ToLowerInvariant();

            if(_toCAPS)
                str = new string(str.Select(char.ToUpper).ToArray());

            return str;
        }
    }
}