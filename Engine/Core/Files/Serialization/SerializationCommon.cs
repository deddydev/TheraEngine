using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Tools;

namespace TheraEngine.Files.Serialization
{
    internal enum InterfaceType
    {
        None,
        IList,
    }
    internal class MemberTreeNode
    {
        public MemberTreeNode(object root)
            : this(root, root == null ? null : new VarInfo(root.GetType(), null)) { }
        public MemberTreeNode(object obj, VarInfo info)
        {
            Object = obj;
            Info = info;
            Interface = InterfaceType.None;

            if (info == null)
                return;

            List<VarInfo> members = SerializationCommon.CollectSerializedMembers(info.VariableType);

            Members = members.
                Where(x => (x.Attrib == null || x.Attrib.Condition == null) ? true : ExpressionParser.Evaluate<bool>(x.Attrib.Condition, obj)).
                Select(x => new MemberTreeNode(obj == null ? null : x.GetValue(obj), x)).
                ToList();

            CategorizedMembers = Members.Where(x => x.Info.Category != null).GroupBy(x => SerializationCommon.FixElementName(x.Info.Category)).ToList();
            foreach (var grouping in CategorizedMembers)
                foreach (MemberTreeNode p in grouping)
                    Members.Remove(p);

            if (Object is IList array)
            {
                Interface = InterfaceType.IList;
                IListMembers = new MemberTreeNode[array.Count];
                for (int i = 0; i < array.Count; ++i)
                    IListMembers[i] = new MemberTreeNode(array[i]);
            }
        }
        
        public object Object;
        public VarInfo Info;
        public int CalculatedSize;
        public int FlagSize;
        public List<MemberTreeNode> Members;
        public List<IGrouping<string, MemberTreeNode>> CategorizedMembers;
        public InterfaceType Interface;
        public MemberTreeNode[] IListMembers;

        public override string ToString()
        {
            return Info.Name;
        }
    }
    /// <summary>
    /// Stores a field/property's information.
    /// </summary>
    internal class VarInfo
    {
        private string _name;
        private string _category = null;
        private MemberInfo _info;
        private TSerialize _attrib;
        private Type _variableType, _owningType;

        public Type OwningType => _owningType;
        public Type VariableType => _variableType;
        public string Name => _name;
        public string Category => _category;
        public TSerialize Attrib => _attrib;

