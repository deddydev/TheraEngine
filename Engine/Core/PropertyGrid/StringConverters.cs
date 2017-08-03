using System.ComponentModel;
using System.Globalization;
namespace System
{
    public class Vec4StringConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
            => destinationType == typeof(Vec4);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => value.ToString();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => new Vec4(value.ToString());
    }

    public class Vec3StringConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
            => destinationType == typeof(Vec3);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
            => value.ToString();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => new Vec3(value.ToString());
    }

    public class Vec2StringConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(Vec2);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => value.ToString();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => new Vec2(value.ToString());
    }
    
    public class ColorF3StringConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(ColorF3);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => value.ToString();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => new ColorF3(value.ToString());
    }

    public class ColorF4StringConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(ColorF4);
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => value.ToString();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => new ColorF4(value.ToString());
    }
}
