using System.Collections;
using TheraEngine.Core.Reflection;

namespace Extensions
{
    public static class TypeExtension
    {
        public static TypeProxy DetermineElementTypeProxy(this IList list)
        {
            TypeProxy listType = list.GetTypeProxy();
            TypeProxy elementType = listType.GetElementType();
            if (elementType != null)
                return elementType;
            if (listType.IsGenericType && listType.GenericTypeArguments.Length == 1)
                return listType.GenericTypeArguments[0];
            return null;
        }
        public static TypeProxy DetermineKeyTypeProxy(this IDictionary dic)
        {
            TypeProxy listType = dic.GetTypeProxy();
            if (listType.IsGenericType)
                return listType.GenericTypeArguments[0];
            return null;
        }
        public static TypeProxy DetermineValueTypeProxy(this IDictionary dic)
        {
            TypeProxy listType = dic.GetTypeProxy();
            if (listType.IsGenericType)
                return listType.GenericTypeArguments[1];
            return null;
        }
    }   
}
