using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Reflection.Proxies;

namespace TheraEngine.Core.Reflection
{
    public abstract class MemberInfoProxy : MarshalByRefObject
    {
        public static MemberInfoProxy Get(MemberInfo info)
        {
            switch (info)
            {
                case FieldInfo fieldInfo:
                    return FieldInfoProxy.Get(fieldInfo);
                case PropertyInfo propertyInfo:
                    return PropertyInfoProxy.Get(propertyInfo);
                case EventInfo eventInfo:
                    return EventInfoProxy.Get(eventInfo);
                default:
                    return null;
            }
        }

        private MemberInfo Value { get; set; }
        public AppDomain Domain => AppDomain.CurrentDomain;

        //public MemberInfoProxy() { }
        protected MemberInfoProxy(MemberInfo value) => Value = value;

        //
        // Summary:
        //     When overridden in a derived class, gets a System.Reflection.MemberTypes value
        //     indicating the type of the member — method, constructor, event, and so on.
        //
        // Returns:
        //     A System.Reflection.MemberTypes value indicating the type of member.
        public MemberTypes MemberType => Value.MemberType;
        //
        // Summary:
        //     Gets the name of the current member.
        //
        // Returns:
        //     A System.String containing the name of this member.
        public string Name => Value.Name;
        //
        // Summary:
        //     Gets the class that declares this member.
        //
        // Returns:
        //     The Type object for the class that declares this member.
        public TypeProxy DeclaringType => Value.DeclaringType;
        //
        // Summary:
        //     Gets the class object that was used to obtain this instance of MemberInfo.
        //
        // Returns:
        //     The Type object through which this MemberInfo object was obtained.
        public TypeProxy ReflectedType => Value.ReflectedType;
        //
        // Summary:
        //     Gets a collection that contains this member's custom attributes.
        //
        // Returns:
        //     A collection that contains this member's custom attributes.
        public IEnumerable<CustomAttributeData> CustomAttributes => Value.CustomAttributes;
        //
        // Summary:
        //     Gets a value that identifies a metadata element.
        //
        // Returns:
        //     A value which, in combination with System.Reflection.MemberInfo.Module, uniquely
        //     identifies a metadata element.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current System.Reflection.MemberInfo represents an array method, such as
        //     Address, on an array type whose element type is a dynamic type that has not been
        //     completed. To get a metadata token in this case, pass the System.Reflection.MemberInfo
        //     object to the System.Reflection.Emit.ModuleBuilder.GetMethodToken(System.Reflection.MethodInfo)
        //     method; or use the System.Reflection.Emit.ModuleBuilder.GetArrayMethodToken(System.Type,System.String,System.Reflection.CallingConventions,System.Type,System.Type[])
        //     method to get the token directly, instead of using the System.Reflection.Emit.ModuleBuilder.GetArrayMethod(System.Type,System.String,System.Reflection.CallingConventions,System.Type,System.Type[])
        //     method to get a System.Reflection.MethodInfo first.
        public int MetadataToken => Value.MetadataToken;
        //
        // Summary:
        //     Gets the module in which the type that declares the member represented by the
        //     current System.Reflection.MemberInfo is defined.
        //
        // Returns:
        //     The System.Reflection.Module in which the type that declares the member represented
        //     by the current System.Reflection.MemberInfo is defined.
        //
        // Exceptions:
        //   T:System.NotImplementedException:
        //     This method is not implemented.
        //public ModuleProxy Module => Value.Module;

