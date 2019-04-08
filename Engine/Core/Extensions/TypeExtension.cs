using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Files.Serialization;

namespace System
{
    public enum EGenericVarianceFlag
    {
        None,
        CovariantOut,
        ContravariantIn,
    }
    public enum ETypeConstraintFlag
    {
        None,
        Struct,             //struct
        Class,              //class
        NewClass,           //class, new()
        NewStructOrClass,   //new()
    }
    public static class TypeExtension
    {
        private static readonly Dictionary<Type, string> DefaultDictionary = new Dictionary<Type, string>
        {
            { typeof(void),     "void"      },
            { typeof(char),     "char"      },
            { typeof(bool),     "bool"      },
            { typeof(byte),     "byte"      },
            { typeof(sbyte),    "sbyte"     },
            { typeof(short),    "short"     },
            { typeof(ushort),   "ushort"    },
            { typeof(int),      "int"       },
            { typeof(uint),     "uint"      },
            { typeof(long),     "long"      },
            { typeof(ulong),    "ulong"     },
            { typeof(float),    "float"     },
            { typeof(double),   "double"    },
            { typeof(decimal),  "decimal"   },
            { typeof(string),   "string"    },
            { typeof(object),   "object"    },
        };

        public static bool IsAssignableTo(this Type type, Type other) => other.IsAssignableFrom(type);
        
        /// <summary>
        /// Returns the type as a string in the form that it is written in code.
        /// </summary>
        public static string GetFriendlyName(this Type type, string openBracket = "<", string closeBracket = ">")
            => type.GetFriendlyName(DefaultDictionary, openBracket, closeBracket);
        /// <summary>
        /// Returns the type as a string in the form that it is written in code.
        /// </summary>
        public static string GetFriendlyName(this Type type, Dictionary<Type, string> translations, string openBracket = "<", string closeBracket = ">")
        {
            if (type == null)
                return "null";
            else if (translations.ContainsKey(type))
                return translations[type];
            else if (type.IsArray)
                return GetFriendlyName(type.GetElementType(), translations, openBracket, closeBracket) + "[]";
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetFriendlyName(type.GetGenericArguments()[0], translations, openBracket, closeBracket) + "?";
            else if (type.IsGenericType)
                return type.Name.Split('`')[0] + openBracket + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x, translations, openBracket, closeBracket))) + closeBracket;
            else
                return type.Name;
        }

        /// <summary>
        /// Returns the default value for the given type. Similar to default(T).
        /// </summary>
        public static object GetDefaultValue(this Type t)
            => t == null ? null : (t.IsValueType && Nullable.GetUnderlyingType(t) == null) ? SerializationCommon.CreateInstance(t) : null;

        public static void GetGenericParameterConstraints(this Type genericTypeParam, out EGenericVarianceFlag gvf, out ETypeConstraintFlag tcf)
        {
            GenericParameterAttributes gpa = genericTypeParam.GenericParameterAttributes;
            GenericParameterAttributes variance = gpa & GenericParameterAttributes.VarianceMask;
            GenericParameterAttributes constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

            gvf = EGenericVarianceFlag.None;
            tcf = ETypeConstraintFlag.None;

            if (variance != GenericParameterAttributes.None)
            {
                if ((variance & GenericParameterAttributes.Covariant) != 0)
                    gvf = EGenericVarianceFlag.CovariantOut;
                else
                    gvf = EGenericVarianceFlag.ContravariantIn;
            }

            if (constraints != GenericParameterAttributes.None)
            {
                if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                    tcf = ETypeConstraintFlag.Struct;
                else
                {
                    if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
                        tcf = ETypeConstraintFlag.NewStructOrClass;
                    if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
                    {
                        if (tcf == ETypeConstraintFlag.NewStructOrClass)
                            tcf = ETypeConstraintFlag.NewClass;
                        else
                            tcf = ETypeConstraintFlag.Class;
                    }
                }
            }
        }

        public static T GetCustomAttributeExt<T>(this Type type) where T : Attribute
        {
            T[] types = type.GetCustomAttributesExt<T>();
            if (types.Length > 0)
                return types[0];
            return null;
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
        public static MemberInfo[] GetMembersExt(this Type type, BindingFlags bindingAttr)
        {
            bindingAttr &= ~BindingFlags.FlattenHierarchy;
            bindingAttr |= BindingFlags.DeclaredOnly;
            List<MemberInfo> members = new List<MemberInfo>();
            while (type != null)
            {
                members.AddRange(type.GetMembers(bindingAttr));
                type = type.BaseType;
            }
            return members.ToArray();
        }
    }
}
