using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using TheraEngine.Core.Tools;
using TheraEngine.Core.Files;
using System.Threading.Tasks;
using System.Threading;

namespace TheraEngine.Core.Files.Serialization
{
    /// <summary>
    /// Stores a field/property's information.
    /// </summary>
    public class VarInfo
    {
        private MemberInfo _info;

        public Type OwningType { get; }
        public Type VariableType { get; }
        public string Name { get; }
        public string Category { get; } = null;
        public TSerialize Attrib { get; }

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
            if (obj is null)
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
            Name = new string(name.Where(x => !char.IsWhiteSpace(x)).ToArray());
        }
        public VarInfo(Type type, Type owningType)
        {
            _info = null;
            Attrib = null;
            VariableType = type;
            OwningType = owningType;
            Name = null;
            Category = null;
        }
        public VarInfo(MemberInfo info)
        {
            _info = info;
            Attrib = _info.GetCustomAttribute<TSerialize>();
            if (_info.MemberType.HasFlag(MemberTypes.Field))
                VariableType = ((FieldInfo)_info).FieldType;
            else if (_info.MemberType.HasFlag(MemberTypes.Property))
                VariableType = ((PropertyInfo)_info).PropertyType;
            OwningType = _info.DeclaringType;
            if (Attrib.NameOverride != null)
                Name = Attrib.NameOverride;
            else
            {
                //Don't want to use display name, usually includes spaces or specialized formatting
                //DisplayNameAttribute nameAttrib = _info.GetCustomAttribute<DisplayNameAttribute>();
                //if (nameAttrib != null)
                //    Name = nameAttrib.DisplayName;
                //else
                    Name = _info.Name;
            }
            Name = new string(Name.Where(x => !char.IsWhiteSpace(x)).ToArray());
            if (Attrib.UseCategory)
            {
                if (Attrib.OverrideCategory != null)
                    Category = SerializationCommon.FixElementName(Attrib.OverrideCategory);
                else
                {
                    CategoryAttribute categoryAttrib = _info.GetCustomAttribute<CategoryAttribute>();
                    if (categoryAttrib != null)
                        Category = SerializationCommon.FixElementName(categoryAttrib.Category);
                }
            }
        }
        public override string ToString() => Name;
    }
}
