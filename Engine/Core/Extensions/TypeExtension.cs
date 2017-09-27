using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        private static readonly Dictionary<Type, string> _typeToFriendlyName = new Dictionary<Type, string>
        {
            { typeof(string), "string" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(short), "short" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(sbyte), "sbyte" },
            { typeof(float), "float" },
            { typeof(ushort), "ushort" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(void), "void" }
        };

        public static string GetFriendlyName(this Type type, string openBracket = "<", string closeBracket = ">")
        {
            if (_typeToFriendlyName.TryGetValue(type, out string friendlyName))
                return friendlyName;

            friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }
                friendlyName += openBracket;
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; i++)
                {
                    string typeParamName = typeParameters[i].GetFriendlyName(openBracket, closeBracket);
                    friendlyName += (i == 0 ? typeParamName : ", " + typeParamName);
                }
                friendlyName += closeBracket;
            }

            if (type.IsArray)
                return type.GetElementType().GetFriendlyName() + "[]";

            return friendlyName;
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