        public void SetValue(object obj, object value)
        {
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                ((FieldInfo)_info).SetValue(obj, value);
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)_info;
                if (p.CanWrite)
                    p.SetValue(obj, value);
                else
                    Engine.LogWarning("Can't set property '" + p.Name + "' in " + p.DeclaringType.GetFriendlyName());
            }
        }
        public object GetValue(object obj)
        {
            if (ReferenceEquals(obj, null))
                return null;
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                return ((FieldInfo)_info).GetValue(obj);
            if (_info.MemberType.HasFlag(MemberTypes.Property))
            {
                PropertyInfo p = (PropertyInfo)_info;
                if (p.CanRead)
                    return p.GetValue(obj);
                else
                    Engine.LogWarning("Can't read property '" + p.Name + "' in " + p.DeclaringType.GetFriendlyName());
            }
            return null;
        }
        public VarInfo(Type type, Type owningType, string name) : this(type, owningType)
        {
            _name = name;
        }
        public VarInfo(Type type, Type owningType)
        {
            _info = null;
            _attrib = null;
            _variableType = type;
            _owningType = owningType;
            _name = null;
            _category = null;
        }
        public VarInfo(MemberInfo info)
        {
            _info = info;
            _attrib = _info.GetCustomAttribute<TSerialize>();
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                _variableType = ((FieldInfo)_info).FieldType;
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
                _variableType = ((PropertyInfo)_info).PropertyType;
            _owningType = _info.DeclaringType;
            if (_attrib.NameOverride != null)
                _name = _attrib.NameOverride;
            else
            {
                DisplayNameAttribute nameAttrib = _info.GetCustomAttribute<DisplayNameAttribute>();
                if (nameAttrib != null)
                    _name = nameAttrib.DisplayName;
                else
                    _name = _info.Name;
            }
            if (_attrib.UseCategory)
            {
                if (_attrib.OverrideXmlCategory != null)
                    _category = _attrib.OverrideXmlCategory;
                else
                {
                    CategoryAttribute categoryAttrib = _info.GetCustomAttribute<CategoryAttribute>();
                    if (categoryAttrib != null)
                        _category = categoryAttrib.Category;
                }
            }
        }
        public override string ToString() => Name;
    }
    internal static partial class SerializationCommon
    {
        internal static string GetTypeName(Type t)
        {
            if (t == null || t.IsInterface)
                return null;
            string name = t.Name;
            if (t.IsGenericType)
            {
                return t.Name.Remove(t.Name.IndexOf('`'))/* + "-"*/;
                //Type[] genericArgs = t.GenericTypeArguments;
                //bool first = true;
                //foreach (Type gt in genericArgs)
                //{
                //    if (first)
                //        first = false;
                //    else
                //        name += ", ";
                //    name += GetTypeName(gt);
                //}
                //name += "-";
            }
            return t.Name;
        }
        internal static List<VarInfo> CollectSerializedMembers(Type t)
        {
            List<VarInfo> fields = t.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).
                Where(x => (x is FieldInfo || x is PropertyInfo) && Attribute.IsDefined(x, typeof(TSerialize))).
                Select(x => new VarInfo(x)).
                //False comes first, so negate the bool so attributes are first
                OrderBy(x => x.Attrib.XmlNodeType != EXmlNodeType.Attribute).ToList();

            int attribCount = fields.Count(x => x.Attrib.XmlNodeType == EXmlNodeType.Attribute);
            int elementCount = fields.Count - attribCount;

            for (int i = 0; i < fields.Count; ++i)
            {
                VarInfo info = fields[i];
                TSerialize s = info.Attrib;
                if (s.Order >= 0)
                {
                    int index = s.Order;

                    if (i < attribCount)
                        index = index.Clamp(0, attribCount - 1);
                    else
                        index = index.Clamp(0, elementCount - 1) + attribCount;

                    if (index == i)
                        continue;
                    fields.RemoveAt(i--);
                    if (index == fields.Count)
                        fields.Add(info);
                    else
                        fields.Insert(index, info);
                }
            }
            return fields;
        }

        //TODO: run constructor or not?
        /// <summary>
        /// Creates an object of the given type.
        /// </summary>
        public static object CreateObject(Type t)
        {
            //return FormatterServices.GetUninitializedObject(t);
            return Activator.CreateInstance(t);
        }
        public enum ValueType
        {
            Array,
            Dictionary,
            Parsable,
            Enum,
            String,
            Struct,
            Pointer,
            Manual,
        }
        public static ValueType GetValueType(Type t)
        {
            if (t.IsSubclassOf(typeof(FileObject)) && (FileObject.GetFileExtension(t)?.ManualXmlConfigSerialize == true))
            {
                return ValueType.Manual;
            }
            else if (t.GetInterface("IParsable") != null)
            {
                return ValueType.Parsable;
            }
            else if (t.GetInterface("IList") != null)
            {
                return ValueType.Array;
            }
            else if (t.GetInterface("IDictionary") != null)
            {
                return ValueType.Dictionary;
            }
            else if (t.IsEnum)
            {
                return ValueType.Enum;
            }
            else if (t == typeof(string))
            {
                return ValueType.String;
            }
            else if (t.IsValueType)
            {
                return ValueType.Struct;
            }
            else
            {
                return ValueType.Pointer;
            }
        }
        public static bool IsEnum(Type t)
        {
            return string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture);
        }
        public static bool IsPrimitiveType(Type t)
        {
            switch (t.Name)
            {
                case "Boolean":
                case "SByte":
                case "Byte":
                case "Char":
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Single":
                case "Double":
                case "Decimal":
                case "String":
                    return true;
            }
            return false;
        }

        public static string FixElementName(string name)
        {
            //Element names are case-sensitive
            //Element names must start with a letter or underscore
            //Element names cannot start with the letters xml(or XML, or Xml, etc)
            //Element names can contain letters, digits, hyphens, underscores, and periods
            //Element names cannot contain spaces

            name = name.Replace(" ", "");
            if (name.ToLowerInvariant().StartsWith("xml"))
                name = name.Substring(3);

            return name;
        }
    }
}
