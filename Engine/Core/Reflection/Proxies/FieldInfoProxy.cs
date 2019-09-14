using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEngine.Core.Reflection
{
    public class FieldInfoProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<FieldInfo, FieldInfoProxy> Proxies { get; }
            = new ConcurrentDictionary<FieldInfo, FieldInfoProxy>();
        public static FieldInfoProxy Get(FieldInfo info)
            => info is null ? null : Proxies.GetOrAdd(info, new FieldInfoProxy(info));
        public static implicit operator FieldInfoProxy(FieldInfo info) => Get(info);
        public static implicit operator FieldInfo(FieldInfoProxy proxy) => proxy.Value;

        private FieldInfo Value { get; set; }

        //public FieldInfoProxy() { }
        private FieldInfoProxy(FieldInfo value) : base(value) => Value = value;

        public override bool IsGridViewable => Value.IsPublic;
        public override bool IsGridWriteable => Value.IsPublic;

        //
        // Summary:
        //     Gets a RuntimeFieldHandle, which is a handle to the internal metadata representation
        //     of a field.
        //
        // Returns:
        //     A handle to the internal metadata representation of a field.
        public RuntimeFieldHandle FieldHandle => Value.FieldHandle;
        //
        // Summary:
        //     Gets a value indicating whether the field is public.
        //
        // Returns:
        //     true if this field is public; otherwise, false.
        public bool IsPublic => Value.IsPublic;
        //
        // Summary:
        //     Gets a value indicating whether the field is private.
        //
        // Returns:
        //     true if the field is private; otherwise; false.
        public bool IsPrivate => Value.IsPrivate;
        //
        // Summary:
        //     Gets a value indicating whether the visibility of this field is described by
        //     System.Reflection.FieldAttributes.Family; that is, the field is visible only
        //     within its class and derived classes.
        //
        // Returns:
        //     true if access to this field is exactly described by System.Reflection.FieldAttributes.Family;
        //     otherwise, false.
        public bool IsFamily => Value.IsFamily;
        //
        // Summary:
        //     Gets a value indicating whether the potential visibility of this field is described
        //     by System.Reflection.FieldAttributes.Assembly; that is, the field is visible
        //     at most to other types in the same assembly, and is not visible to derived types
        //     outside the assembly.
        //
        // Returns:
        //     true if the visibility of this field is exactly described by System.Reflection.FieldAttributes.Assembly;
        //     otherwise, false.
        public bool IsAssembly => Value.IsAssembly;
        //
        // Summary:
        //     Gets a value indicating whether the visibility of this field is described by
        //     System.Reflection.FieldAttributes.FamANDAssem; that is, the field can be accessed
        //     from derived classes, but only if they are in the same assembly.
        //
        // Returns:
        //     true if access to this field is exactly described by System.Reflection.FieldAttributes.FamANDAssem;
        //     otherwise, false.
        public bool IsFamilyAndAssembly => IsFamilyAndAssembly;
        //
        // Summary:
        //     Gets a value indicating whether the potential visibility of this field is described
        //     by System.Reflection.FieldAttributes.FamORAssem; that is, the field can be accessed
        //     by derived classes wherever they are, and by classes in the same assembly.
        //
        // Returns:
        //     true if access to this field is exactly described by System.Reflection.FieldAttributes.FamORAssem;
        //     otherwise, false.
        public bool IsFamilyOrAssembly => Value.IsFamilyOrAssembly;
        //
        // Summary:
        //     Gets a value indicating whether the field is static.
        //
        // Returns:
        //     true if this field is static; otherwise, false.
        public bool IsStatic => Value.IsStatic;
        //
        // Summary:
        //     Gets a value indicating whether the field can only be set in the body of the
        //     constructor.
        //
        // Returns:
        //     true if the field has the InitOnly attribute set; otherwise, false.
        public bool IsInitOnly => Value.IsInitOnly;
        //
        // Summary:
        //     Gets a value indicating whether the value is written at compile time and cannot
        //     be changed.
        //
        // Returns:
        //     true if the field has the Literal attribute set; otherwise, false.
        public bool IsLiteral => Value.IsLiteral;
        //
        // Summary:
        //     Gets a value indicating whether this field has the NotSerialized attribute.
        //
        // Returns:
        //     true if the field has the NotSerialized attribute set; otherwise, false.
        public bool IsNotSerialized => Value.IsNotSerialized;
        //
        // Summary:
        //     Gets a value indicating whether the corresponding SpecialName attribute is set
        //     in the System.Reflection.FieldAttributes enumerator.
        //
        // Returns:
        //     true if the SpecialName attribute is set in System.Reflection.FieldAttributes;
        //     otherwise, false.
        public bool IsSpecialName => Value.IsSpecialName;
        //
        // Summary:
        //     Gets a value indicating whether the corresponding PinvokeImpl attribute is set
        //     in System.Reflection.FieldAttributes.
        //
        // Returns:
        //     true if the PinvokeImpl attribute is set in System.Reflection.FieldAttributes;
        //     otherwise, false.
        public bool IsPinvokeImpl => Value.IsPinvokeImpl;
        //
        // Summary:
        //     Gets a value that indicates whether the current field is security-critical or
        //     security-safe-critical at the current trust level.
        //
        // Returns:
        //     true if the current field is security-critical or security-safe-critical at the
        //     current trust level; false if it is transparent.
        public bool IsSecurityCritical => Value.IsSecurityCritical;
        //
        // Summary:
        //     Gets the attributes associated with this field.
        //
        // Returns:
        //     The FieldAttributes for this field.
        public FieldAttributes Attributes => Value.Attributes;
        //
        // Summary:
        //     Gets the type of this field object.
        //
        // Returns:
        //     The type of this field object.
        public TypeProxy FieldType => Value.FieldType;
        //
        // Summary:
        //     Gets a value that indicates whether the current field is transparent at the current
        //     trust level.
        //
        // Returns:
        //     true if the field is security-transparent at the current trust level; otherwise,
        //     false.
        public bool IsSecurityTransparent => Value.IsSecurityTransparent;
        //
        // Summary:
        //     Gets a value that indicates whether the current field is security-safe-critical
        //     at the current trust level.
        //
        // Returns:
        //     true if the current field is security-safe-critical at the current trust level;
        //     false if it is security-critical or transparent.
        public bool IsSecuritySafeCritical => Value.IsSecuritySafeCritical;

        public object GetValue(object parentObject)
            => Value.GetValue(parentObject);
        public void SetValue(object parentObject, object memberObject)
            => Value.SetValue(parentObject, memberObject);
        public object GetRawConstantValue() 
            => Value.GetRawConstantValue();
    }
}
