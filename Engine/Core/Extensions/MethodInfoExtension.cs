﻿using System.Reflection;

namespace System
{
    public static class MethodInfoExtension
    {
        public static string GetFriendlyName(this MethodBase method, string openBracket = "<", string closeBracket = ">")
        {
            if (method == null)
                return "null";
            
            MethodInfo realMethod = method as MethodInfo;

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

            //There is no way to know if new should be included or not 
            //without the class type passed in as well to check if it is a derived type
            //if (method.IsHideBySig && method.DeclaringType.IsAssignableFrom(classType))
            //    friendlyName += "new ";

            if (method.IsVirtual)
                friendlyName += "virtual ";
            if (realMethod != null && realMethod.GetBaseDefinition() != realMethod)
                friendlyName += "override ";
            if (method.IsAbstract)
                friendlyName += "abstract ";

            if (realMethod != null)
                friendlyName += realMethod.ReturnType.GetFriendlyName() + " ";
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
                    friendlyName += " = ";
                    if (p.DefaultValue == null)
                    {
                        friendlyName += "null";
                    }
                    else
                    {
                        if (p.ParameterType == typeof(string))
                            friendlyName += "\"" + p.DefaultValue.ToString() + "\"";
                        else if(p.ParameterType == typeof(char))
                            friendlyName += "\'" + p.DefaultValue.ToString() + "\'";
                        else
                            friendlyName += p.DefaultValue.ToString();
                    }
                }
            }
            friendlyName += ")";
            return friendlyName;
        }
    }
}
