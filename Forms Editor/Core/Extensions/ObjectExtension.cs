using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheraEditor.Core.Extensions
{
    public static class ObjectExtension
    {
        public static object CallPrivateMethod(this object o, string methodName, params object[] args)
            => o?.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(o, args);
    }
}
