using Nabunassar.Struct;
using System.Numerics;

namespace Nabunassar.Entities.Data.ComplexValues
{
    internal class ComplexValue<T>
        where T : struct, INumber<T>
    {
        private T _summarizedValue = default;
        private List<Pair<ComplexValueType, T>> _values = new();

        public T GetValue(ComplexValueType type)
        {
            T _result = default;

            var selection = _values.Where(x => x.First == type);
            foreach (var selected in selection)
            {
                _result += selected.Second;
            }

            return _result;
        }

        public T GetValueExcept(ComplexValueType type)
        {
            T _result = default;

            var selection = _values.Where(x => x.First != type);
            foreach (var selected in selection)
            {
                _result += selected.Second;
            }

            return _result;
        }

        public void AddValue(T value, ComplexValueType type)
        {
            _values.Add(new Pair<ComplexValueType, T>(type, value));
            _summarizedValue += value;
        }

        public T GetComplexValue() => _summarizedValue;
    }
}
