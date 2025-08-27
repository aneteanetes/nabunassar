using Geranium.Reflection;
using System.Reflection;

namespace Nabunassar.Struct
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class ValueAttribute : Attribute
    {
        public object Value { get; set; }

        public ValueAttribute(object value) => Value = value;
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class IconAttribute : ValueAttribute
    {
        public IconAttribute(object value) : base(value)
        {
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class TitleAttribute : ValueAttribute
    {
        public TitleAttribute(object value) : base(value)
        {
        }

        public new string Value => base.Value.ToString();
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class InfoAttribute : TitleAttribute
    {
        public InfoAttribute(object value) : base(value)
        {
        }

        public new string Value => base.Value.ToString();
    }

    public static class ValueAttributeExtensions
    {
        public static object ValueAttribute<T>(this Assembly obj)
            where T : ValueAttribute
        {
            return GetAttribute<T>(Attribute.GetCustomAttributes(obj, typeof(T)));
        }

        public static object ValueAttribute<T>(this MemberInfo obj)
            where T : ValueAttribute
        {
            return GetAttribute<T>(Attribute.GetCustomAttributes(obj, typeof(T)));
        }

        public static object ValueAttribute<T>(this Module obj)
            where T : ValueAttribute
        {
            return GetAttribute<T>(Attribute.GetCustomAttributes(obj, typeof(T)));
        }

        public static object ValueAttribute<T>(this ParameterInfo obj)
            where T : ValueAttribute
        {
            return GetAttribute<T>(Attribute.GetCustomAttributes(obj, typeof(T)));
        }

        private static object GetAttribute<T>(Attribute[] attributes) where T : ValueAttribute => attributes.FirstOrDefault(x => x is T).As<T>()?.Value;

        public static object ValueAttribute(this Assembly asm) => asm.ValueAttribute<ValueAttribute>();

        public static object ValueAttribute(this ParameterInfo asm) => asm.ValueAttribute<ValueAttribute>();

        public static object ValueAttribute(this Module asm) => asm.ValueAttribute<ValueAttribute>();

        public static object ValueAttribute(this MemberInfo asm) => asm.ValueAttribute<ValueAttribute>();

        public static TResult Value<TAttribute, TResult>(this Assembly asm) where TAttribute : ValueAttribute => asm.ValueAttribute<TAttribute>().As<TResult>();

        public static TResult Value<TAttribute, TResult>(this ParameterInfo asm) where TAttribute : ValueAttribute => asm.ValueAttribute<TAttribute>().As<TResult>();

        public static TResult Value<TAttribute, TResult>(this Module asm) where TAttribute : ValueAttribute => asm.ValueAttribute<TAttribute>().As<TResult>();

        public static TResult Value<TAttribute, TResult>(this MemberInfo asm) where TAttribute : ValueAttribute => asm.ValueAttribute<TAttribute>().As<TResult>();

        public static TResult ValueAttr<TAttribute, TResult>(this object enumValue)
           where TAttribute : ValueAttribute
        {
            var enumType = enumValue.GetType();
            if (!enumType.IsEnum)
                return default;

            var field = enumType.GetField(enumValue.ToString());
            if (field == default)
                return default;

            return field.Value<TAttribute, TResult>();
        }

        public static bool IsAttributeExists<TAttribute>(this Type type) where TAttribute : Attribute
            => Attribute.GetCustomAttributes(type, typeof(TAttribute)).Length != 0;
    }
}