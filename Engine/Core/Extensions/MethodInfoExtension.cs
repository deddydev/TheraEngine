using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class MethodInfoExtension
    {
        public static string GetFriendlyName(this MethodInfo method, string openBracket = "<", string closeBracket = ">")
        {
            if (method == null)
                return "null";

            string friendlyName = "";
            if (method.IsPublic)
                friendlyName += "public ";
            if (method.IsPrivate)
                friendlyName += "private ";
            if (method.IsFamily)
                friendlyName += "protected ";
            if (method.IsAssembly)
                friendlyName += "internal ";
            if (method.IsFinal)
                friendlyName += "sealed ";
            if (method.IsStatic)
                friendlyName += "static ";
            if (method.IsHideBySig)
                friendlyName += "new ";
            if (method.IsVirtual)
                friendlyName += "virtual ";
            if (method.GetBaseDefinition() != method)
                friendlyName += "override ";
            if (method.IsAbstract)
                friendlyName += "abstract ";
            friendlyName += method.Name;
            if (method.IsGenericMethod)
            {
                friendlyName += openBracket;
                Type[] parameters = method.GetGenericArguments();
                bool first = true;
                foreach (Type parameter in parameters)
                {
                    if (first)
                        first = false;
                    else
                        friendlyName += ", ";
                    friendlyName += parameter.GetFriendlyName(openBracket, closeBracket);
                }
                friendlyName += closeBracket;
            }
        }
    }
}
