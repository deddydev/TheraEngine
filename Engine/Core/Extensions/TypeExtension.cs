using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TypeExtension
    {
        public static object GetDefaultValue(this Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null) ? Activator.CreateInstance(t) : null;
    }
}
