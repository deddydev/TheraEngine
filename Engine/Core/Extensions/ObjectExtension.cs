using TheraEngine;
using TheraEngine.Core.Reflection;

namespace Extensions
{
    public static class ObjectExtension
    {
        public static TypeProxy GetTypeProxy(this object o)
        {
            if (o is IObject iobj && iobj.Domain.IsGameDomain())
                return iobj.GetTypeProxy();
            
            return o.GetType();
        }
        /// <summary>
        /// Creates an entirely independent copy of the given object.
        /// </summary>
        //public static object HardCopy(this object o)
        //{
        //    if (o is null)
        //        return null;

        //    Type t = o.GetType();

        //    if (t.IsPrimitive)
        //        return o;

        //    if (t == typeof(string))
        //        return string.Copy((string)o);

        //    object o2 = FormatterServices.GetUninitializedObject(t);
        //    FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        //    foreach (FieldInfo field in fields)
        //        field.SetValue(o2, field.GetValue(o).HardCopy());

        //    return o2;
        //}
    }
}