        //
        // Summary:
        //     Returns a value that indicates whether this instance is equal to a specified
        //     object.
        //
        // Parameters:
        //   obj:
        //     An object to compare with this instance, or null.
        //
        // Returns:
        //     true if obj equals the type and value of this instance; otherwise, false.
        public override bool Equals(object obj) =>
            obj is MemberInfoProxy prox && Value.Equals(prox.Value);
        //
        // Summary:
        //     When overridden in a derived class, returns an array of all custom attributes
        //     applied to this member.
        //
        // Parameters:
        //   inherit:
        //     true to search this member's inheritance chain to find the attributes; otherwise,
        //     false. This parameter is ignored for properties and events; see Remarks.
        //
        // Returns:
        //     An array that contains all the custom attributes applied to this member, or an
        //     array with zero elements if no attributes are defined.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     This member belongs to a type that is loaded into the reflection-only context.
        //     See How to: Load Assemblies into the Reflection-Only Context.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type could not be loaded.
        public object[] GetCustomAttributes(bool inherit)
            => Value.GetCustomAttributes(inherit);
        //
        // Summary:
        //     When overridden in a derived class, returns an array of custom attributes applied
        //     to this member and identified by System.Type.
        //
        // Parameters:
        //   attributeType:
        //     The type of attribute to search for. Only attributes that are assignable to this
        //     type are returned.
        //
        //   inherit:
        //     true to search this member's inheritance chain to find the attributes; otherwise,
        //     false. This parameter is ignored for properties and events; see Remarks.
        //
        // Returns:
        //     An array of custom attributes applied to this member, or an array with zero elements
        //     if no attributes assignable to attributeType have been applied.
        //
        // Exceptions:
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        //
        //   T:System.ArgumentNullException:
        //     If attributeType is null.
        //
        //   T:System.InvalidOperationException:
        //     This member belongs to a type that is loaded into the reflection-only context.
        //     See How to: Load Assemblies into the Reflection-Only Context.
        public object[] GetCustomAttributes(TypeProxy attributeType, bool inherit)
            => Value.GetCustomAttributes((Type)attributeType, inherit);
        //
        // Summary:
        //     When overridden in a derived class, returns an array of custom attributes applied
        //     to this member and identified by System.Type.
        //
        // Parameters:
        //   attributeType:
        //     The type of attribute to search for. Only attributes that are assignable to this
        //     type are returned.
        //
        //   inherit:
        //     true to search this member's inheritance chain to find the attributes; otherwise,
        //     false. This parameter is ignored for properties and events; see Remarks.
        //
        // Returns:
        //     An array of custom attributes applied to this member, or an array with zero elements
        //     if no attributes assignable to attributeType have been applied.
        //
        // Exceptions:
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        //
        //   T:System.ArgumentNullException:
        //     If attributeType is null.
        //
        //   T:System.InvalidOperationException:
        //     This member belongs to a type that is loaded into the reflection-only context.
        //     See How to: Load Assemblies into the Reflection-Only Context.
        public object[] GetCustomAttributes(Type attributeType, bool inherit)
            => Value.GetCustomAttributes(attributeType, inherit);
        //
        // Summary:
        //     Returns a list of System.Reflection.CustomAttributeData objects representing
        //     data about the attributes that have been applied to the target member.
        //
        // Returns:
        //     A generic list of System.Reflection.CustomAttributeData objects representing
        //     data about the attributes that have been applied to the target member.
        public IList<CustomAttributeData> GetCustomAttributesData()
            => Value.GetCustomAttributesData();
        //
        // Summary:
        //     Returns the hash code for this instance.
        //
        // Returns:
        //     A 32-bit signed integer hash code.
        public override int GetHashCode()
            => Value.GetHashCode();
        //
        // Summary:
        //     When overridden in a derived class, indicates whether one or more attributes
        //     of the specified type or of its derived types is applied to this member.
        //
        // Parameters:
        //   attributeType:
        //     The type of custom attribute to search for. The search includes derived types.
        //
        //   inherit:
        //     true to search this member's inheritance chain to find the attributes; otherwise,
        //     false. This parameter is ignored for properties and events; see Remarks.
        //
        // Returns:
        //     true if one or more instances of attributeType or any of its derived types is
        //     applied to this member; otherwise, false.
        public bool IsDefined(Type attributeType, bool inherit)
            => Value.IsDefined(attributeType, inherit);

