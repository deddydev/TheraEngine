using AppDomainToolkit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection.Proxies;

namespace TheraEngine.Core.Reflection
{
    /// <summary>
    /// Remotable interface for accessing information of a type.
    /// </summary>
    public sealed class TypeProxy : MemberInfoProxy
    {
        public static ConcurrentDictionary<Type, TypeProxy> Proxies { get; }
            = new ConcurrentDictionary<Type, TypeProxy>();
        public static TypeProxy Get(Type type)
            => type == null ? null : Proxies.GetOrAdd(type, new TypeProxy(type));

        public static implicit operator TypeProxy(Type type) => Get(type);
        public static explicit operator Type(TypeProxy proxy) => proxy?.Value;

        private Type Value { get; set; }

        //public TypeProxy() { }
        private TypeProxy(Type value) : base(value) => Value = value;

        public string GetFriendlyName(string openBracket = "<", string closeBracket = ">") 
            => Value.GetFriendlyName(openBracket, closeBracket);
        public object GetDefaultValue()
            => Value.GetDefaultValue();
        public TypeProxy DetermineElementType()
            => Value.DetermineElementType();

        public delegate TypeProxy DelTypeCreationFail(string typeDeclaration);
        public static DelTypeCreationFail TypeCreationFailed;
        public static TypeProxy CreateType(string typeDeclaration)
        {
            try
            {
                AssemblyQualifiedName asmQualName = new AssemblyQualifiedName(typeDeclaration);
                string asmName = asmQualName.AssemblyName;
                //var domains = Engine.EnumAppDomains();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies(); //domains.SelectMany(x => x.GetAssemblies());

                //Assembly asm = Assembly.Load(asmQualName.AssemblyName);
                //Version version = asm.GetName().Version;
                //asmQualName.VersionMinor = version.Minor;
                //asmQualName.VersionBuild = version.Build;
                //asmQualName.VersionRevision = version.Revision;

                //AssemblyName asmName = asmQualName.GetAssemblyName();

                //typeDeclaration = asmQualName.ToString();

                return TypeProxy.GetType(typeDeclaration,
                    name => assemblies.FirstOrDefault(assembly => assembly.GetName().Name.EqualsInvariantIgnoreCase(name.Name)),
                    null,
                    true);
            }
            catch// (Exception ex)
            {
                TypeProxy type = TypeCreationFailed?.Invoke(typeDeclaration);
                if (type is null)
                {
                    Debug.Print("Unable to create type " + typeDeclaration);
                    //Engine.LogException(ex);
                }
                else
                    return type;
            }
            return null;
        }
        public void GetGenericParameterConstraints(out EGenericVarianceFlag gvf, out ETypeConstraintFlag tcf)
        {
            GenericParameterAttributes gpa = GenericParameterAttributes;
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

        public bool FitsConstraints(EGenericVarianceFlag gvf, ETypeConstraintFlag tcf) 
            => Value.FitsConstraints(gvf, tcf);

        //
        // Summary:
        //     Gets a combination of System.Reflection.GenericParameterAttributes flags that
        //     describe the covariance and special constraints of the current generic type parameter.
        //
        // Returns:
        //     A bitwise combination of System.Reflection.GenericParameterAttributes values
        //     that describes the covariance and special constraints of the current generic
        //     type parameter.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current System.Type object is not a generic type parameter. That is, the
        //     System.Type.IsGenericParameter property returns false.
        //
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class.
        public GenericParameterAttributes GenericParameterAttributes => Value.GenericParameterAttributes;

        public Delegate CreateDelegate(MethodInfoProxy m)
            => Delegate.CreateDelegate(Value, (MethodInfo)m);

        //
        // Summary:
        //     Gets a value indicating whether the System.Type can be accessed by code outside
        //     the assembly.
        //
        // Returns:
        //     true if the current System.Type is a public type or a public nested type such
        //     that all the enclosing types are public; otherwise, false.
        public bool IsVisible => Value.IsVisible;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is not declared public.
        //
        // Returns:
        //     true if the System.Type is not declared public and is not a nested type; otherwise,
        //     false.
        public bool IsNotPublic => Value.IsNotPublic;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is declared public.
        //
        // Returns:
        //     true if the System.Type is declared public and is not a nested type; otherwise,
        //     false.
        public bool IsPublic => Value.IsPublic;
        //
        // Summary:
        //     Gets a value indicating whether a class is nested and declared public.
        //
        // Returns:
        //     true if the class is nested and declared public; otherwise, false.
        public bool IsNestedPublic => Value.IsNestedPublic;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is nested and visible only within
        //     its own assembly.
        //
        // Returns:
        //     true if the System.Type is nested and visible only within its own assembly; otherwise,
        //     false.
        public bool IsNestedAssembly => Value.IsNestedAssembly;

        public static TypeProxy TypeOf<T>()
        {
            TypeProxy proxy = null;
            var domains = AppDomainHelper.AppDomains;
            foreach (AppDomain domain in domains)
            {
                if (domain == AppDomain.CurrentDomain)
                {
                    try
                    {
                        proxy = Get(typeof(T));
                    }
                    catch
                    {
                        proxy = null;
                    }
                }
                else
                {
                    proxy = RemoteFunc.Invoke(domain, () =>
                    {
                        try
                        {
                            return Get(typeof(T));
                        }
                        catch
                        {
                            return null;
                        }
                    });
                }

                if (!(proxy is null))
                    break;
            }
            return proxy;
        }

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is nested and visible only within
        //     its own family.
        //
        // Returns:
        //     true if the System.Type is nested and visible only within its own family; otherwise,
        //     false.
        public bool IsNestedFamily => Value.IsNestedFamily;
        //
        // Summary:
        //     Gets the attributes associated with the System.Type.
        //
        // Returns:
        //     A System.Reflection.TypeAttributes object representing the attribute set of the
        //     System.Type, unless the System.Type represents a generic type parameter, in which
        //     case the value is unspecified.
        public TypeAttributes Attributes => Value.Attributes;

        public TypeCode GetTypeCode() => Type.GetTypeCode(Value);

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is nested and visible only to
        //     classes that belong to both its own family and its own assembly.
        //
        // Returns:
        //     true if the System.Type is nested and visible only to classes that belong to
        //     both its own family and its own assembly; otherwise, false.
        public bool IsNestedFamANDAssem => Value.IsNestedFamANDAssem;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is nested and visible only to
        //     classes that belong to either its own family or to its own assembly.
        //
        // Returns:
        //     true if the System.Type is nested and visible only to classes that belong to
        //     its own family or to its own assembly; otherwise, false.
        public bool IsNestedFamORAssem => Value.IsNestedFamORAssem;
        //
        // Summary:
        //     Gets a value indicating whether the fields of the current type are laid out automatically
        //     by the common language runtime.
        //
        // Returns:
        //     true if the System.Type.Attributes property of the current type includes System.Reflection.TypeAttributes.AutoLayout;
        //     otherwise, false.
        public bool IsAutoLayout => Value.IsAutoLayout;

        public bool HasCustomAttribute<T>() where T : Attribute => GetCustomAttribute<T>() != null;
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
        public override T[] GetCustomAttributes<T>(bool inherit)
            => Value.GetCustomAttributesExt<T>().ToArray();
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
        public override ProxyList<T> GetCustomAttributes<T>()
            => new ProxyList<T>(Value.GetCustomAttributesExt<T>());

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is nested and declared private.
        //
        // Returns:
        //     true if the System.Type is nested and declared private; otherwise, false.
        public bool IsNestedPrivate => Value.IsNestedPrivate;
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type object represents a type
        //     whose definition is nested inside the definition of another type.
        //
        // Returns:
        //     true if the System.Type is nested inside another type; otherwise, false.
        public bool IsNested => Value.IsNested;
        //
        // Summary:
        //     Gets the namespace of the System.Type.
        //
        // Returns:
        //     The namespace of the System.Type; null if the current instance has no namespace
        //     or represents a generic parameter.
        public string Namespace => Value.Namespace;
        //
        // Summary:
        //     Gets the type from which the current System.Type directly inherits.
        //
        // Returns:
        //     The System.Type from which the current System.Type directly inherits, or null
        //     if the current Type represents the System.Object class or an interface.
        public TypeProxy BaseType => Get(Value.BaseType);
        //
        // Summary:
        //     Gets the assembly-qualified name of the type, which includes the name of the
        //     assembly from which this System.Type object was loaded.
        //
        // Returns:
        //     The assembly-qualified name of the System.Type, which includes the name of the
        //     assembly from which the System.Type was loaded, or null if the current instance
        //     represents a generic type parameter.
        public string AssemblyQualifiedName => Value.AssemblyQualifiedName;
        //
        // Summary:
        //     Gets a value indicating whether the fields of the current type are laid out sequentially,
        //     in the order that they were defined or emitted to the metadata.
        //
        // Returns:
        //     true if the System.Type.Attributes property of the current type includes System.Reflection.TypeAttributes.SequentialLayout;
        //     otherwise, false.
        public bool IsLayoutSequential => Value.IsLayoutSequential;
        //
        // Summary:
        //     Gets the fully qualified name of the type, including its namespace but not its
        //     assembly.
        //
        // Returns:
        //     The fully qualified name of the type, including its namespace but not its assembly;
        //     or null if the current instance represents a generic type parameter, an array
        //     type, pointer type, or byref type based on a type parameter, or a generic type
        //     that is not a generic type definition but contains unresolved type parameters.
        public string FullName => Value.FullName;
        //
        // Summary:
        //     Gets the handle for the current System.Type.
        //
        // Returns:
        //     The handle for the current System.Type.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The .NET Compact Framework does not currently support this property.
        public RuntimeTypeHandle TypeHandle => Value.TypeHandle;
        //
        // Summary:
        //     Gets the System.Reflection.Assembly in which the type is declared. For generic
        //     types, gets the System.Reflection.Assembly in which the generic type is defined.
        //
        // Returns:
        //     An System.Reflection.Assembly instance that describes the assembly containing
        //     the current type. For generic types, the instance describes the assembly that
        //     contains the generic type definition, not the assembly that creates and uses
        //     a particular constructed type.
        public AssemblyProxy Assembly => AssemblyProxy.Get(Value.Assembly);
        //
        // Summary:
        //     Gets the module (the DLL) in which the current System.Type is defined.
        //
        // Returns:
        //     The module in which the current System.Type is defined.
        //public Module Module { get; }
        //
        // Summary:
        //     Gets the GUID associated with the System.Type.
        //
        // Returns:
        //     The GUID associated with the System.Type.
        public Guid GUID => Value.GUID;

        //
        // Summary:
        //     Gets a System.Runtime.InteropServices.StructLayoutAttribute that describes the
        //     layout of the current type.
        //
        // Returns:
        //     Gets a System.Runtime.InteropServices.StructLayoutAttribute that describes the
        //     gross layout features of the current type.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class.
        //public StructLayoutAttribute StructLayoutAttribute { get; }
        //
        // Summary:
        //     Gets the class object that was used to obtain this member.
        //
        // Returns:
        //     The Type object through which this System.Type object was obtained.
        //public TypeProxy ReflectedType => Get(Value.ReflectedType);

        //
        // Summary:
        //     Gets a System.Reflection.MethodBase that represents the declaring method, if
        //     the current System.Type represents a type parameter of a generic method.
        //
        // Returns:
        //     If the current System.Type represents a type parameter of a generic method, a
        //     System.Reflection.MethodBase that represents declaring method; otherwise, null.
        public MethodBaseProxy DeclaringMethod => MethodBaseProxy.Get(Value.DeclaringMethod);

        public object CreateInstance()
        {
            if (!AppDomainHelper.IsPrimaryDomain)
                Debug.Print("Creating instance on AppDomain " + Domain.FriendlyName);
            return SerializationCommon.CreateInstance(Value);
        }
        public object CreateInstance(params object[] args)
        {
            if (!AppDomainHelper.IsPrimaryDomain)
                Debug.Print("Creating instance on AppDomain " + Domain.FriendlyName);
            return SerializationCommon.CreateInstance(Value, args);
        }
        public TypeProxy GetUnderlyingNullableType() => Nullable.GetUnderlyingType(Value);
        public Array CreateArrayInstance(int length)
        {
            if (!AppDomainHelper.IsPrimaryDomain)
                Debug.Print("Creating array instance on AppDomain " + Domain.FriendlyName);
            return Array.CreateInstance(Value, length);
        }

        //
        // Summary:
        //     Gets the type that declares the current nested type or generic type parameter.
        //
        // Returns:
        //     A System.Type object representing the enclosing type, if the current type is
        //     a nested type; or the generic type definition, if the current type is a type
        //     parameter of a generic type; or the type that declares the generic method, if
        //     the current type is a type parameter of a generic method; otherwise, null.
        //public TypeProxy DeclaringType => Get(Value.DeclaringType);

        //
        // Summary:
        //     Gets the initializer for the type.
        //
        // Returns:
        //     An object that contains the name of the class constructor for the System.Type.
        [ComVisible(true)]
        public ConstructorInfoProxy TypeInitializer => ConstructorInfoProxy.Get(Value.TypeInitializer);

        //
        // Summary:
        //     Gets a value indicating whether the fields of the current type are laid out at
        //     explicitly specified offsets.
        //
        // Returns:
        //     true if the System.Type.Attributes property of the current type includes System.Reflection.TypeAttributes.ExplicitLayout;
        //     otherwise, false.
        public bool IsExplicitLayout => Value.IsExplicitLayout;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is a value type.
        //
        // Returns:
        //     true if the System.Type is a value type; otherwise, false.
        public bool IsValueType => Value.IsValueType;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is an interface; that is, not
        //     a class or a value type.
        //
        // Returns:
        //     true if the System.Type is an interface; otherwise, false.
        public bool IsInterface => Value.IsInterface;
        //
        // Summary:
        //     Gets a value that indicates whether the current type is security-safe-critical
        //     at the current trust level; that is, whether it can perform critical operations
        //     and can be accessed by transparent code.
        //
        // Returns:
        //     true if the current type is security-safe-critical at the current trust level;
        //     false if it is security-critical or transparent.
        public bool IsSecuritySafeCritical => Value.IsSecuritySafeCritical;
        //
        // Summary:
        //     Gets a value that indicates whether the current type is security-critical or
        //     security-safe-critical at the current trust level, and therefore can perform
        //     critical operations.
        //
        // Returns:
        //     true if the current type is security-critical or security-safe-critical at the
        //     current trust level; false if it is transparent.
        public bool IsSecurityCritical => Value.IsSecurityCritical;
        //
        // Summary:
        //     Gets an array of the generic type arguments for this type.
        //
        // Returns:
        //     An array of the generic type arguments for this type.
        public TypeProxy[] GenericTypeArguments => Value.GenericTypeArguments.Select(x => Get(x)).ToArray();

        public object ParseEnum(string value) => Enum.Parse(Value, value);

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is marshaled by reference.
        //
        // Returns:
        //     true if the System.Type is marshaled by reference; otherwise, false.
        public bool IsMarshalByRef => Value.IsMarshalByRef;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type can be hosted in a context.
        //
        // Returns:
        //     true if the System.Type can be hosted in a context; otherwise, false.
        public bool IsContextful => Value.IsContextful;
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type encompasses or refers
        //     to another type; that is, whether the current System.Type is an array, a pointer,
        //     or is passed by reference.
        //
        // Returns:
        //     true if the System.Type is an array, a pointer, or is passed by reference; otherwise,
        //     false.
        public bool HasElementType => Value.HasElementType;

        public object PtrToStructure(IntPtr address) => Marshal.PtrToStructure(address, Value);

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is a COM object.
        //
        // Returns:
        //     true if the System.Type is a COM object; otherwise, false.
        public bool IsCOMObject => Value.IsCOMObject;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is one of the primitive types.
        //
        // Returns:
        //     true if the System.Type is one of the primitive types; otherwise, false.
        public bool IsPrimitive => Value.IsPrimitive;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is a pointer.
        //
        // Returns:
        //     true if the System.Type is a pointer; otherwise, false.
        public bool IsPointer => Value.IsPointer;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is passed by reference.
        //
        // Returns:
        //     true if the System.Type is passed by reference; otherwise, false.
        public bool IsByRef => Value.IsByRef;
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type object has type parameters
        //     that have not been replaced by specific types.
        //
        // Returns:
        //     true if the System.Type object is itself a generic type parameter or has type
        //     parameters for which specific types have not been supplied; otherwise, false.
        public bool ContainsGenericParameters => Value.ContainsGenericParameters;
        //
        // Summary:
        //     Gets the position of the type parameter in the type parameter list of the generic
        //     type or method that declared the parameter, when the System.Type object represents
        //     a type parameter of a generic type or a generic method.
        //
        // Returns:
        //     The position of a type parameter in the type parameter list of the generic type
        //     or method that defines the parameter. Position numbers begin at 0.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current type does not represent a type parameter. That is, System.Type.IsGenericParameter
        //     returns false.
        public int GenericParameterPosition => Value.GenericParameterPosition;
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type represents a type parameter
        //     in the definition of a generic type or method.
        //
        // Returns:
        //     true if the System.Type object represents a type parameter of a generic type
        //     definition or generic method definition; otherwise, false.
        public bool IsGenericParameter => Value.IsGenericParameter;
        //
        // Summary:
        //     Gets a value that indicates whether this object represents a constructed generic
        //     type. You can create instances of a constructed generic type.
        //
        // Returns:
        //     true if this object represents a constructed generic type; otherwise, false.
        public bool IsConstructedGenericType => Value.IsConstructedGenericType;
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type represents a generic
        //     type definition, from which other generic types can be constructed.
        //
        // Returns:
        //     true if the System.Type object represents a generic type definition; otherwise,
        //     false.
        public bool IsGenericTypeDefinition => Value.IsGenericTypeDefinition;
        //
        // Summary:
        //     Gets a value indicating whether the current type is a generic type.
        //
        // Returns:
        //     true if the current type is a generic type; otherwise, false.
        public bool IsGenericType => Value.IsGenericType;
        //
        // Summary:
        //     Gets a value that indicates whether the type is an array.
        //
        // Returns:
        //     true if the current type is an array; otherwise, false.
        public bool IsArray => Value.IsArray;
        //
        // Summary:
        //     Gets a value indicating whether the string format attribute AutoClass is selected
        //     for the System.Type.
        //
        // Returns:
        //     true if the string format attribute AutoClass is selected for the System.Type;
        //     otherwise, false.
        public bool IsAutoClass => Value.IsAutoClass;
        //
        // Summary:
        //     Gets a value indicating whether the string format attribute UnicodeClass is selected
        //     for the System.Type.
        //
        // Returns:
        //     true if the string format attribute UnicodeClass is selected for the System.Type;
        //     otherwise, false.
        public bool IsUnicodeClass => Value.IsUnicodeClass;
        //
        // Summary:
        //     Gets a value indicating whether the string format attribute AnsiClass is selected
        //     for the System.Type.
        //
        // Returns:
        //     true if the string format attribute AnsiClass is selected for the System.Type;
        //     otherwise, false.
        public bool IsAnsiClass => Value.IsAnsiClass;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is serializable.
        //
        // Returns:
        //     true if the System.Type is serializable; otherwise, false.
        public bool IsSerializable => Value.IsSerializable;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type has a System.Runtime.InteropServices.ComImportAttribute
        //     attribute applied, indicating that it was imported from a COM type library.
        //
        // Returns:
        //     true if the System.Type has a System.Runtime.InteropServices.ComImportAttribute;
        //     otherwise, false.
        public bool IsImport => Value.IsImport;
        //
        // Summary:
        //     Gets a value indicating whether the type has a name that requires special handling.
        //
        // Returns:
        //     true if the type has a name that requires special handling; otherwise, false.
        public bool IsSpecialName => Value.IsSpecialName;
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type represents an enumeration.
        //
        // Returns:
        //     true if the current System.Type represents an enumeration; otherwise, false.
        public bool IsEnum => Value.IsEnum;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is declared sealed.
        //
        // Returns:
        //     true if the System.Type is declared sealed; otherwise, false.
        public bool IsSealed => Value.IsSealed;
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is abstract and must be overridden.
        //
        // Returns:
        //     true if the System.Type is abstract; otherwise, false.
        public bool IsAbstract => Value.IsAbstract;

        //
        // Summary:
        //     Gets a System.Reflection.MemberTypes value indicating that this member is a type
        //     or a nested type.
        //
        // Returns:
        //     A System.Reflection.MemberTypes value indicating that this member is a type or
        //     a nested type.
        //public MemberTypes MemberType => Value.MemberType;

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is a class or a delegate; that
        //     is, not a value type or interface.
        //
        // Returns:
        //     true if the System.Type is a class; otherwise, false.
        public bool IsClass => Value.IsClass;
        //
        // Summary:
        //     Gets a value that indicates whether the current type is transparent at the current
        //     trust level, and therefore cannot perform critical operations.
        //
        // Returns:
        //     true if the type is security-transparent at the current trust level; otherwise,
        //     false.
        public bool IsSecurityTransparent => Value.IsSecurityTransparent;
        //
        // Summary:
        //     Indicates the type provided by the common language runtime that represents this
        //     type.
        //
        // Returns:
        //     The underlying system type for the System.Type.
        public TypeProxy UnderlyingSystemType => Get(Value.UnderlyingSystemType);

        //
        // Summary:
        //     Gets the System.Type with the specified name, performing a case-sensitive search.
        //
        // Parameters:
        //   typeName:
        //     The assembly-qualified name of the type to get. See System.Type.AssemblyQualifiedName.
        //     If the type is in the currently executing assembly or in Mscorlib.dll, it is
        //     sufficient to supply the type name qualified by its namespace.
        //
        // Returns:
        //     The type with the specified name, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.ArgumentException:
        //     typeName represents a generic type that has a pointer type, a ByRef type, or
        //     System.Void as one of its type arguments.-or- typeName represents a generic type
        //     that has an incorrect number of type arguments.-or- typeName represents a generic
        //     type, and one of its type arguments does not satisfy the constraints for the
        //     corresponding type parameter.
        //
        //   T:System.TypeLoadException:
        //     typeName represents an array of System.TypedReference.
        //
        //   T:System.IO.FileLoadException:
        //     In the .NET for Windows Store apps or the Portable Class Library, catch the base
        //     class exception, System.IO.IOException, instead.The assembly or one of its dependencies
        //     was found, but could not be loaded.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-Version 2.0 or later
        //     of the common language runtime is currently loaded, and the assembly was compiled
        //     with a later version.
        public static TypeProxy GetType(string typeName) => Get(Type.GetType(typeName));

        //
        // Summary:
        //     Returns the System.Reflection.TypeInfo representation of the specified type.
        //
        // Parameters:
        //   type:
        //     The type to convert.
        //
        // Returns:
        //     The converted object.
        public TypeInfo GetTypeInfo() => Value.GetTypeInfo();

        //
        // Summary:
        //     Gets the type with the specified name, specifying whether to perform a case-sensitive
        //     search and whether to throw an exception if the type is not found, and optionally
        //     providing custom methods to resolve the assembly and the type.
        //
        // Parameters:
        //   typeName:
        //     The name of the type to get. If the typeResolver parameter is provided, the type
        //     name can be any string that typeResolver is capable of resolving. If the assemblyResolver
        //     parameter is provided or if standard type resolution is used, typeName must be
        //     an assembly-qualified name (see System.Type.AssemblyQualifiedName), unless the
        //     type is in the currently executing assembly or in Mscorlib.dll, in which case
        //     it is sufficient to supply the type name qualified by its namespace.
        //
        //   assemblyResolver:
        //     A method that locates and returns the assembly that is specified in typeName.
        //     The assembly name is passed to assemblyResolver as an System.Reflection.AssemblyName
        //     object. If typeName does not contain the name of an assembly, assemblyResolver
        //     is not called. If assemblyResolver is not supplied, standard assembly resolution
        //     is performed. Caution Do not pass methods from unknown or untrusted callers.
        //     Doing so could result in elevation of privilege for malicious code. Use only
        //     methods that you provide or that you are familiar with.
        //
        //   typeResolver:
        //     A method that locates and returns the type that is specified by typeName from
        //     the assembly that is returned by assemblyResolver or by standard assembly resolution.
        //     If no assembly is provided, the method can provide one. The method also takes
        //     a parameter that specifies whether to perform a case-insensitive search; the
        //     value of ignoreCase is passed to that parameter. Caution Do not pass methods
        //     from unknown or untrusted callers.
        //
        //   throwOnError:
        //     true to throw an exception if the type cannot be found; false to return null.
        //     Specifying false also suppresses some other exception conditions, but not all
        //     of them. See the Exceptions section.
        //
        //   ignoreCase:
        //     true to perform a case-insensitive search for typeName, false to perform a case-sensitive
        //     search for typeName.
        //
        // Returns:
        //     The type with the specified name. If the type is not found, the throwOnError
        //     parameter specifies whether null is returned or an exception is thrown. In some
        //     cases, an exception is thrown regardless of the value of throwOnError. See the
        //     Exceptions section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.TypeLoadException:
        //     throwOnError is true and the type is not found. -or- throwOnError is true and
        //     typeName contains invalid characters, such as an embedded tab.-or- throwOnError
        //     is true and typeName is an empty string.-or- throwOnError is true and typeName
        //     represents an array type with an invalid size. -or- typeName represents an array
        //     of System.TypedReference.
        //
        //   T:System.ArgumentException:
        //     An error occurs when typeName is parsed into a type name and an assembly name
        //     (for example, when the simple type name includes an unescaped special character).-or-
        //     throwOnError is true and typeName contains invalid syntax (for example, "MyType[,*,]").-or-
        //     typeName represents a generic type that has a pointer type, a ByRef type, or
        //     System.Void as one of its type arguments.-or- typeName represents a generic type
        //     that has an incorrect number of type arguments.-or- typeName represents a generic
        //     type, and one of its type arguments does not satisfy the constraints for the
        //     corresponding type parameter.
        //
        //   T:System.IO.FileNotFoundException:
        //     throwOnError is true and the assembly or one of its dependencies was not found.
        //
        //   T:System.IO.FileLoadException:
        //     The assembly or one of its dependencies was found, but could not be loaded. -or-
        //     typeName contains an invalid assembly name.-or- typeName is a valid assembly
        //     name without a type name.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-The assembly was compiled
        //     with a later version of the common language runtime than the version that is
        //     currently loaded.
        public static TypeProxy GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase)
            => Get(Type.GetType(typeName, assemblyResolver, typeResolver, throwOnError, ignoreCase));
        //
        // Summary:
        //     Gets the type with the specified name, specifying whether to throw an exception
        //     if the type is not found, and optionally providing custom methods to resolve
        //     the assembly and the type.
        //
        // Parameters:
        //   typeName:
        //     The name of the type to get. If the typeResolver parameter is provided, the type
        //     name can be any string that typeResolver is capable of resolving. If the assemblyResolver
        //     parameter is provided or if standard type resolution is used, typeName must be
        //     an assembly-qualified name (see System.Type.AssemblyQualifiedName), unless the
        //     type is in the currently executing assembly or in Mscorlib.dll, in which case
        //     it is sufficient to supply the type name qualified by its namespace.
        //
        //   assemblyResolver:
        //     A method that locates and returns the assembly that is specified in typeName.
        //     The assembly name is passed to assemblyResolver as an System.Reflection.AssemblyName
        //     object. If typeName does not contain the name of an assembly, assemblyResolver
        //     is not called. If assemblyResolver is not supplied, standard assembly resolution
        //     is performed. Caution Do not pass methods from unknown or untrusted callers.
        //     Doing so could result in elevation of privilege for malicious code. Use only
        //     methods that you provide or that you are familiar with.
        //
        //   typeResolver:
        //     A method that locates and returns the type that is specified by typeName from
        //     the assembly that is returned by assemblyResolver or by standard assembly resolution.
        //     If no assembly is provided, the method can provide one. The method also takes
        //     a parameter that specifies whether to perform a case-insensitive search; false
        //     is passed to that parameter. Caution Do not pass methods from unknown or untrusted
        //     callers.
        //
        //   throwOnError:
        //     true to throw an exception if the type cannot be found; false to return null.
        //     Specifying false also suppresses some other exception conditions, but not all
        //     of them. See the Exceptions section.
        //
        // Returns:
        //     The type with the specified name. If the type is not found, the throwOnError
        //     parameter specifies whether null is returned or an exception is thrown. In some
        //     cases, an exception is thrown regardless of the value of throwOnError. See the
        //     Exceptions section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.TypeLoadException:
        //     throwOnError is true and the type is not found. -or- throwOnError is true and
        //     typeName contains invalid characters, such as an embedded tab.-or- throwOnError
        //     is true and typeName is an empty string.-or- throwOnError is true and typeName
        //     represents an array type with an invalid size. -or- typeName represents an array
        //     of System.TypedReference.
        //
        //   T:System.ArgumentException:
        //     An error occurs when typeName is parsed into a type name and an assembly name
        //     (for example, when the simple type name includes an unescaped special character).-or-
        //     throwOnError is true and typeName contains invalid syntax (for example, "MyType[,*,]").-or-
        //     typeName represents a generic type that has a pointer type, a ByRef type, or
        //     System.Void as one of its type arguments.-or- typeName represents a generic type
        //     that has an incorrect number of type arguments.-or- typeName represents a generic
        //     type, and one of its type arguments does not satisfy the constraints for the
        //     corresponding type parameter.
        //
        //   T:System.IO.FileNotFoundException:
        //     throwOnError is true and the assembly or one of its dependencies was not found.
        //     -or- typeName contains an invalid assembly name.-or- typeName is a valid assembly
        //     name without a type name.
        //
        //   T:System.IO.FileLoadException:
        //     The assembly or one of its dependencies was found, but could not be loaded.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-The assembly was compiled
        //     with a later version of the common language runtime than the version that is
        //     currently loaded.
        public static TypeProxy GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError)
            => Get(Type.GetType(typeName, assemblyResolver, typeResolver, throwOnError));
        //
        // Summary:
        //     Gets the type with the specified name, optionally providing custom methods to
        //     resolve the assembly and the type.
        //
        // Parameters:
        //   typeName:
        //     The name of the type to get. If the typeResolver parameter is provided, the type
        //     name can be any string that typeResolver is capable of resolving. If the assemblyResolver
        //     parameter is provided or if standard type resolution is used, typeName must be
        //     an assembly-qualified name (see System.Type.AssemblyQualifiedName), unless the
        //     type is in the currently executing assembly or in Mscorlib.dll, in which case
        //     it is sufficient to supply the type name qualified by its namespace.
        //
        //   assemblyResolver:
        //     A method that locates and returns the assembly that is specified in typeName.
        //     The assembly name is passed to assemblyResolver as an System.Reflection.AssemblyName
        //     object. If typeName does not contain the name of an assembly, assemblyResolver
        //     is not called. If assemblyResolver is not supplied, standard assembly resolution
        //     is performed. Caution Do not pass methods from unknown or untrusted callers.
        //     Doing so could result in elevation of privilege for malicious code. Use only
        //     methods that you provide or that you are familiar with.
        //
        //   typeResolver:
        //     A method that locates and returns the type that is specified by typeName from
        //     the assembly that is returned by assemblyResolver or by standard assembly resolution.
        //     If no assembly is provided, the typeResolver method can provide one. The method
        //     also takes a parameter that specifies whether to perform a case-insensitive search;
        //     false is passed to that parameter. Caution Do not pass methods from unknown or
        //     untrusted callers.
        //
        // Returns:
        //     The type with the specified name, or null if the type is not found.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.ArgumentException:
        //     An error occurs when typeName is parsed into a type name and an assembly name
        //     (for example, when the simple type name includes an unescaped special character).-or-
        //     typeName represents a generic type that has a pointer type, a ByRef type, or
        //     System.Void as one of its type arguments.-or- typeName represents a generic type
        //     that has an incorrect number of type arguments.-or- typeName represents a generic
        //     type, and one of its type arguments does not satisfy the constraints for the
        //     corresponding type parameter.
        //
        //   T:System.TypeLoadException:
        //     typeName represents an array of System.TypedReference.
        //
        //   T:System.IO.FileLoadException:
        //     The assembly or one of its dependencies was found, but could not be loaded. -or-
        //     typeName contains an invalid assembly name.-or- typeName is a valid assembly
        //     name without a type name.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-The assembly was compiled
        //     with a later version of the common language runtime than the version that is
        //     currently loaded.
        public static TypeProxy GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver)
            => Get(Type.GetType(typeName, assemblyResolver, typeResolver));
        //
        // Summary:
        //     Gets the System.Type with the specified name, performing a case-sensitive search
        //     and specifying whether to throw an exception if the type is not found.
        //
        // Parameters:
        //   typeName:
        //     The assembly-qualified name of the type to get. See System.Type.AssemblyQualifiedName.
        //     If the type is in the currently executing assembly or in Mscorlib.dll, it is
        //     sufficient to supply the type name qualified by its namespace.
        //
        //   throwOnError:
        //     true to throw an exception if the type cannot be found; false to return null.
        //     Specifying false also suppresses some other exception conditions, but not all
        //     of them. See the Exceptions section.
        //
        // Returns:
        //     The type with the specified name. If the type is not found, the throwOnError
        //     parameter specifies whether null is returned or an exception is thrown. In some
        //     cases, an exception is thrown regardless of the value of throwOnError. See the
        //     Exceptions section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.TypeLoadException:
        //     throwOnError is true and the type is not found. -or- throwOnError is true and
        //     typeName contains invalid characters, such as an embedded tab.-or- throwOnError
        //     is true and typeName is an empty string.-or- throwOnError is true and typeName
        //     represents an array type with an invalid size. -or- typeName represents an array
        //     of System.TypedReference.
        //
        //   T:System.ArgumentException:
        //     throwOnError is true and typeName contains invalid syntax. For example, "MyType[,*,]".-or-
        //     typeName represents a generic type that has a pointer type, a ByRef type, or
        //     System.Void as one of its type arguments.-or- typeName represents a generic type
        //     that has an incorrect number of type arguments.-or- typeName represents a generic
        //     type, and one of its type arguments does not satisfy the constraints for the
        //     corresponding type parameter.
        //
        //   T:System.IO.FileNotFoundException:
        //     throwOnError is true and the assembly or one of its dependencies was not found.
        //
        //   T:System.IO.FileLoadException:
        //     In the .NET for Windows Store apps or the Portable Class Library, catch the base
        //     class exception, System.IO.IOException, instead.The assembly or one of its dependencies
        //     was found, but could not be loaded.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-Version 2.0 or later
        //     of the common language runtime is currently loaded, and the assembly was compiled
        //     with a later version.
        public static TypeProxy GetType(string typeName, bool throwOnError)
            => Get(Type.GetType(typeName, throwOnError));
        //
        // Summary:
        //     Gets the System.Type with the specified name, specifying whether to throw an
        //     exception if the type is not found and whether to perform a case-sensitive search.
        //
        // Parameters:
        //   typeName:
        //     The assembly-qualified name of the type to get. See System.Type.AssemblyQualifiedName.
        //     If the type is in the currently executing assembly or in Mscorlib.dll, it is
        //     sufficient to supply the type name qualified by its namespace.
        //
        //   throwOnError:
        //     true to throw an exception if the type cannot be found; false to return null.Specifying
        //     false also suppresses some other exception conditions, but not all of them. See
        //     the Exceptions section.
        //
        //   ignoreCase:
        //     true to perform a case-insensitive search for typeName, false to perform a case-sensitive
        //     search for typeName.
        //
        // Returns:
        //     The type with the specified name. If the type is not found, the throwOnError
        //     parameter specifies whether null is returned or an exception is thrown. In some
        //     cases, an exception is thrown regardless of the value of throwOnError. See the
        //     Exceptions section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.TypeLoadException:
        //     throwOnError is true and the type is not found. -or- throwOnError is true and
        //     typeName contains invalid characters, such as an embedded tab.-or- throwOnError
        //     is true and typeName is an empty string.-or- throwOnError is true and typeName
        //     represents an array type with an invalid size. -or- typeName represents an array
        //     of System.TypedReference.
        //
        //   T:System.ArgumentException:
        //     throwOnError is true and typeName contains invalid syntax. For example, "MyType[,*,]".-or-
        //     typeName represents a generic type that has a pointer type, a ByRef type, or
        //     System.Void as one of its type arguments.-or- typeName represents a generic type
        //     that has an incorrect number of type arguments.-or- typeName represents a generic
        //     type, and one of its type arguments does not satisfy the constraints for the
        //     corresponding type parameter.
        //
        //   T:System.IO.FileNotFoundException:
        //     throwOnError is true and the assembly or one of its dependencies was not found.
        //
        //   T:System.IO.FileLoadException:
        //     The assembly or one of its dependencies was found, but could not be loaded.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-Version 2.0 or later
        //     of the common language runtime is currently loaded, and the assembly was compiled
        //     with a later version.
        public static TypeProxy GetType(string typeName, bool throwOnError, bool ignoreCase)
            => Get(Type.GetType(typeName, throwOnError, ignoreCase));
        //
        // Summary:
        //     Gets the types of the objects in the specified array.
        //
        // Parameters:
        //   args:
        //     An array of objects whose types to determine.
        //
        // Returns:
        //     An array of System.Type objects representing the types of the corresponding elements
        //     in args.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     args is null. -or-One or more of the elements in args is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     The class initializers are invoked and at least one throws an exception.
        public static TypeProxy[] GetTypeArray(object[] args)
            => args.Select(x => Get(x?.GetType())).ToArray();
        //
        // Summary:
        //     Gets the underlying type code of the specified System.Type.
        //
        // Parameters:
        //   type:
        //     The type whose underlying type code to get.
        //
        // Returns:
        //     The code of the underlying type, or System.TypeCode.Empty if type is null.
        public static TypeCode GetTypeCode(TypeProxy type)
            => Type.GetTypeCode(type.Value);
        //
        // Summary:
        //     Gets the underlying type code of the specified System.Type.
        //
        // Parameters:
        //   type:
        //     The type whose underlying type code to get.
        //
        // Returns:
        //     The code of the underlying type, or System.TypeCode.Empty if type is null.
        public static TypeCode GetTypeCode(Type type)
            => Type.GetTypeCode(type);
        //
        // Summary:
        //     Gets the type associated with the specified class identifier (CLSID) from the
        //     specified server, specifying whether to throw an exception if an error occurs
        //     while loading the type.
        //
        // Parameters:
        //   clsid:
        //     The CLSID of the type to get.
        //
        //   server:
        //     The server from which to load the type. If the server name is null, this method
        //     automatically reverts to the local machine.
        //
        //   throwOnError:
        //     true to throw any exception that occurs.-or- false to ignore any exception that
        //     occurs.
        //
        // Returns:
        //     System.__ComObject regardless of whether the CLSID is valid.
        [SecuritySafeCritical]
        public static TypeProxy GetTypeFromCLSID(Guid clsid, string server, bool throwOnError)
            => Type.GetTypeFromCLSID(clsid, server, throwOnError);
        //
        // Summary:
        //     Gets the type associated with the specified class identifier (CLSID).
        //
        // Parameters:
        //   clsid:
        //     The CLSID of the type to get.
        //
        // Returns:
        //     System.__ComObject regardless of whether the CLSID is valid.
        [SecuritySafeCritical]
        public static TypeProxy GetTypeFromCLSID(Guid clsid)
            => Type.GetTypeFromCLSID(clsid);
        //
        // Summary:
        //     Gets the type associated with the specified class identifier (CLSID), specifying
        //     whether to throw an exception if an error occurs while loading the type.
        //
        // Parameters:
        //   clsid:
        //     The CLSID of the type to get.
        //
        //   throwOnError:
        //     true to throw any exception that occurs.-or- false to ignore any exception that
        //     occurs.
        //
        // Returns:
        //     System.__ComObject regardless of whether the CLSID is valid.
        [SecuritySafeCritical]
        public static TypeProxy GetTypeFromCLSID(Guid clsid, bool throwOnError)
            => Type.GetTypeFromCLSID(clsid, throwOnError);
        //
        // Summary:
        //     Gets the type associated with the specified class identifier (CLSID) from the
        //     specified server.
        //
        // Parameters:
        //   clsid:
        //     The CLSID of the type to get.
        //
        //   server:
        //     The server from which to load the type. If the server name is null, this method
        //     automatically reverts to the local machine.
        //
        // Returns:
        //     System.__ComObject regardless of whether the CLSID is valid.
        [SecuritySafeCritical]
        public static TypeProxy GetTypeFromCLSID(Guid clsid, string server)
            => Type.GetTypeFromCLSID(clsid, server);
        //
        // Summary:
        //     Gets the type referenced by the specified type handle.
        //
        // Parameters:
        //   handle:
        //     The object that refers to the type.
        //
        // Returns:
        //     The type referenced by the specified System.RuntimeTypeHandle, or null if the
        //     System.RuntimeTypeHandle.Value property of handle is null.
        //
        // Exceptions:
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        [SecuritySafeCritical]
        public static TypeProxy GetTypeFromHandle(RuntimeTypeHandle handle)
            => Type.GetTypeFromHandle(handle);
        //
        // Summary:
        //     Gets the type associated with the specified program identifier (ProgID), specifying
        //     whether to throw an exception if an error occurs while loading the type.
        //
        // Parameters:
        //   progID:
        //     The ProgID of the type to get.
        //
        //   throwOnError:
        //     true to throw any exception that occurs.-or- false to ignore any exception that
        //     occurs.
        //
        // Returns:
        //     The type associated with the specified program identifier (ProgID), if progID
        //     is a valid entry in the registry and a type is associated with it; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     progID is null.
        //
        //   T:System.Runtime.InteropServices.COMException:
        //     The specified ProgID is not registered.
        [SecurityCritical]
        public static TypeProxy GetTypeFromProgID(string progID, bool throwOnError)
            => Type.GetTypeFromProgID(progID, throwOnError);
        //
        // Summary:
        //     Gets the type associated with the specified program identifier (progID) from
        //     the specified server, returning null if an error is encountered while loading
        //     the type.
        //
        // Parameters:
        //   progID:
        //     The progID of the type to get.
        //
        //   server:
        //     The server from which to load the type. If the server name is null, this method
        //     automatically reverts to the local machine.
        //
        // Returns:
        //     The type associated with the specified program identifier (progID), if progID
        //     is a valid entry in the registry and a type is associated with it; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     prodID is null.
        [SecurityCritical]
        public static TypeProxy GetTypeFromProgID(string progID, string server)
            => Type.GetTypeFromProgID(progID, server);
        //
        // Summary:
        //     Gets the type associated with the specified program identifier (progID) from
        //     the specified server, specifying whether to throw an exception if an error occurs
        //     while loading the type.
        //
        // Parameters:
        //   progID:
        //     The progID of the System.Type to get.
        //
        //   server:
        //     The server from which to load the type. If the server name is null, this method
        //     automatically reverts to the local machine.
        //
        //   throwOnError:
        //     true to throw any exception that occurs.-or- false to ignore any exception that
        //     occurs.
        //
        // Returns:
        //     The type associated with the specified program identifier (progID), if progID
        //     is a valid entry in the registry and a type is associated with it; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     progID is null.
        //
        //   T:System.Runtime.InteropServices.COMException:
        //     The specified progID is not registered.
        [SecurityCritical]
        public static TypeProxy GetTypeFromProgID(string progID, string server, bool throwOnError)
            => Type.GetTypeFromProgID(progID, server, throwOnError);
        //
        // Summary:
        //     Gets the type associated with the specified program identifier (ProgID), returning
        //     null if an error is encountered while loading the System.Type.
        //
        // Parameters:
        //   progID:
        //     The ProgID of the type to get.
        //
        // Returns:
        //     The type associated with the specified ProgID, if progID is a valid entry in
        //     the registry and a type is associated with it; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     progID is null.
        [SecurityCritical]
        public static TypeProxy GetTypeFromProgID(string progID)
            => Type.GetTypeFromProgID(progID);
        //
        // Summary:
        //     Gets the handle for the System.Type of a specified object.
        //
        // Parameters:
        //   o:
        //     The object for which to get the type handle.
        //
        // Returns:
        //     The handle for the System.Type of the specified System.Object.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     o is null.
        public static RuntimeTypeHandle GetTypeHandle(object o)
            => Type.GetTypeHandle(o);
        //
        // Summary:
        //     Gets the System.Type with the specified name, specifying whether to perform a
        //     case-sensitive search and whether to throw an exception if the type is not found.
        //     The type is loaded for reflection only, not for execution.
        //
        // Parameters:
        //   typeName:
        //     The assembly-qualified name of the System.Type to get.
        //
        //   throwIfNotFound:
        //     true to throw a System.TypeLoadException if the type cannot be found; false to
        //     return null if the type cannot be found. Specifying false also suppresses some
        //     other exception conditions, but not all of them. See the Exceptions section.
        //
        //   ignoreCase:
        //     true to perform a case-insensitive search for typeName; false to perform a case-sensitive
        //     search for typeName.
        //
        // Returns:
        //     The type with the specified name, if found; otherwise, null. If the type is not
        //     found, the throwIfNotFound parameter specifies whether null is returned or an
        //     exception is thrown. In some cases, an exception is thrown regardless of the
        //     value of throwIfNotFound. See the Exceptions section.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     typeName is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        //
        //   T:System.TypeLoadException:
        //     throwIfNotFound is true and the type is not found. -or- throwIfNotFound is true
        //     and typeName contains invalid characters, such as an embedded tab.-or- throwIfNotFound
        //     is true and typeName is an empty string.-or- throwIfNotFound is true and typeName
        //     represents an array type with an invalid size. -or- typeName represents an array
        //     of System.TypedReference objects.
        //
        //   T:System.ArgumentException:
        //     typeName does not include the assembly name.-or- throwIfNotFound is true and
        //     typeName contains invalid syntax; for example, "MyType[,*,]".-or- typeName represents
        //     a generic type that has a pointer type, a ByRef type, or System.Void as one of
        //     its type arguments.-or- typeName represents a generic type that has an incorrect
        //     number of type arguments.-or- typeName represents a generic type, and one of
        //     its type arguments does not satisfy the constraints for the corresponding type
        //     parameter.
        //
        //   T:System.IO.FileNotFoundException:
        //     throwIfNotFound is true and the assembly or one of its dependencies was not found.
        //
        //   T:System.IO.FileLoadException:
        //     The assembly or one of its dependencies was found, but could not be loaded.
        //
        //   T:System.BadImageFormatException:
        //     The assembly or one of its dependencies is not valid. -or-The assembly was compiled
        //     with a later version of the common language runtime than the version that is
        //     currently loaded.
        public static TypeProxy ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase)
            => Type.ReflectionOnlyGetType(typeName, throwIfNotFound, ignoreCase);
        //
        // Summary:
        //     Determines if the underlying system type of the current System.Type object is
        //     the same as the underlying system type of the specified System.Object.
        //
        // Parameters:
        //   o:
        //     The object whose underlying system type is to be compared with the underlying
        //     system type of the current System.Type. For the comparison to succeed, o must
        //     be able to be cast or converted to an object of type System.Type.
        //
        // Returns:
        //     true if the underlying system type of o is the same as the underlying system
        //     type of the current System.Type; otherwise, false. This method also returns false
        //     if: . o is null. o cannot be cast or converted to a System.Type object.
        public override bool Equals(object o)
            => o is TypeProxy proxy && EqualTo(proxy);
        //
        // Summary:
        //     Determines if the underlying system type of the current System.Type is the same
        //     as the underlying system type of the specified System.Type.
        //
        // Parameters:
        //   o:
        //     The object whose underlying system type is to be compared with the underlying
        //     system type of the current System.Type.
        //
        // Returns:
        //     true if the underlying system type of o is the same as the underlying system
        //     type of the current System.Type; otherwise, false.
        public bool Equals(TypeProxy o)
            => Value.Equals(o);
        //
        // Summary:
        //     Determines if the underlying system type of the current System.Type is the same
        //     as the underlying system type of the specified System.Type.
        //
        // Parameters:
        //   o:
        //     The object whose underlying system type is to be compared with the underlying
        //     system type of the current System.Type.
        //
        // Returns:
        //     true if the underlying system type of o is the same as the underlying system
        //     type of the current System.Type; otherwise, false.
        public bool Equals(Type o)
            => Value.Equals(o);
        //
        // Summary:
        //     Returns an array of System.Type objects representing a filtered list of interfaces
        //     implemented or inherited by the current System.Type.
        //
        // Parameters:
        //   filter:
        //     The delegate that compares the interfaces against filterCriteria.
        //
        //   filterCriteria:
        //     The search criteria that determines whether an interface should be included in
        //     the returned array.
        //
        // Returns:
        //     An array of System.Type objects representing a filtered list of the interfaces
        //     implemented or inherited by the current System.Type, or an empty array of type
        //     System.Type if no interfaces matching the filter are implemented or inherited
        //     by the current System.Type.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     filter is null.
        //
        //   T:System.Reflection.TargetInvocationException:
        //     A static initializer is invoked and throws an exception.
        public TypeProxy[] FindInterfaces(TypeFilter filter, object filterCriteria)
            => Value.FindInterfaces(filter, filterCriteria).Select(x => Get(x)).ToArray();
        //
        // Summary:
        //     Returns a filtered array of System.Reflection.MemberInfo objects of the specified
        //     member type.
        //
        // Parameters:
        //   memberType:
        //     An object that indicates the type of member to search for.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   filter:
        //     The delegate that does the comparisons, returning true if the member currently
        //     being inspected matches the filterCriteria and false otherwise. You can use the
        //     FilterAttribute, FilterName, and FilterNameIgnoreCase delegates supplied by this
        //     class. The first uses the fields of FieldAttributes, MethodAttributes, and MethodImplAttributes
        //     as search criteria, and the other two delegates use String objects as the search
        //     criteria.
        //
        //   filterCriteria:
        //     The search criteria that determines whether a member is returned in the array
        //     of MemberInfo objects.The fields of FieldAttributes, MethodAttributes, and MethodImplAttributes
        //     can be used in conjunction with the FilterAttribute delegate supplied by this
        //     class.
        //
        // Returns:
        //     A filtered array of System.Reflection.MemberInfo objects of the specified member
        //     type.-or- An empty array of type System.Reflection.MemberInfo, if the current
        //     System.Type does not have members of type memberType that match the filter criteria.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     filter is null.
        public MemberInfoProxy[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
            => Value.FindMembers(memberType, bindingAttr, filter, filterCriteria).Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Gets the number of dimensions in an array.
        //
        // Returns:
        //     An integer that contains the number of dimensions in the current type.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The functionality of this method is unsupported in the base class and must be
        //     implemented in a derived class instead.
        //
        //   T:System.ArgumentException:
        //     The current type is not an array.
        public int GetArrayRank()
            => Value.GetArrayRank();
        //
        // Summary:
        //     Searches for a constructor whose parameters match the specified argument types
        //     and modifiers, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the constructor to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.-or-
        //     System.Type.EmptyTypes.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the parameter type array. The default
        //     binder does not process this parameter.
        //
        // Returns:
        //     A System.Reflection.ConstructorInfo object representing the constructor that
        //     matches the specified requirements, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        [ComVisible(true)]
        public ConstructorInfoProxy GetConstructor(BindingFlags bindingAttr, Binder binder, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetConstructor(bindingAttr, binder, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for a constructor whose parameters match the specified argument types
        //     and modifiers, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the constructor to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.-or-
        //     System.Type.EmptyTypes.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the parameter type array. The default
        //     binder does not process this parameter.
        //
        // Returns:
        //     A System.Reflection.ConstructorInfo object representing the constructor that
        //     matches the specified requirements, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        [ComVisible(true)]
        public ConstructorInfoProxy GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
            => Value.GetConstructor(bindingAttr, binder, types, modifiers);
        //
        // Summary:
        //     Searches for a constructor whose parameters match the specified argument types
        //     and modifiers, using the specified binding constraints and the specified calling
        //     convention.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   callConvention:
        //     The object that specifies the set of rules to use regarding the order and layout
        //     of arguments, how the return value is passed, what registers are used for arguments,
        //     and the stack is cleaned up.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the constructor to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. The default binder
        //     does not process this parameter.
        //
        // Returns:
        //     An object representing the constructor that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        [ComVisible(true)]
        public ConstructorInfoProxy GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetConstructor(bindingAttr, binder, callConvention, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for a constructor whose parameters match the specified argument types
        //     and modifiers, using the specified binding constraints and the specified calling
        //     convention.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   callConvention:
        //     The object that specifies the set of rules to use regarding the order and layout
        //     of arguments, how the return value is passed, what registers are used for arguments,
        //     and the stack is cleaned up.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the constructor to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. The default binder
        //     does not process this parameter.
        //
        // Returns:
        //     An object representing the constructor that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        [ComVisible(true)]
        public ConstructorInfoProxy GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => Value.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        //
        // Summary:
        //     Searches for a public instance constructor whose parameters match the types in
        //     the specified array.
        //
        // Parameters:
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the desired constructor.-or- An empty array of System.Type objects,
        //     to get a constructor that takes no parameters. Such an empty array is provided
        //     by the static field System.Type.EmptyTypes.
        //
        // Returns:
        //     An object representing the public instance constructor whose parameters match
        //     the types in the parameter type array, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        [ComVisible(true)]
        public ConstructorInfoProxy GetConstructor(TypeProxy[] types)
            => Value.GetConstructor(types.Select(x => x.Value).ToArray());
        //
        // Summary:
        //     Searches for a public instance constructor whose parameters match the types in
        //     the specified array.
        //
        // Parameters:
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the desired constructor.-or- An empty array of System.Type objects,
        //     to get a constructor that takes no parameters. Such an empty array is provided
        //     by the static field System.Type.EmptyTypes.
        //
        // Returns:
        //     An object representing the public instance constructor whose parameters match
        //     the types in the parameter type array, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        [ComVisible(true)]
        public ConstructorInfoProxy GetConstructor(Type[] types)
            => Value.GetConstructor(types);
        //
        // Summary:
        //     When overridden in a derived class, searches for the constructors defined for
        //     the current System.Type, using the specified BindingFlags.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An array of System.Reflection.ConstructorInfo objects representing all constructors
        //     defined for the current System.Type that match the specified binding constraints,
        //     including the type initializer if it is defined. Returns an empty array of type
        //     System.Reflection.ConstructorInfo if no constructors are defined for the current
        //     System.Type, if none of the defined constructors match the binding constraints,
        //     or if the current System.Type represents a type parameter in the definition of
        //     a generic type or generic method.
        [ComVisible(true)]
        public ConstructorInfoProxy[] GetConstructors(BindingFlags bindingAttr)
              => Value.GetConstructors(bindingAttr).Select(x => ConstructorInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Returns all the public constructors defined for the current System.Type.
        //
        // Returns:
        //     An array of System.Reflection.ConstructorInfo objects representing all the public
        //     instance constructors defined for the current System.Type, but not including
        //     the type initializer (static constructor). If no public instance constructors
        //     are defined for the current System.Type, or if the current System.Type represents
        //     a type parameter in the definition of a generic type or generic method, an empty
        //     array of type System.Reflection.ConstructorInfo is returned.
        [ComVisible(true)]
        public ConstructorInfoProxy[] GetConstructors()
              => Value.GetConstructors().Select(x => ConstructorInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the members defined for the current System.Type whose System.Reflection.DefaultMemberAttribute
        //     is set.
        //
        // Returns:
        //     An array of System.Reflection.MemberInfo objects representing all default members
        //     of the current System.Type.-or- An empty array of type System.Reflection.MemberInfo,
        //     if the current System.Type does not have default members.
        public MemberInfoProxy[] GetDefaultMembers()
              => Value.GetDefaultMembers().Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     When overridden in a derived class, returns the System.Type of the object encompassed
        //     or referred to by the current array, pointer or reference type.
        //
        // Returns:
        //     The System.Type of the object encompassed or referred to by the current array,
        //     pointer, or reference type, or null if the current System.Type is not an array
        //     or a pointer, or is not passed by reference, or represents a generic type or
        //     a type parameter in the definition of a generic type or generic method.
        public TypeProxy GetElementType()
              => Value.GetElementType();
        //
        // Summary:
        //     Returns the name of the constant that has the specified value, for the current
        //     enumeration type.
        //
        // Parameters:
        //   value:
        //     The value whose name is to be retrieved.
        //
        // Returns:
        //     The name of the member of the current enumeration type that has the specified
        //     value, or null if no such constant is found.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The current type is not an enumeration.-or- value is neither of the current type
        //     nor does it have the same underlying type as the current type.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        public string GetEnumName(object value)
              => Value.GetEnumName(value);
        //
        // Summary:
        //     Returns the names of the members of the current enumeration type.
        //
        // Returns:
        //     An array that contains the names of the members of the enumeration.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The current type is not an enumeration.
        public string[] GetEnumNames()
              => Value.GetEnumNames();
        //
        // Summary:
        //     Returns the underlying type of the current enumeration type.
        //
        // Returns:
        //     The underlying type of the current enumeration.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The current type is not an enumeration.-or-The enumeration type is not valid,
        //     because it contains more than one instance field.
        public TypeProxy GetEnumUnderlyingType()
              => Value.GetEnumUnderlyingType();
        //
        // Summary:
        //     Returns an array of the values of the constants in the current enumeration type.
        //
        // Returns:
        //     An array that contains the values. The elements of the array are sorted by the
        //     binary values (that is, the unsigned values) of the enumeration constants.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The current type is not an enumeration.
        public Array GetEnumValues()
              => Value.GetEnumValues();
        //
        // Summary:
        //     When overridden in a derived class, returns the System.Reflection.EventInfo object
        //     representing the specified event, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of an event which is declared or inherited by
        //     the current System.Type.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     The object representing the specified event that is declared or inherited by
        //     the current System.Type, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public EventInfoProxy GetEvent(string name, BindingFlags bindingAttr)
              => Value.GetEvent(name, bindingAttr);
        //
        // Summary:
        //     Returns the System.Reflection.EventInfo object representing the specified public
        //     event.
        //
        // Parameters:
        //   name:
        //     The string containing the name of an event that is declared or inherited by the
        //     current System.Type.
        //
        // Returns:
        //     The object representing the specified public event that is declared or inherited
        //     by the current System.Type, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public EventInfoProxy GetEvent(string name)
              => Value.GetEvent(name);
        //
        // Summary:
        //     When overridden in a derived class, searches for events that are declared or
        //     inherited by the current System.Type, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An array of System.Reflection.EventInfo objects representing all events that
        //     are declared or inherited by the current System.Type that match the specified
        //     binding constraints.-or- An empty array of type System.Reflection.EventInfo,
        //     if the current System.Type does not have events, or if none of the events match
        //     the binding constraints.
        public EventInfoProxy[] GetEvents(BindingFlags bindingAttr)
              => Value.GetEvents(bindingAttr).Select(x => EventInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Returns all the public events that are declared or inherited by the current System.Type.
        //
        // Returns:
        //     An array of System.Reflection.EventInfo objects representing all the public events
        //     which are declared or inherited by the current System.Type.-or- An empty array
        //     of type System.Reflection.EventInfo, if the current System.Type does not have
        //     public events.
        public EventInfoProxy[] GetEvents()
              => Value.GetEvents().Select(x => EventInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the specified field, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the data field to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An object representing the field that matches the specified requirements, if
        //     found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public FieldInfoProxy GetField(string name, BindingFlags bindingAttr)
              => Value.GetField(name, bindingAttr);
        //
        // Summary:
        //     Searches for the public field with the specified name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the data field to get.
        //
        // Returns:
        //     An object representing the public field with the specified name, if found; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        //
        //   T:System.NotSupportedException:
        //     This System.Type object is a System.Reflection.Emit.TypeBuilder whose System.Reflection.Emit.TypeBuilder.CreateType
        //     method has not yet been called.
        public FieldInfoProxy GetField(string name)
              => Value.GetField(name);
        //
        // Summary:
        //     When overridden in a derived class, searches for the fields defined for the current
        //     System.Type, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An array of System.Reflection.FieldInfo objects representing all fields defined
        //     for the current System.Type that match the specified binding constraints.-or-
        //     An empty array of type System.Reflection.FieldInfo, if no fields are defined
        //     for the current System.Type, or if none of the defined fields match the binding
        //     constraints.
        public FieldInfoProxy[] GetFields(BindingFlags bindingAttr)
              => Value.GetFields(bindingAttr).Select(x => FieldInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Returns all the public fields of the current System.Type.
        //
        // Returns:
        //     An array of System.Reflection.FieldInfo objects representing all the public fields
        //     defined for the current System.Type.-or- An empty array of type System.Reflection.FieldInfo,
        //     if no public fields are defined for the current System.Type.
        public FieldInfoProxy[] GetFields()
              => Value.GetFields().Select(x => FieldInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Returns an array of System.Type objects that represent the type arguments of
        //     a closed generic type or the type parameters of a generic type definition.
        //
        // Returns:
        //     An array of System.Type objects that represent the type arguments of a generic
        //     type. Returns an empty array if the current type is not a generic type.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class. Derived classes must provide
        //     an implementation.
        public TypeProxy[] GetGenericArguments()
              => Value.GetGenericArguments().Select(x => Get(x)).ToArray();
        //
        // Summary:
        //     Returns an array of System.Type objects that represent the constraints on the
        //     current generic type parameter.
        //
        // Returns:
        //     An array of System.Type objects that represent the constraints on the current
        //     generic type parameter.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current System.Type object is not a generic type parameter. That is, the
        //     System.Type.IsGenericParameter property returns false.
        public TypeProxy[] GetGenericParameterConstraints()
              => Value.GetGenericParameterConstraints().Select(x => Get(x)).ToArray();
        //
        // Summary:
        //     Returns a System.Type object that represents a generic type definition from which
        //     the current generic type can be constructed.
        //
        // Returns:
        //     A System.Type object representing a generic type from which the current type
        //     can be constructed.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current type is not a generic type. That is, System.Type.IsGenericType returns
        //     false.
        //
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class. Derived classes must provide
        //     an implementation.
        public TypeProxy GetGenericTypeDefinition()
              => Value.GetGenericTypeDefinition();
        //
        // Summary:
        //     Returns the hash code for this instance.
        //
        // Returns:
        //     The hash code for this instance.
        public override int GetHashCode()
            => Value.GetHashCode();
        //
        // Summary:
        //     When overridden in a derived class, searches for the specified interface, specifying
        //     whether to do a case-insensitive search for the interface name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the interface to get. For generic interfaces,
        //     this is the mangled name.
        //
        //   ignoreCase:
        //     true to ignore the case of that part of name that specifies the simple interface
        //     name (the part that specifies the namespace must be correctly cased).-or- false
        //     to perform a case-sensitive search for all parts of name.
        //
        // Returns:
        //     An object representing the interface with the specified name, implemented or
        //     inherited by the current System.Type, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     The current System.Type represents a type that implements the same generic interface
        //     with different type arguments.
        public Type GetInterface(string name, bool ignoreCase)
              => Value.GetInterface(name, ignoreCase);
        //
        // Summary:
        //     Searches for the interface with the specified name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the interface to get. For generic interfaces,
        //     this is the mangled name.
        //
        // Returns:
        //     An object representing the interface with the specified name, implemented or
        //     inherited by the current System.Type, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     The current System.Type represents a type that implements the same generic interface
        //     with different type arguments.
        public TypeProxy GetInterface(string name)
              => Value.GetInterface(name);
        //
        // Summary:
        //     Returns an interface mapping for the specified interface type.
        //
        // Parameters:
        //   interfaceType:
        //     The interface type to retrieve a mapping for.
        //
        // Returns:
        //     An object that represents the interface mapping for interfaceType.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     interfaceType is not implemented by the current type. -or-The interfaceType parameter
        //     does not refer to an interface. -or- interfaceType is a generic interface, and
        //     the current type is an array type.
        //
        //   T:System.ArgumentNullException:
        //     interfaceType is null.
        //
        //   T:System.InvalidOperationException:
        //     The current System.Type represents a generic type parameter; that is, System.Type.IsGenericParameter
        //     is true.
        //
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class. Derived classes must provide
        //     an implementation.
        [ComVisible(true)]
        public InterfaceMapping GetInterfaceMap(Type interfaceType)
              => Value.GetInterfaceMap(interfaceType);
        //
        // Summary:
        //     When overridden in a derived class, gets all the interfaces implemented or inherited
        //     by the current System.Type.
        //
        // Returns:
        //     An array of System.Type objects representing all the interfaces implemented or
        //     inherited by the current System.Type.-or- An empty array of type System.Type,
        //     if no interfaces are implemented or inherited by the current System.Type.
        //
        // Exceptions:
        //   T:System.Reflection.TargetInvocationException:
        //     A static initializer is invoked and throws an exception.
        public TypeProxy[] GetInterfaces()
              => Value.GetInterfaces().Select(x => Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the public members with the specified name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public members to get.
        //
        // Returns:
        //     An array of System.Reflection.MemberInfo objects representing the public members
        //     with the specified name, if found; otherwise, an empty array.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public MemberInfoProxy[] GetMember(string name)
              => Value.GetMember(name).Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the specified members, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the members to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return an empty array.
        //
        // Returns:
        //     An array of System.Reflection.MemberInfo objects representing the public members
        //     with the specified name, if found; otherwise, an empty array.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public MemberInfoProxy[] GetMember(string name, BindingFlags bindingAttr)
              => Value.GetMember(name, bindingAttr).Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the specified members of the specified member type, using the specified
        //     binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the members to get.
        //
        //   type:
        //     The value to search for.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return an empty array.
        //
        // Returns:
        //     An array of System.Reflection.MemberInfo objects representing the public members
        //     with the specified name, if found; otherwise, an empty array.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        //
        //   T:System.NotSupportedException:
        //     A derived class must provide an implementation.
        public MemberInfoProxy[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
            => Value.GetMember(name, type, bindingAttr).Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Returns all the public members of the current System.Type.
        //
        // Returns:
        //     An array of System.Reflection.MemberInfo objects representing all the public
        //     members of the current System.Type.-or- An empty array of type System.Reflection.MemberInfo,
        //     if the current System.Type does not have public members.
        public MemberInfoProxy[] GetMembers()
            => Value.GetMembers().Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     When overridden in a derived class, searches for the members defined for the
        //     current System.Type, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero (System.Reflection.BindingFlags.Default),
        //     to return an empty array.
        //
        // Returns:
        //     An array of System.Reflection.MemberInfo objects representing all members defined
        //     for the current System.Type that match the specified binding constraints.-or-
        //     An empty array of type System.Reflection.MemberInfo, if no members are defined
        //     for the current System.Type, or if none of the defined members match the binding
        //     constraints.
        public MemberInfoProxy[] GetMembers(BindingFlags bindingAttr)
            => Value.GetMembers(bindingAttr).Select(x => MemberInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the public method with the specified name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public method to get.
        //
        // Returns:
        //     An object that represents the public method with the specified name, if found;
        //     otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name.
        //
        //   T:System.ArgumentNullException:
        //     name is null.
        public MethodInfoProxy GetMethod(string name)
            => Value.GetMethod(name);
        //
        // Summary:
        //     Searches for the specified method, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the method to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An object representing the method that matches the specified requirements, if
        //     found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.
        public MethodInfoProxy GetMethod(string name, BindingFlags bindingAttr)
            => Value.GetMethod(name, bindingAttr);
        //
        // Summary:
        //     Searches for the specified public method whose parameters match the specified
        //     argument types and modifiers.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public method to get.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. To be only used
        //     when calling through COM interop, and only parameters that are passed by reference
        //     are handled. The default binder does not process this parameter.
        //
        // Returns:
        //     An object representing the public method that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and specified parameters.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.
        public MethodInfoProxy GetMethod(string name, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetMethod(name, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for the specified public method whose parameters match the specified
        //     argument types and modifiers.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public method to get.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. To be only used
        //     when calling through COM interop, and only parameters that are passed by reference
        //     are handled. The default binder does not process this parameter.
        //
        // Returns:
        //     An object representing the public method that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and specified parameters.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.
        public MethodInfoProxy GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
            => Value.GetMethod(name, types, modifiers);
        //
        // Summary:
        //     Searches for the specified method whose parameters match the specified argument
        //     types and modifiers, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the method to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. To be only used
        //     when calling through COM interop, and only parameters that are passed by reference
        //     are handled. The default binder does not process this parameter.
        //
        // Returns:
        //     An object representing the method that matches the specified requirements, if
        //     found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.
        public MethodInfoProxy GetMethod(string name, BindingFlags bindingAttr, Binder binder, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetMethod(name, bindingAttr, binder, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for the specified method whose parameters match the specified argument
        //     types and modifiers, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the method to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. To be only used
        //     when calling through COM interop, and only parameters that are passed by reference
        //     are handled. The default binder does not process this parameter.
        //
        // Returns:
        //     An object representing the method that matches the specified requirements, if
        //     found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.
        public MethodInfoProxy GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
            => Value.GetMethod(name, bindingAttr, binder, types, modifiers);
        //
        // Summary:
        //     Searches for the specified method whose parameters match the specified argument
        //     types and modifiers, using the specified binding constraints and the specified
        //     calling convention.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the method to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   callConvention:
        //     The object that specifies the set of rules to use regarding the order and layout
        //     of arguments, how the return value is passed, what registers are used for arguments,
        //     and how the stack is cleaned up.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. To be only used
        //     when calling through COM interop, and only parameters that are passed by reference
        //     are handled. The default binder does not process this parameter.
        //
        // Returns:
        //     An object representing the method that matches the specified requirements, if
        //     found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.
        public MethodInfoProxy GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetMethod(name, bindingAttr, binder, callConvention, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for the specified method whose parameters match the specified argument
        //     types and modifiers, using the specified binding constraints and the specified
        //     calling convention.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the method to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   callConvention:
        //     The object that specifies the set of rules to use regarding the order and layout
        //     of arguments, how the return value is passed, what registers are used for arguments,
        //     and how the stack is cleaned up.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. To be only used
        //     when calling through COM interop, and only parameters that are passed by reference
        //     are handled. The default binder does not process this parameter.
        //
        // Returns:
        //     An object representing the method that matches the specified requirements, if
        //     found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.
        public MethodInfoProxy GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            => Value.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
        //
        // Summary:
        //     Searches for the specified public method whose parameters match the specified
        //     argument types.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public method to get.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        // Returns:
        //     An object representing the public method whose parameters match the specified
        //     argument types, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and specified parameters.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        public MethodInfoProxy GetMethod(string name, TypeProxy[] types)
            => Value.GetMethod(name, types.Select(x => x.Value).ToArray());
        //
        // Summary:
        //     Searches for the specified public method whose parameters match the specified
        //     argument types.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public method to get.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the method to get.-or- An empty array of System.Type objects (as
        //     provided by the System.Type.EmptyTypes field) to get a method that takes no parameters.
        //
        // Returns:
        //     An object representing the public method whose parameters match the specified
        //     argument types, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method is found with the specified name and specified parameters.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.-or- One of the elements in types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        public MethodInfoProxy GetMethod(string name, Type[] types)
            => Value.GetMethod(name, types);
        //
        // Summary:
        //     When overridden in a derived class, searches for the methods defined for the
        //     current System.Type, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An array of System.Reflection.MethodInfo objects representing all methods defined
        //     for the current System.Type that match the specified binding constraints.-or-
        //     An empty array of type System.Reflection.MethodInfo, if no methods are defined
        //     for the current System.Type, or if none of the defined methods match the binding
        //     constraints.
        public MethodInfoProxy[] GetMethods(BindingFlags bindingAttr)
            => Value.GetMethods(bindingAttr).Select(x => MethodInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Returns all the public methods of the current System.Type.
        //
        // Returns:
        //     An array of System.Reflection.MethodInfo objects representing all the public
        //     methods defined for the current System.Type.-or- An empty array of type System.Reflection.MethodInfo,
        //     if no public methods are defined for the current System.Type.
        public MethodInfoProxy[] GetMethods()
            => Value.GetMethods().Select(x => MethodInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     When overridden in a derived class, searches for the specified nested type, using
        //     the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the nested type to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An object representing the nested type that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public Type GetNestedType(string name, BindingFlags bindingAttr)
            => Value.GetNestedType(name, bindingAttr);
        //
        // Summary:
        //     Searches for the public nested type with the specified name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the nested type to get.
        //
        // Returns:
        //     An object representing the public nested type with the specified name, if found;
        //     otherwise, null.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     name is null.
        public TypeProxy GetNestedType(string name)
            => Value.GetNestedType(name);
        //
        // Summary:
        //     When overridden in a derived class, searches for the types nested in the current
        //     System.Type, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An array of System.Type objects representing all the types nested in the current
        //     System.Type that match the specified binding constraints (the search is not recursive),
        //     or an empty array of type System.Type, if no nested types are found that match
        //     the binding constraints.
        public TypeProxy[] GetNestedTypes(BindingFlags bindingAttr)
            => Value.GetNestedTypes(bindingAttr).Select(x => Get(x)).ToArray();
        //
        // Summary:
        //     Returns the public types nested in the current System.Type.
        //
        // Returns:
        //     An array of System.Type objects representing the public types nested in the current
        //     System.Type (the search is not recursive), or an empty array of type System.Type
        //     if no public types are nested in the current System.Type.
        public TypeProxy[] GetNestedTypes()
            => Value.GetNestedTypes().Select(x => Get(x)).ToArray();
        //
        // Summary:
        //     Returns all the public properties of the current System.Type.
        //
        // Returns:
        //     An array of System.Reflection.PropertyInfo objects representing all public properties
        //     of the current System.Type.-or- An empty array of type System.Reflection.PropertyInfo,
        //     if the current System.Type does not have public properties.
        public PropertyInfoProxy[] GetProperties()
            => Value.GetProperties().Select(x => PropertyInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     When overridden in a derived class, searches for the properties of the current
        //     System.Type, using the specified binding constraints.
        //
        // Parameters:
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An array of System.Reflection.PropertyInfo objects representing all properties
        //     of the current System.Type that match the specified binding constraints.-or-
        //     An empty array of type System.Reflection.PropertyInfo, if the current System.Type
        //     does not have properties, or if none of the properties match the binding constraints.
        public PropertyInfoProxy[] GetProperties(BindingFlags bindingAttr)
            => Value.GetProperties(bindingAttr).Select(x => PropertyInfoProxy.Get(x)).ToArray();
        //
        // Summary:
        //     Searches for the public property with the specified name and return type.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   returnType:
        //     The return type of the property.
        //
        // Returns:
        //     An object representing the public property with the specified name, if found;
        //     otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name.
        //
        //   T:System.ArgumentNullException:
        //     name is null, or returnType is null.
        public PropertyInfoProxy GetProperty(string name, TypeProxy returnType)
            => Value.GetProperty(name, returnType.Value);
        //
        // Summary:
        //     Searches for the public property with the specified name and return type.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   returnType:
        //     The return type of the property.
        //
        // Returns:
        //     An object representing the public property with the specified name, if found;
        //     otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name.
        //
        //   T:System.ArgumentNullException:
        //     name is null, or returnType is null.
        public PropertyInfo GetProperty(string name, Type returnType)
            => Value.GetProperty(name, returnType);
        //
        // Summary:
        //     Searches for the specified public property whose parameters match the specified
        //     argument types.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        // Returns:
        //     An object representing the public property whose parameters match the specified
        //     argument types, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     argument types.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, TypeProxy[] types)
            => Value.GetProperty(name, types.Select(x => x.Value).ToArray());
        //
        // Summary:
        //     Searches for the specified public property whose parameters match the specified
        //     argument types.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        // Returns:
        //     An object representing the public property whose parameters match the specified
        //     argument types, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     argument types.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, Type[] types)
            => Value.GetProperty(name, types);
        //
        // Summary:
        //     Searches for the specified property whose parameters match the specified argument
        //     types and modifiers, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the property to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   returnType:
        //     The return type of the property.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. The default binder
        //     does not process this parameter.
        //
        // Returns:
        //     An object representing the property that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, BindingFlags bindingAttr, Binder binder, TypeProxy returnType, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetProperty(name, bindingAttr, binder, returnType.Value, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for the specified property whose parameters match the specified argument
        //     types and modifiers, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the property to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder.
        //
        //   returnType:
        //     The return type of the property.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. The default binder
        //     does not process this parameter.
        //
        // Returns:
        //     An object representing the property that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     binding constraints.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            => Value.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        //
        // Summary:
        //     Searches for the specified public property whose parameters match the specified
        //     argument types and modifiers.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   returnType:
        //     The return type of the property.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. The default binder
        //     does not process this parameter.
        //
        // Returns:
        //     An object representing the public property that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     argument types and modifiers.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, TypeProxy returnType, TypeProxy[] types, ParameterModifier[] modifiers)
            => Value.GetProperty(name, returnType.Value, types.Select(x => x.Value).ToArray(), modifiers);
        //
        // Summary:
        //     Searches for the specified public property whose parameters match the specified
        //     argument types and modifiers.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   returnType:
        //     The return type of the property.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the types array. The default binder
        //     does not process this parameter.
        //
        // Returns:
        //     An object representing the public property that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     argument types and modifiers.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.-or- modifiers is multidimensional.-or- types and modifiers
        //     do not have the same length.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
            => Value.GetProperty(name, returnType, types, modifiers);
        //
        // Summary:
        //     Searches for the specified property, using the specified binding constraints.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the property to get.
        //
        //   bindingAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted.-or- Zero, to return null.
        //
        // Returns:
        //     An object representing the property that matches the specified requirements,
        //     if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     binding constraints. See Remarks.
        //
        //   T:System.ArgumentNullException:
        //     name is null.
        public PropertyInfoProxy GetProperty(string name, BindingFlags bindingAttr)
            => Value.GetProperty(name, bindingAttr);
        //
        // Summary:
        //     Searches for the specified public property whose parameters match the specified
        //     argument types.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   returnType:
        //     The return type of the property.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        // Returns:
        //     An object representing the public property whose parameters match the specified
        //     argument types, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     argument types.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, TypeProxy returnType, TypeProxy[] types)
            => Value.GetProperty(name, returnType.Value, types.Select(x => x.Value).ToArray());
        //
        // Summary:
        //     Searches for the specified public property whose parameters match the specified
        //     argument types.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        //   returnType:
        //     The return type of the property.
        //
        //   types:
        //     An array of System.Type objects representing the number, order, and type of the
        //     parameters for the indexed property to get.-or- An empty array of the type System.Type
        //     (that is, Type[] types = new Type[0]) to get a property that is not indexed.
        //
        // Returns:
        //     An object representing the public property whose parameters match the specified
        //     argument types, if found; otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name and matching the specified
        //     argument types.
        //
        //   T:System.ArgumentNullException:
        //     name is null.-or- types is null.
        //
        //   T:System.ArgumentException:
        //     types is multidimensional.
        //
        //   T:System.NullReferenceException:
        //     An element of types is null.
        public PropertyInfoProxy GetProperty(string name, Type returnType, Type[] types)
            => Value.GetProperty(name, returnType, types);
        //
        // Summary:
        //     Searches for the public property with the specified name.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the public property to get.
        //
        // Returns:
        //     An object representing the public property with the specified name, if found;
        //     otherwise, null.
        //
        // Exceptions:
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one property is found with the specified name. See Remarks.
        //
        //   T:System.ArgumentNullException:
        //     name is null.
        public PropertyInfoProxy GetProperty(string name)
            => Value.GetProperty(name);
        //
        // Summary:
        //     Gets the current System.Type.
        //
        // Returns:
        //     The current System.Type.
        //
        // Exceptions:
        //   T:System.Reflection.TargetInvocationException:
        //     A class initializer is invoked and throws an exception.
        public new TypeProxy GetType()
            => Value.GetType();
        //
        // Summary:
        //     When overridden in a derived class, invokes the specified member, using the specified
        //     binding constraints and matching the specified argument list, modifiers and culture.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the constructor, method, property, or field
        //     member to invoke.-or- An empty string ("") to invoke the default member. -or-For
        //     IDispatch members, a string representing the DispID, for example "[DispID=3]".
        //
        //   invokeAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted. The access can be one of the BindingFlags such as
        //     Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup
        //     need not be specified. If the type of lookup is omitted, BindingFlags.Public
        //     | BindingFlags.Instance | BindingFlags.Static are used.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder. Note that explicitly defining a System.Reflection.Binder
        //     object may be required for successfully invoking method overloads with variable
        //     arguments.
        //
        //   target:
        //     The object on which to invoke the specified member.
        //
        //   args:
        //     An array containing the arguments to pass to the member to invoke.
        //
        //   modifiers:
        //     An array of System.Reflection.ParameterModifier objects representing the attributes
        //     associated with the corresponding element in the args array. A parameter's associated
        //     attributes are stored in the member's signature. The default binder processes
        //     this parameter only when calling a COM component.
        //
        //   culture:
        //     The System.Globalization.CultureInfo object representing the globalization locale
        //     to use, which may be necessary for locale-specific conversions, such as converting
        //     a numeric String to a Double.-or- A null reference (Nothing in Visual Basic)
        //     to use the current thread's System.Globalization.CultureInfo.
        //
        //   namedParameters:
        //     An array containing the names of the parameters to which the values in the args
        //     array are passed.
        //
        // Returns:
        //     An object representing the return value of the invoked member.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     invokeAttr does not contain CreateInstance and name is null.
        //
        //   T:System.ArgumentException:
        //     args and modifiers do not have the same length.-or- invokeAttr is not a valid
        //     System.Reflection.BindingFlags attribute.-or- invokeAttr does not contain one
        //     of the following binding flags: InvokeMethod, CreateInstance, GetField, SetField,
        //     GetProperty, or SetProperty.-or- invokeAttr contains CreateInstance combined
        //     with InvokeMethod, GetField, SetField, GetProperty, or SetProperty.-or- invokeAttr
        //     contains both GetField and SetField.-or- invokeAttr contains both GetProperty
        //     and SetProperty.-or- invokeAttr contains InvokeMethod combined with SetField
        //     or SetProperty.-or- invokeAttr contains SetField and args has more than one element.-or-
        //     The named parameter array is larger than the argument array.-or- This method
        //     is called on a COM object and one of the following binding flags was not passed
        //     in: BindingFlags.InvokeMethod, BindingFlags.GetProperty, BindingFlags.SetProperty,
        //     BindingFlags.PutDispProperty, or BindingFlags.PutRefDispProperty.-or- One of
        //     the named parameter arrays contains a string that is null.
        //
        //   T:System.MethodAccessException:
        //     The specified member is a class initializer.
        //
        //   T:System.MissingFieldException:
        //     The field or property cannot be found.
        //
        //   T:System.MissingMethodException:
        //     No method can be found that matches the arguments in args.-or- No member can
        //     be found that has the argument names supplied in namedParameters.-or- The current
        //     System.Type object represents a type that contains open type parameters, that
        //     is, System.Type.ContainsGenericParameters returns true.
        //
        //   T:System.Reflection.TargetException:
        //     The specified member cannot be invoked on target.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method matches the binding criteria.
        //
        //   T:System.InvalidOperationException:
        //     The method represented by name has one or more unspecified generic type parameters.
        //     That is, the method's System.Reflection.MethodInfo.ContainsGenericParameters
        //     property returns true.
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            => Value.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        //
        // Summary:
        //     Invokes the specified member, using the specified binding constraints and matching
        //     the specified argument list and culture.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the constructor, method, property, or field
        //     member to invoke.-or- An empty string ("") to invoke the default member. -or-For
        //     IDispatch members, a string representing the DispID, for example "[DispID=3]".
        //
        //   invokeAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted. The access can be one of the BindingFlags such as
        //     Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup
        //     need not be specified. If the type of lookup is omitted, BindingFlags.Public
        //     | BindingFlags.Instance | BindingFlags.Static are used.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder. Note that explicitly defining a System.Reflection.Binder
        //     object may be required for successfully invoking method overloads with variable
        //     arguments.
        //
        //   target:
        //     The object on which to invoke the specified member.
        //
        //   args:
        //     An array containing the arguments to pass to the member to invoke.
        //
        //   culture:
        //     The object representing the globalization locale to use, which may be necessary
        //     for locale-specific conversions, such as converting a numeric System.String to
        //     a System.Double.-or- A null reference (Nothing in Visual Basic) to use the current
        //     thread's System.Globalization.CultureInfo.
        //
        // Returns:
        //     An object representing the return value of the invoked member.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     invokeAttr does not contain CreateInstance and name is null.
        //
        //   T:System.ArgumentException:
        //     invokeAttr is not a valid System.Reflection.BindingFlags attribute. -or- invokeAttr
        //     does not contain one of the following binding flags: InvokeMethod, CreateInstance,
        //     GetField, SetField, GetProperty, or SetProperty.-or- invokeAttr contains CreateInstance
        //     combined with InvokeMethod, GetField, SetField, GetProperty, or SetProperty.-or-
        //     invokeAttr contains both GetField and SetField.-or- invokeAttr contains both
        //     GetProperty and SetProperty.-or- invokeAttr contains InvokeMethod combined with
        //     SetField or SetProperty.-or- invokeAttr contains SetField and args has more than
        //     one element.-or- This method is called on a COM object and one of the following
        //     binding flags was not passed in: BindingFlags.InvokeMethod, BindingFlags.GetProperty,
        //     BindingFlags.SetProperty, BindingFlags.PutDispProperty, or BindingFlags.PutRefDispProperty.-or-
        //     One of the named parameter arrays contains a string that is null.
        //
        //   T:System.MethodAccessException:
        //     The specified member is a class initializer.
        //
        //   T:System.MissingFieldException:
        //     The field or property cannot be found.
        //
        //   T:System.MissingMethodException:
        //     No method can be found that matches the arguments in args.-or- The current System.Type
        //     object represents a type that contains open type parameters, that is, System.Type.ContainsGenericParameters
        //     returns true.
        //
        //   T:System.Reflection.TargetException:
        //     The specified member cannot be invoked on target.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method matches the binding criteria.
        //
        //   T:System.InvalidOperationException:
        //     The method represented by name has one or more unspecified generic type parameters.
        //     That is, the method's System.Reflection.MethodInfo.ContainsGenericParameters
        //     property returns true.
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
            => Value.InvokeMember(name, invokeAttr, binder, target, args, culture);
        //
        // Summary:
        //     Invokes the specified member, using the specified binding constraints and matching
        //     the specified argument list.
        //
        // Parameters:
        //   name:
        //     The string containing the name of the constructor, method, property, or field
        //     member to invoke.-or- An empty string ("") to invoke the default member. -or-For
        //     IDispatch members, a string representing the DispID, for example "[DispID=3]".
        //
        //   invokeAttr:
        //     A bitmask comprised of one or more System.Reflection.BindingFlags that specify
        //     how the search is conducted. The access can be one of the BindingFlags such as
        //     Public, NonPublic, Private, InvokeMethod, GetField, and so on. The type of lookup
        //     need not be specified. If the type of lookup is omitted, BindingFlags.Public
        //     | BindingFlags.Instance | BindingFlags.Static are used.
        //
        //   binder:
        //     An object that defines a set of properties and enables binding, which can involve
        //     selection of an overloaded method, coercion of argument types, and invocation
        //     of a member through reflection.-or- A null reference (Nothing in Visual Basic),
        //     to use the System.Type.DefaultBinder. Note that explicitly defining a System.Reflection.Binder
        //     object may be required for successfully invoking method overloads with variable
        //     arguments.
        //
        //   target:
        //     The object on which to invoke the specified member.
        //
        //   args:
        //     An array containing the arguments to pass to the member to invoke.
        //
        // Returns:
        //     An object representing the return value of the invoked member.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     invokeAttr does not contain CreateInstance and name is null.
        //
        //   T:System.ArgumentException:
        //     invokeAttr is not a valid System.Reflection.BindingFlags attribute. -or- invokeAttr
        //     does not contain one of the following binding flags: InvokeMethod, CreateInstance,
        //     GetField, SetField, GetProperty, or SetProperty.-or- invokeAttr contains CreateInstance
        //     combined with InvokeMethod, GetField, SetField, GetProperty, or SetProperty.-or-
        //     invokeAttr contains both GetField and SetField.-or- invokeAttr contains both
        //     GetProperty and SetProperty.-or- invokeAttr contains InvokeMethod combined with
        //     SetField or SetProperty.-or- invokeAttr contains SetField and args has more than
        //     one element.-or- This method is called on a COM object and one of the following
        //     binding flags was not passed in: BindingFlags.InvokeMethod, BindingFlags.GetProperty,
        //     BindingFlags.SetProperty, BindingFlags.PutDispProperty, or BindingFlags.PutRefDispProperty.-or-
        //     One of the named parameter arrays contains a string that is null.
        //
        //   T:System.MethodAccessException:
        //     The specified member is a class initializer.
        //
        //   T:System.MissingFieldException:
        //     The field or property cannot be found.
        //
        //   T:System.MissingMethodException:
        //     No method can be found that matches the arguments in args.-or- The current System.Type
        //     object represents a type that contains open type parameters, that is, System.Type.ContainsGenericParameters
        //     returns true.
        //
        //   T:System.Reflection.TargetException:
        //     The specified member cannot be invoked on target.
        //
        //   T:System.Reflection.AmbiguousMatchException:
        //     More than one method matches the binding criteria.
        //
        //   T:System.NotSupportedException:
        //     The .NET Compact Framework does not currently support this method.
        //
        //   T:System.InvalidOperationException:
        //     The method represented by name has one or more unspecified generic type parameters.
        //     That is, the method's System.Reflection.MethodInfo.ContainsGenericParameters
        //     property returns true.
        [DebuggerHidden]
        [DebuggerStepThrough]
        public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
            => Value.InvokeMember(name, invokeAttr, binder, target, args);
        //
        // Summary:
        //     Determines whether an instance of a specified type can be assigned to an instance
        //     of the current type.
        //
        // Parameters:
        //   c:
        //     The type to compare with the current type.
        //
        // Returns:
        //     true if any of the following conditions is true: c and the current instance represent
        //     the same type. c is derived either directly or indirectly from the current instance.
        //     c is derived directly from the current instance if it inherits from the current
        //     instance; c is derived indirectly from the current instance if it inherits from
        //     a succession of one or more classes that inherit from the current instance. The
        //     current instance is an interface that c implements. c is a generic type parameter,
        //     and the current instance represents one of the constraints of c. In the following
        //     example, the current instance is a System.Type object that represents the System.IO.Stream
        //     class. GenericWithConstraint is a generic type whose generic type parameter must
        //     be of type System.IO.Stream. Passing its generic type parameter to the System.Type.IsAssignableFrom(System.Type)
        //     indicates that an instance of the generic type parameter can be assigned to an
        //     System.IO.Stream object. System.Type.IsAssignableFrom#2 c represents a value
        //     type, and the current instance represents Nullable<c> (Nullable(Of c) in Visual
        //     Basic). false if none of these conditions are true, or if c is null.
        public bool IsAssignableFrom(TypeProxy c)
        {
#if EDITOR
            if (c.Domain.IsGameDomain() && !Domain.IsGameDomain())
                return c.IsAssignableTo(this);
#endif
            return Value.IsAssignableFrom(c.Value);
        }
        public bool IsAssignableTo(TypeProxy c)
        {
#if EDITOR
            if (c.Domain.IsGameDomain() && !Domain.IsGameDomain())
                return c.IsAssignableFrom(this);
#endif
            return Value.IsAssignableTo(c.Value);
        }
        //
        // Summary:
        //     Determines whether an instance of a specified type can be assigned to an instance
        //     of the current type.
        //
        // Parameters:
        //   c:
        //     The type to compare with the current type.
        //
        // Returns:
        //     true if any of the following conditions is true: c and the current instance represent
        //     the same type. c is derived either directly or indirectly from the current instance.
        //     c is derived directly from the current instance if it inherits from the current
        //     instance; c is derived indirectly from the current instance if it inherits from
        //     a succession of one or more classes that inherit from the current instance. The
        //     current instance is an interface that c implements. c is a generic type parameter,
        //     and the current instance represents one of the constraints of c. In the following
        //     example, the current instance is a System.Type object that represents the System.IO.Stream
        //     class. GenericWithConstraint is a generic type whose generic type parameter must
        //     be of type System.IO.Stream. Passing its generic type parameter to the System.Type.IsAssignableFrom(System.Type)
        //     indicates that an instance of the generic type parameter can be assigned to an
        //     System.IO.Stream object. System.Type.IsAssignableFrom#2 c represents a value
        //     type, and the current instance represents Nullable<c> (Nullable(Of c) in Visual
        //     Basic). false if none of these conditions are true, or if c is null.
        public bool IsAssignableFrom(Type c)
            => Value.IsAssignableFrom(c);
        public bool IsAssignableTo(Type c)
           => Value.IsAssignableTo(c);
        //
        // Summary:
        //     Returns a value that indicates whether the specified value exists in the current
        //     enumeration type.
        //
        // Parameters:
        //   value:
        //     The value to be tested.
        //
        // Returns:
        //     true if the specified value is a member of the current enumeration type; otherwise,
        //     false.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The current type is not an enumeration.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.InvalidOperationException:
        //     value is of a type that cannot be the underlying type of an enumeration.
        public bool IsEnumDefined(object value)
            => Value.IsEnumDefined(value);
        //
        // Summary:
        //     Determines whether two COM types have the same identity and are eligible for
        //     type equivalence.
        //
        // Parameters:
        //   other:
        //     The COM type that is tested for equivalence with the current type.
        //
        // Returns:
        //     true if the COM types are equivalent; otherwise, false. This method also returns
        //     false if one type is in an assembly that is loaded for execution, and the other
        //     is in an assembly that is loaded into the reflection-only context.
        public bool IsEquivalentTo(TypeProxy other)
            => Value.IsEquivalentTo(other.Value);
        //
        // Summary:
        //     Determines whether two COM types have the same identity and are eligible for
        //     type equivalence.
        //
        // Parameters:
        //   other:
        //     The COM type that is tested for equivalence with the current type.
        //
        // Returns:
        //     true if the COM types are equivalent; otherwise, false. This method also returns
        //     false if one type is in an assembly that is loaded for execution, and the other
        //     is in an assembly that is loaded into the reflection-only context.
        public bool IsEquivalentTo(Type other)
            => Value.IsEquivalentTo(other);
        //
        // Summary:
        //     Determines whether the specified object is an instance of the current System.Type.
        //
        // Parameters:
        //   o:
        //     The object to compare with the current type.
        //
        // Returns:
        //     true if the current Type is in the inheritance hierarchy of the object represented
        //     by o, or if the current Type is an interface that o implements. false if neither
        //     of these conditions is the case, if o is null, or if the current Type is an open
        //     generic type (that is, System.Type.ContainsGenericParameters returns true).
        public bool IsInstanceOfType(object o)
            => Value.IsInstanceOfType(o);
        //
        // Summary:
        //     Determines whether the current System.Type derives from the specified System.Type.
        //
        // Parameters:
        //   c:
        //     The type to compare with the current type.
        //
        // Returns:
        //     true if the current Type derives from c; otherwise, false. This method also returns
        //     false if c and the current Type are equal.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     c is null.
        [ComVisible(true)]
        public bool IsSubclassOf(TypeProxy c)
            => Value.IsSubclassOf(c.Value);
        //
        // Summary:
        //     Determines whether the current System.Type derives from the specified System.Type.
        //
        // Parameters:
        //   c:
        //     The type to compare with the current type.
        //
        // Returns:
        //     true if the current Type derives from c; otherwise, false. This method also returns
        //     false if c and the current Type are equal.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     c is null.
        [ComVisible(true)]
        public bool IsSubclassOf(Type c)
            => Value.IsSubclassOf(c);
        //
        // Summary:
        //     Returns a System.Type object representing an array of the current type, with
        //     the specified number of dimensions.
        //
        // Parameters:
        //   rank:
        //     The number of dimensions for the array. This number must be less than or equal
        //     to 32.
        //
        // Returns:
        //     An object representing an array of the current type, with the specified number
        //     of dimensions.
        //
        // Exceptions:
        //   T:System.IndexOutOfRangeException:
        //     rank is invalid. For example, 0 or negative.
        //
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class.
        //
        //   T:System.TypeLoadException:
        //     The current type is System.TypedReference.-or-The current type is a ByRef type.
        //     That is, System.Type.IsByRef returns true. -or- rank is greater than 32.
        public TypeProxy MakeArrayType(int rank)
            => Value.MakeArrayType(rank);
        //
        // Summary:
        //     Returns a System.Type object representing a one-dimensional array of the current
        //     type, with a lower bound of zero.
        //
        // Returns:
        //     A System.Type object representing a one-dimensional array of the current type,
        //     with a lower bound of zero.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class. Derived classes must provide
        //     an implementation.
        //
        //   T:System.TypeLoadException:
        //     The current type is System.TypedReference.-or-The current type is a ByRef type.
        //     That is, System.Type.IsByRef returns true.
        public TypeProxy MakeArrayType()
            => Value.MakeArrayType();
        //
        // Summary:
        //     Returns a System.Type object that represents the current type when passed as
        //     a ref parameter (ByRef parameter in Visual Basic).
        //
        // Returns:
        //     A System.Type object that represents the current type when passed as a ref parameter
        //     (ByRef parameter in Visual Basic).
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class.
        //
        //   T:System.TypeLoadException:
        //     The current type is System.TypedReference.-or-The current type is a ByRef type.
        //     That is, System.Type.IsByRef returns true.
        public TypeProxy MakeByRefType()
            => Value.MakeByRefType();
        //
        // Summary:
        //     Substitutes the elements of an array of types for the type parameters of the
        //     current generic type definition and returns a System.Type object representing
        //     the resulting constructed type.
        //
        // Parameters:
        //   typeArguments:
        //     An array of types to be substituted for the type parameters of the current generic
        //     type.
        //
        // Returns:
        //     A System.Type representing the constructed type formed by substituting the elements
        //     of typeArguments for the type parameters of the current generic type.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current type does not represent a generic type definition. That is, System.Type.IsGenericTypeDefinition
        //     returns false.
        //
        //   T:System.ArgumentNullException:
        //     typeArguments is null.-or- Any element of typeArguments is null.
        //
        //   T:System.ArgumentException:
        //     The number of elements in typeArguments is not the same as the number of type
        //     parameters in the current generic type definition.-or- Any element of typeArguments
        //     does not satisfy the constraints specified for the corresponding type parameter
        //     of the current generic type. -or- typeArguments contains an element that is a
        //     pointer type (System.Type.IsPointer returns true), a by-ref type (System.Type.IsByRef
        //     returns true), or System.Void.
        //
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class. Derived classes must provide
        //     an implementation.
        public TypeProxy MakeGenericType(params TypeProxy[] typeArguments)
        {
#if EDITOR
            TypeProxy p = typeArguments.FirstOrDefault(x => x.Domain.IsGameDomain());
            if (p != null)
                return p.MakeGenericTypeInGameDomain(this, typeArguments.Where(x => x != p).ToArray());
#endif
            Type[] types = new Type[typeArguments.Length];
            //Engine.PrintLine(Domain.FriendlyName);
            for (int i = 0; i < types.Length; ++i)
            {
                //Engine.PrintLine(typeArguments[i].Domain.FriendlyName);
                types[i] = typeArguments[i].Value;
            }
            return Get(Value.MakeGenericType(types));
        }
#if EDITOR
        public TypeProxy MakeGenericTypeInGameDomain(TypeProxy mainType, params TypeProxy[] otherArgs)
        {
            //This type is also an argument
            var typeArguments = otherArgs.Append(this).ToArray();

            Type[] types = new Type[typeArguments.Length];
            //Engine.PrintLine(Domain.FriendlyName);
            for (int i = 0; i < types.Length; ++i)
            {
                //Engine.PrintLine(typeArguments[i].Domain.FriendlyName);
                types[i] = typeArguments[i].Value;
            }
            return Get(mainType.Value.MakeGenericType(types));
        }
#endif
        //
        // Summary:
        //     Substitutes the elements of an array of types for the type parameters of the
        //     current generic type definition and returns a System.Type object representing
        //     the resulting constructed type.
        //
        // Parameters:
        //   typeArguments:
        //     An array of types to be substituted for the type parameters of the current generic
        //     type.
        //
        // Returns:
        //     A System.Type representing the constructed type formed by substituting the elements
        //     of typeArguments for the type parameters of the current generic type.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The current type does not represent a generic type definition. That is, System.Type.IsGenericTypeDefinition
        //     returns false.
        //
        //   T:System.ArgumentNullException:
        //     typeArguments is null.-or- Any element of typeArguments is null.
        //
        //   T:System.ArgumentException:
        //     The number of elements in typeArguments is not the same as the number of type
        //     parameters in the current generic type definition.-or- Any element of typeArguments
        //     does not satisfy the constraints specified for the corresponding type parameter
        //     of the current generic type. -or- typeArguments contains an element that is a
        //     pointer type (System.Type.IsPointer returns true), a by-ref type (System.Type.IsByRef
        //     returns true), or System.Void.
        //
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class. Derived classes must provide
        //     an implementation.
        public TypeProxy MakeGenericType(params Type[] typeArguments)
            => Value.MakeGenericType(typeArguments);
        //
        // Summary:
        //     Returns a System.Type object that represents a pointer to the current type.
        //
        // Returns:
        //     A System.Type object that represents a pointer to the current type.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The invoked method is not supported in the base class.
        //
        //   T:System.TypeLoadException:
        //     The current type is System.TypedReference.-or-The current type is a ByRef type.
        //     That is, System.Type.IsByRef returns true.
        public TypeProxy MakePointerType()
            => Value.MakePointerType();
        //
        // Summary:
        //     Returns a String representing the name of the current Type.
        //
        // Returns:
        //     A System.String representing the name of the current System.Type.
        //public override string ToString() 
        //    => Value.ToString();

        //
        // Summary:
        //     Indicates whether two System.Type objects are equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is equal to right; otherwise, false.
        [SecuritySafeCritical]
        public static bool operator ==(TypeProxy left, TypeProxy right)
            => left is null ? right is null : left.EqualTo(right);
        //
        // Summary:
        //     Indicates whether two System.Type objects are not equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is not equal to right; otherwise, false.
        [SecuritySafeCritical]
        public static bool operator !=(TypeProxy left, TypeProxy right)
            => left is null ? !(right is null) : !left.EqualTo(right);

        //
        // Summary:
        //     Indicates whether two System.Type objects are equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is equal to right; otherwise, false.
        //[SecuritySafeCritical]
        //public static bool operator ==(TypeProxy left, Type right)
        //    => left?.Value == right;
        //
        // Summary:
        //     Indicates whether two System.Type objects are not equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is not equal to right; otherwise, false.
        //[SecuritySafeCritical]
        //public static bool operator !=(TypeProxy left, Type right)
        //    => left?.Value != right;
        //
        // Summary:
        //     Indicates whether two System.Type objects are equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is equal to right; otherwise, false.
        //[SecuritySafeCritical]
        //public static bool operator ==(Type left, TypeProxy right)
        //    => left == right?.Value;
        //
        // Summary:
        //     Indicates whether two System.Type objects are not equal.
        //
        // Parameters:
        //   left:
        //     The first object to compare.
        //
        //   right:
        //     The second object to compare.
        //
        // Returns:
        //     true if left is not equal to right; otherwise, false.
        //[SecuritySafeCritical]
        //public static bool operator !=(Type left, TypeProxy right)
        //    => left != right?.Value;

        public override string ToString() => "[" + Domain.FriendlyName + "] " + Value.FullName;
        public class EqualityComparer : IEqualityComparer<TypeProxy>
        {
            public bool Equals(TypeProxy x, TypeProxy y)
            {
                return x == y;
            }
            public int GetHashCode(TypeProxy x)
            {
                return x.GetHashCode();
            }
        }
    }
}
