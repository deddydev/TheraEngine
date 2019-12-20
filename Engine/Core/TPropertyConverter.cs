namespace TheraEngine.Core
{
    public abstract class TPropertyConverter
    {
        public abstract object ConvertFromSource(object sourceValue);
        public abstract object ConvertToSource(object sourceValue);
    }
}