        //
        // Summary:
        //     Indicates whether two System.Reflection.MemberInfo objects are equal.
        //
        // Parameters:
        //   left:
        //     The System.Reflection.MemberInfo to compare to right.
        //
        //   right:
        //     The System.Reflection.MemberInfo to compare to left.
        //
        // Returns:
        //     true if left is equal to right; otherwise false.
        public static bool operator ==(MemberInfoProxy left, MemberInfoProxy right)
            => left is null ? right is null : left.EqualTo(right);
        //
        // Summary:
        //     Indicates whether two System.Reflection.MemberInfo objects are not equal.
        //
        // Parameters:
        //   left:
        //     The System.Reflection.MemberInfo to compare to right.
        //
        //   right:
        //     The System.Reflection.MemberInfo to compare to left.
        //
        // Returns:
        //     true if left is not equal to right; otherwise false.
        public static bool operator !=(MemberInfoProxy left, MemberInfoProxy right)
           => left is null ? !(right is null) : !left.EqualTo(right);
        public bool EqualTo(MemberInfoProxy other)
        {
            if (other is null)
                return false;

            if (other.Domain.IsGameDomain() && !Domain.IsGameDomain())
                return other.EqualTo(this);

            return Value == other.Value;
        }
        public bool EqualTo(MemberInfo other)
        {
            if (other is null)
                return false;

            return Value == other;
        }
        //
        // Summary:
        //     Retrieves a custom attribute of a specified type that is applied to a specified
        //     member, and optionally inspects the ancestors of that member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        //   inherit:
        //     true to inspect the ancestors of element; otherwise, false.
        //
        // Returns:
        //     A custom attribute that matches attributeType, or null if no such attribute is
        //     found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one of the requested attributes was found.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public Attribute GetCustomAttribute(TypeProxy attributeType, bool inherit)
            => Value.GetCustomAttribute((Type)attributeType, inherit);
        //
        // Summary:
        //     Retrieves a custom attribute of a specified type that is applied to a specified
        //     member, and optionally inspects the ancestors of that member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        //   inherit:
        //     true to inspect the ancestors of element; otherwise, false.
        //
        // Returns:
        //     A custom attribute that matches attributeType, or null if no such attribute is
        //     found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one of the requested attributes was found.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public Attribute GetCustomAttribute(Type attributeType, bool inherit)
            => Value.GetCustomAttribute(attributeType, inherit);
        //
        // Summary:
        //     Retrieves a custom attribute of a specified type that is applied to a specified
        //     member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        // Type parameters:
        //   T:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A custom attribute that matches T, or null if no such attribute is found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element is null.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one of the requested attributes was found.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public T GetCustomAttribute<T>() where T : Attribute
            => Value.GetCustomAttribute<T>();
        //
        // Summary:
        //     Retrieves a custom attribute of a specified type that is applied to a specified
        //     member, and optionally inspects the ancestors of that member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   inherit:
        //     true to inspect the ancestors of element; otherwise, false.
        //
        // Type parameters:
        //   T:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A custom attribute that matches T, or null if no such attribute is found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element is null.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one of the requested attributes was found.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public T GetCustomAttribute<T>(bool inherit) where T : Attribute
            => Value.GetCustomAttribute<T>(inherit);
        //
        // Summary:
        //     Retrieves a custom attribute of a specified type that is applied to a specified
        //     member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A custom attribute that matches attributeType, or null if no such attribute is
        //     found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one of the requested attributes was found.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public Attribute GetCustomAttribute(Type attributeType)
            => Value.GetCustomAttribute(attributeType);
        //
        // Summary:
        //     Retrieves a custom attribute of a specified type that is applied to a specified
        //     member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A custom attribute that matches attributeType, or null if no such attribute is
        //     found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one of the requested attributes was found.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public Attribute GetCustomAttribute(TypeProxy attributeType)
            => Value.GetCustomAttribute((Type)attributeType);
        //
        // Summary:
        //     Retrieves a collection of custom attributes of a specified type that are applied
        //     to a specified member, and optionally inspects the ancestors of that member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   inherit:
        //     true to inspect the ancestors of element; otherwise, false.
        //
        // Type parameters:
        //   T:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A collection of the custom attributes that are applied to element and that match
        //     T, or an empty collection if no such attributes exist.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element is null.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public virtual T[] GetCustomAttributes<T>(bool inherit) where T : Attribute
            => Value.GetCustomAttributes<T>(inherit).ToArray();
        //
        // Summary:
        //     Retrieves a collection of custom attributes of a specified type that are applied
        //     to a specified member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        // Type parameters:
        //   T:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A collection of the custom attributes that are applied to element and that match
        //     T, or an empty collection if no such attributes exist.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element is null.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public virtual ProxyList<T> GetCustomAttributes<T>() where T : Attribute
            => new ProxyList<T>(Value.GetCustomAttributes<T>());
        //
        // Summary:
        //     Retrieves a collection of custom attributes that are applied to a specified member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        // Returns:
        //     A collection of the custom attributes that are applied to element, or an empty
        //     collection if no such attributes exist.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element is null.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public IEnumerable<Attribute> GetCustomAttributes()
            => Value.GetCustomAttributes();
        //
        // Summary:
        //     Retrieves a collection of custom attributes of a specified type that are applied
        //     to a specified member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A collection of the custom attributes that are applied to element and that match
        //     attributeType, or an empty collection if no such attributes exist.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public IEnumerable<Attribute> GetCustomAttributes(Type attributeType)
            => Value.GetCustomAttributes(attributeType);
        //
        // Summary:
        //     Retrieves a collection of custom attributes of a specified type that are applied
        //     to a specified member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        // Returns:
        //     A collection of the custom attributes that are applied to element and that match
        //     attributeType, or an empty collection if no such attributes exist.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        //
        //   T:System.TypeLoadException:
        //     A custom attribute type cannot be loaded.
        public IEnumerable<Attribute> GetCustomAttributes(TypeProxy attributeType)
            => Value.GetCustomAttributes((Type)attributeType);
        //
        // Summary:
        //     Indicates whether custom attributes of a specified type are applied to a specified
        //     assembly.
        //
        // Parameters:
        //   element:
        //     The assembly to inspect.
        //
        //   attributeType:
        //     The type of the attribute to search for.
        //
        // Returns:
        //     true if an attribute of the specified type is applied to element; otherwise,
        //     false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        public bool IsAttributeDefined(Type attributeType)
            => Value.IsDefined(attributeType);
        //
        // Summary:
        //     Indicates whether custom attributes of a specified type are applied to a specified
        //     member.
        //
        // Parameters:
        //   element:
        //     The member to inspect.
        //
        //   attributeType:
        //     The type of attribute to search for.
        //
        // Returns:
        //     true if an attribute of the specified type is applied to element; otherwise,
        //     false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     element or attributeType is null.
        //
        //   T:System.ArgumentException:
        //     attributeType is not derived from System.Attribute.
        //
        //   T:System.NotSupportedException:
        //     element is not a constructor, method, property, event, type, or field.
        public bool IsDefined(TypeProxy attributeType)
            => Value.IsDefined((Type)attributeType);
    }
}
