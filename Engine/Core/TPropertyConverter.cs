using System;

namespace TheraEngine.Core
{
    public abstract class TPropertyConverter
    {
        public abstract object ConvertFromSource(object value);
        public abstract object ConvertToSource(object value);
    }
    public class TMethodPropertyConverter : TPropertyConverter
    {
        public TMethodPropertyConverter() { }
        public TMethodPropertyConverter(Func<object, object> fromSource, Func<object, object> toSource)
        {
            FromSourceConverter = fromSource;
            ToSourceConverter = toSource;
        }

        public Func<object, object> FromSourceConverter { get; set; }
        public Func<object, object> ToSourceConverter { get; set; }

        public override object ConvertFromSource(object value)
            => FromSourceConverter?.Invoke(value) ?? value;

        public override object ConvertToSource(object value)
            => ToSourceConverter?.Invoke(value) ?? value;
    }
}
