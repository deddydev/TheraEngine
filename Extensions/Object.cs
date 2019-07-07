using System.Reflection;

namespace Extensions
{
    public static partial class Ext
    {
        public static object CallPrivateMethod(this object o, string methodName, params object[] args)
            => o?.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(o, args);
    }
}
