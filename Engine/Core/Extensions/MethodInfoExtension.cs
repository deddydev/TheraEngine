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
            bool first = true;
            if (method.IsGenericMethod)
            {
                friendlyName += openBracket;
                Type[] genericArgs = method.GetGenericArguments();
                foreach (Type generic in genericArgs)
                {
                    if (first)
                        first = false;
                    else
                        friendlyName += ", ";
                    friendlyName += generic.GetFriendlyName(openBracket, closeBracket);
                }
                friendlyName += closeBracket;
            }
            friendlyName += "(";
            ParameterInfo[] parameters = method.GetParameters();
            first = true;
            foreach (var p in parameters)
            {
                if (first)
                    first = false;
                else
                    friendlyName += ", ";

                if (p.IsIn)
                    friendlyName += "in ";
                if (p.IsOut)
                    friendlyName += "out ";
                if (p.ParameterType.IsByRef)
                    friendlyName += "ref ";

                string typeName = p.ParameterType.GetFriendlyName(openBracket, closeBracket);
                friendlyName += typeName + " " + p.Name;
                if (p.HasDefaultValue)
                {
                    if (p.ParameterType == typeof(string))
                        friendlyName += " = \"" + p.DefaultValue.ToString() + "\"";
                    else
                        friendlyName += " = " + p.DefaultValue.ToString();
                }
            }
            friendlyName += ")";
            return friendlyName;
        }
    }
}
