using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Core.Reflection
{
    public class PropertyInfoProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<PropertyInfo, PropertyInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<PropertyInfo, PropertyInfoProxy>();
        public static PropertyInfoProxy Get(PropertyInfo info)
            => info == null ? null : Proxies.GetOrAdd(info, new PropertyInfoProxy(info));
        public static implicit operator PropertyInfoProxy(PropertyInfo info) => Get(info);
        public static implicit operator PropertyInfo(PropertyInfoProxy proxy) => proxy.Value;

        private PropertyInfo Value { get; set; }

        //public PropertyInfoProxy() { }
        private PropertyInfoProxy(PropertyInfo value) : base(value) => Value = value;

        public TypeProxy PropertyType => Value.PropertyType;

        public bool CanRead => Value.CanRead;
        public bool CanWrite => Value.CanWrite;
        public MethodInfoProxy GetMethod => Value.GetMethod;
        public MethodInfoProxy SetMethod => Value.SetMethod;

        public object GetValue(object parentObject)
            => Value.GetValue(parentObject);
        public void SetValue(object parentObject, object memberObject)
            => Value.SetValue(parentObject, memberObject);
        public ParameterInfoProxy[] GetIndexParameters()
            => Value.GetIndexParameters().Select(x => ParameterInfoProxy.Get(x)).ToArray();
        public void GetStringAttributes(out bool isMultiLine, out bool isPath, out bool isNullable, out bool isUnicode)
        {
            object[] attribs = GetCustomAttributes(true);
            if (attribs.FirstOrDefault(x => x is TStringAttribute) is TStringAttribute s)
            {
                isMultiLine = s.MultiLine;
                isPath = s.Path;
                isNullable = s.Nullable;
                isUnicode = s.Unicode;
            }
            else
            {
                isMultiLine = false;
                isPath = false;
                isNullable = true;
                isUnicode = false;
            }
        }
        public void GetGridDisplayAttributes(
            out string visibilityCondition, 
            out bool visible,
            out bool readOnly,
            out string displayNameOverride,
            out bool editInPlace,
            out string description)
        {
            visibilityCondition = null;
            visible = GetMethod.IsPublic && CanRead;
            readOnly = !CanWrite || !SetMethod.IsPublic;
            displayNameOverride = null;
            editInPlace = false;
            description = null;

            var attribs = GetCustomAttributes(true);
            foreach (object attrib in attribs)
            {
                switch (attrib)
                {
                    case BrowsableIf browsableIf:
                        visibilityCondition = browsableIf.Condition;
                        break;
                    case BrowsableAttribute browsable:
                        visible = visible && browsable.Browsable;
                        break;
                    case ReadOnlyAttribute readOnlyAttrib:
                        readOnly = readOnly || readOnlyAttrib.IsReadOnly;
                        break;
                    case DisplayNameAttribute displayName:
                        displayNameOverride = displayName.DisplayName;
                        break;
                    case EditInPlace editInPlaceAttrib:
                        editInPlace = true;
                        break;
                    case DescriptionAttribute desc:
                        description = desc.Description;
                        break;
                }
            }
        }
        public bool HasIndexParameters()
            => Value.GetIndexParameters().Length > 0;
    }
}
