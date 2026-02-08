using Geranium.Reflection;
using System.Numerics;

namespace Nabunassar.Stats;

public class Parameter
{
    private readonly string key;
    private string valueString;

    private int? valueInt;
    private double? valueDouble;
    private float? valueFloat;
    private bool? valueBoolean;
    private Parameter valueProperty;

    private LinkedList<ParameterModifier> _modifiers = [];

    public string[] Mods => _modifiers.Select(x => x.Name).ToArray();

    public Parameter(string key, string value)
    {
        this.key = key;
        valueString = value;
        ParameterType = ParameterType.String;
    }

    public Parameter(string key, int value) : this(key, value.ToString())
    {
        valueInt = value;
        ParameterType = ParameterType.Int;
    }

    public Parameter(string key, double value) : this(key, value.ToString())
    {
        valueDouble = value;
        ParameterType = ParameterType.Double;
    }

    public Parameter(string key, bool value) : this(key, value.ToString())
    {
        valueBoolean = value;
        ParameterType = ParameterType.Boolean;
    }

    public Parameter(string key, float value) : this(key, value.ToString())
    {
        valueFloat = value;
        ParameterType = ParameterType.Float;
    }

    public Parameter(string key, Parameter value) : this(key, value.ToString())
    {
        valueProperty = value;
        ParameterType = ParameterType.Object;
    }

    public string Key => key;

    public ParameterType ParameterType { get; }

    public T Get<T>()
    {
        static T cast<TValue>(TValue value) => value.As<T>();

        var stringValue = valueString;
        if (stringValue.IsEmpty())
            return default;

        if (default(T) is Parameter)
            return valueProperty.As<T>();

        T value = default;
        var typeCode = Type.GetTypeCode(typeof(T));

        if (typeCode == TypeCode.String)
        {
            var strVal = ParameterType switch
            {
                ParameterType.String => stringValue,
                ParameterType.Int => valueInt.ToString(),
                ParameterType.Double => valueDouble.ToString(),
                ParameterType.Float => valueFloat.ToString(),
                ParameterType.Boolean => valueBoolean.ToString(),
                _ => stringValue
            };
            value = strVal.As<T>();
        }
        else
        {
            value = typeCode switch
            {
                TypeCode.Boolean => cast(valueBoolean ??= stringValue.AsBool()),
                TypeCode.Single => cast(valueFloat ??= stringValue.AsFloat()),
                TypeCode.Double => cast(valueDouble ??= stringValue.AsDouble()),
                TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 => cast(valueInt ??= stringValue.AsInt()),
                _ => default,
            };
        }

        return GetModified(value);
    }

    private T GetModified<T>(T value)
    {
        var current = _modifiers.Last;
        while (current != null)
        {
            value = current.Value.Get(value);
            current = current.Previous;
        }

        return value;
    }

    private T SetModified<T>(T value)
        where T : INumber<T>
    {
        var current = _modifiers.Last;
        while (current != null)
        {
            value = current.Value.Set(value);
            current = current.Previous;
        }

        return value;
    }

    /// <summary>
    /// Устанавливает конкретное значение игнорируя модификаторы
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public Parameter Set<T>(T value)
    {
        if (value.ToString().IsEmpty())
            return this;

        if (value is Parameter propValue)
        {
            valueProperty = propValue;
            return this;
        }

        var typeCode = Type.GetTypeCode(typeof(T));

        switch (typeCode)
        {
            case TypeCode.String: valueString = value.As<string>(); break;
            case TypeCode.Boolean: valueBoolean = new bool?(value.As<bool>()); break;
            case TypeCode.Single: valueFloat = value.As<float>(); break;
            case TypeCode.Double: valueDouble = value.As<double>(); break;
            case TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64: valueInt = value.As<int>(); break;
            default: valueString = value.ToString();
                break;
        }

        return this;
    }

    public Parameter Add<T>(T value)
        where T : INumber<T>
    {
        if (value.ToString().IsEmpty())
            return this;

        if (value is Parameter propValue)
        {
            return this;
        }

        var modified = SetModified(value);

        var typeCode = Type.GetTypeCode(typeof(T));

        switch (typeCode)
        {
            case TypeCode.Single: valueFloat += modified.As<float>(); break;
            case TypeCode.Double: valueDouble += modified.As<double>(); break;
            case TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64: 
                valueInt += value.As<int>(); break;
            default:
                break;
        }

        return this;
    }

    /// <summary>
    /// Применение модификаторов
    /// </summary>
    public void Tick()
    {
        foreach (var mod in _modifiers)
        {
            mod.Tick();
        }
    }

    public void AddModifier(ParameterModifier modifier)
    {
        /// модификаторы для объекта содержащие свойства
        /// накладывают на параметры в том числе
        _modifiers.AddFirst(modifier);
    }

    public ParameterModifier[] GetMods() => _modifiers.ToArray();

    public void ModifierRemove(string name)
    {
        var node = _modifiers.FirstOrDefault(x => x.Name == name);
        _modifiers.Remove(node);
    }

    public static Parameter CreateValue(string key, string value, ParameterType paramType)
    {
        static T convert<T>(string value) => default(T) switch
        {
            int => int.Parse(value).As<T>(),
            double => double.Parse(value).As<T>(),
            float => float.Parse(value).As<T>(),
            bool => bool.Parse(value).As<T>(),
            _ => value.As<T>(),
        };

        return paramType switch
        {
            ParameterType.Int => new Parameter(key, convert<int>(value)),
            ParameterType.Double => new Parameter(key, convert<double>(value)),
            ParameterType.Float => new Parameter(key, convert<float>(value)),
            ParameterType.Boolean => new Parameter(key, convert<bool>(value)),
            _ => new Parameter(key, value),
        };
    }

    public override string ToString()
    {
        var value = ParameterType switch
        {
            ParameterType.Int => valueInt.ToString(),
            ParameterType.Double => valueDouble.ToString(),
            ParameterType.Float => valueFloat.ToString(),
            ParameterType.Boolean => valueBoolean.ToString(),
            _ => valueString
        };

        return string.Create(null, stackalloc char[256], $"[{key}, {value}]");
    }
}