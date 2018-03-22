using System.Reflection;
using System.Runtime.Serialization;

namespace System
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Creates an entirely independent copy of the given object.
        /// </summary>
        public static object HardCopy(this object o)
        {
            if (o is null)
                return null;

            Type t = o.GetType();

            if (t.IsPrimitive)
                return o;

            if (t == typeof(string))
                return string.Copy((string)o);

            object o2 = FormatterServices.GetUninitializedObject(t);
            FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo field in fields)
                field.SetValue(o2, field.GetValue(o));

            return o2;
        }
    }
}
