using CustomEngine.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomEngine.Files.Serialization
{
    /// <summary>
    /// Stores a field/property's information.
    /// </summary>
    internal class VarInfo
    {
        private string _name;
        private string _category = null;
        private MemberInfo _info;
        private Serialize _attrib;
        private Type _variableType;

        public Type VariableType => _variableType;
        public string Name => _name;
        public string Category => _category;
        public Serialize Attrib => _attrib;

        public void SetValue(object obj, object value)
        {
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                ((FieldInfo)_info).SetValue(obj, value);
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
                ((PropertyInfo)_info).SetValue(obj, value);
        }
        public object GetValue(object obj)
        {
            if (ReferenceEquals(obj, null))
                return null;
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                return ((FieldInfo)_info).GetValue(obj);
            if (_info.MemberType.HasFlag(MemberTypes.Property))
                return ((PropertyInfo)_info).GetValue(obj);
            return null;
        }
        public VarInfo(Type type)
        {
            _info = null;
            _attrib = null;
            _variableType = type;
            _name = null;
            _category = null;
        }
        public VarInfo(MemberInfo info)
        {
            _info = info;
            _attrib = _info.GetCustomAttribute<Serialize>();
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                _variableType = ((FieldInfo)_info).FieldType;
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
                _variableType = ((PropertyInfo)_info).PropertyType;
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
                Where(x => (x is FieldInfo || x is PropertyInfo) && Attribute.IsDefined(x, typeof(Serialize))).
                Select(x => new VarInfo(x)).
                //False comes first, so negate the bool so attributes are first
                OrderBy(x => !x.Attrib.IsXmlAttribute).ToList();

            int attribCount = fields.Count(x => x.Attrib.IsXmlAttribute);
            int elementCount = fields.Count - attribCount;

            for (int i = 0; i < fields.Count; ++i)
            {
                VarInfo info = fields[i];
                Serialize s = info.Attrib;
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
    }
}
