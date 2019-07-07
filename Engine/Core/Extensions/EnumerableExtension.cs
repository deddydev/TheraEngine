using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;

namespace Extensions
{
    public static class EnumerableExtension
    {
        public static ThreadSafeList<T> AsThreadSafeList<T>(this IEnumerable<T> enumerable)
        {
            return new ThreadSafeList<T>(enumerable);
        }
    }   
}
