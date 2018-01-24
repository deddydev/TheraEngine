using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    public enum GenericVarianceFlag
    {
        None,
        CovariantOut,
        ContravariantIn,
    }
    public enum TypeConstraintFlag
    {
        None,
        Struct,             //struct
        Class,              //class
        NewClass,           //class, new()
        NewStructOrClass,   //new()
    }
    public static class TypeExtension
    {
        //public static string GetFriendlyName(this Type type, string openBracket = "<", string closeBracket = ">")
        //{
        //    if (type == null)
        //        return "null";

        //    if (_typeToFriendlyName.TryGetValue(type, out string friendlyName))
        //        return friendlyName;

        //    friendlyName = type.Name;
        //    if (type.IsGenericType)
        //    {
        //        int backtick = friendlyName.IndexOf('`');
        //        if (backtick > 0)
        //        {
        //            friendlyName = friendlyName.Remove(backtick);
        //        }
        //        friendlyName += openBracket;
        //        Type[] typeParameters = type.GetGenericArguments();
        //        for (int i = 0; i < typeParameters.Length; i++)
        //        {
        //            string typeParamName = typeParameters[i].GetFriendlyName(openBracket, closeBracket);
        //            friendlyName += (i == 0 ? typeParamName : ", " + typeParamName);
        //        }
        //        friendlyName += closeBracket;
        //    }

        //    if (type.IsArray)
        //        return type.GetElementType().GetFriendlyName() + "[]";

        //    return friendlyName;
        //}
        
        private static Dictionary<Type, string> _defaultDictionary = new Dictionary<System.Type, string>
        {
            { typeof(void), "void" },
            { typeof(char), "char" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(string), "string" },
            { typeof(object), "object" },
        };

        public static string GetFriendlyName(this Type type, string openBracket = "<", string closeBracket = ">")
            => type.GetFriendlyName(_defaultDictionary, openBracket, closeBracket);
        public static string GetFriendlyName(this Type type, Dictionary<Type, string> translations, string openBracket = "<", string closeBracket = ">")
        {
            if (type == null)
                return "null";
            else if (translations.ContainsKey(type))
                return translations[type];
            else if (type.IsArray)
                return GetFriendlyName(type.GetElementType(), translations, openBracket, closeBracket) + "[]";
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0].GetFriendlyName() + "?";
            else if (type.IsGenericType)
                return type.Name.Split('`')[0] + openBracket + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x))) + closeBracket;
            else
                return type.Name;
        }

        public static object GetDefaultValue(this Type t)
            => (t.IsValueType && Nullable.GetUnderlyingType(t) == null) ? Activator.CreateInstance(t) : null;

        public static void GetGenericParameterConstraints(this Type genericTypeParam, out GenericVarianceFlag gvf, out TypeConstraintFlag tcf)
        {
            GenericParameterAttributes gpa = genericTypeParam.GenericParameterAttributes;
            GenericParameterAttributes variance = gpa & GenericParameterAttributes.VarianceMask;
            GenericParameterAttributes constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

            gvf = GenericVarianceFlag.None;
            tcf = TypeConstraintFlag.None;

            if (variance != GenericParameterAttributes.None)
            {
                if ((variance & GenericParameterAttributes.Covariant) != 0)
                    gvf = GenericVarianceFlag.CovariantOut;
                else
                    gvf = GenericVarianceFlag.ContravariantIn;
            }

            if (constraints != GenericParameterAttributes.None)
            {
                if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                    tcf = TypeConstraintFlag.Struct;
                else
                {
                    if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                        tcf = TypeConstraintFlag.NewStructOrClass;
                    if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                    {
                        if (tcf == TypeConstraintFlag.NewStructOrClass)
                            tcf = TypeConstraintFlag.NewClass;
                        else
                            tcf = TypeConstraintFlag.Class;
                    }
                }
            }
        }

        public static T[] GetCustomAttributesExt<T>(this Type type) where T : Attribute
        {
            List<T> list = new List<T>();
            while (type != null)
            {
                list.AddRange(type.GetCustomAttributes<T>());
                type = type.BaseType;
            }
            return list.ToArray();
        }
    }
}
